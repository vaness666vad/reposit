#ifndef VIDEOSTREAM_HPP
#define VIDEOSTREAM_HPP

#include <stdio.h>
#include "libSTRUCTScode.h"

#include <stdlib.h>
#include <wiringPi.h>
#include <pthread.h>
#include <unistd.h>
#include <string.h>

#include <jpeglib.h>
#include <fcntl.h>
#include <errno.h>
#include <sys/ioctl.h>
#include <sys/types.h>
#include <sys/time.h>
#include <sys/mman.h>
#include <linux/videodev2.h>
#include <libv4l2.h>

#include <fcntl.h>
#include <errno.h>
#include <cstring>
#include <linux/videodev2.h>
#include <libv4l2.h>

#include "BufferOptions_Sign.hpp"
#include "RemoteControlled_Sign.hpp"
#include "VideoStream_Sign.hpp"
#define CLEAR(x) memset(&(x), 0, sizeof(x))

struct buffer
{
    void *start;
    size_t length;
};

//конструктор класса VideoStream
VideoStream::VideoStream(BufferOptions *bo, RemoteControlled *rc)
{
    BO = bo;
    RC = rc;
    VideoStreamIsEnable = false;
    DevsCount = 0;
    int devs = GetDevCount();
    char dev_name[15];
    memcpy(dev_name, "/dev/video", 10);
    for (int i = 2; i < devs; i += 1)
    {
        int fd = -1;
        char indexSTR[5];
        memset(indexSTR, '\0', sizeof(indexSTR));
        sprintf(indexSTR, "%d", i);
        memcpy(&dev_name[10], indexSTR, sizeof(indexSTR));
        DevInfo dinf;
        dinf.Devindex = i;
        dinf.SizeCount = 0;
        fd = v4l2_open(dev_name, O_RDWR | O_NONBLOCK, 0); //открывает устройство в не блокирующем режиму
        if (fd >= 0)
        {
            struct v4l2_format fmt; //формат изображения
            for (int s = 0; s < 15; s += 1)
            {
                CLEAR(fmt);
                fmt.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
                fmt.fmt.pix.width = Sizes[s][0];              //разрешение
                fmt.fmt.pix.height = Sizes[s][1];             //разрешение
                fmt.fmt.pix.pixelformat = V4L2_PIX_FMT_RGB24; //24 бита на пиксель то есть 3 байта обычный rgb
                fmt.fmt.pix.field = V4L2_FIELD_INTERLACED;
                int r = xioctl(fd, VIDIOC_S_FMT, &fmt); //задаем видеоформат
                if (r != -1 && fmt.fmt.pix.pixelformat == V4L2_PIX_FMT_RGB24 && fmt.fmt.pix.width == Sizes[s][0] && fmt.fmt.pix.height == Sizes[s][1])
                {
                    dinf.Sizes[dinf.SizeCount][0] = Sizes[s][0];
                    dinf.Sizes[dinf.SizeCount][1] = Sizes[s][1];
                    dinf.SizeCount += 1;
                }
            }
            if (dinf.SizeCount > 0)
            {
                Devs[DevsCount] = dinf;
                DevsCount += 1;
            }
        }
        v4l2_close(fd);
    }
    pthread_mutex_init(&mutexVideoStreamIsEnable, NULL);
    pthread_mutex_init(&mutexinfo, NULL);
}

//выполняет отправку сообщения серверу о камерах
void VideoStream::SendDevsInfo()
{
    int devInfSize = sizeof(DevInfo);
    int bufSize = devInfSize * DevsCount + 1;
    char buf[bufSize];
    buf[0] = 38;
    for (int i = 0; i < DevsCount; i += 1)
    {
        DevInfo d = Devs[i];
        memcpy(&buf[i * devInfSize + 1], &d, devInfSize);
    }
    RC->CS->TCPserCon->SendMessage(buf, bufSize);
}

bool VideoStream::GetVideoStreamStatus()
{
    bool res;
    pthread_mutex_lock(&mutexVideoStreamIsEnable);
    res = VideoStreamIsEnable;
    pthread_mutex_unlock(&mutexVideoStreamIsEnable);
    return res;
}
void VideoStream::SetVideoStreamStatus(bool val)
{
    pthread_mutex_lock(&mutexVideoStreamIsEnable);
    VideoStreamIsEnable = val;
    pthread_mutex_unlock(&mutexVideoStreamIsEnable);
}
void VideoStream::GetInfo(CamInfoSend *cis)
{
    pthread_mutex_lock(&mutexinfo);
    memcpy(cis, &info, sizeof(CamInfoSend));
    pthread_mutex_unlock(&mutexinfo);
}
void VideoStream::SetInfo(CamInfoSend *cis)
{
    pthread_mutex_lock(&mutexinfo);
    memcpy(&info, cis, sizeof(CamInfoSend));
    pthread_mutex_unlock(&mutexinfo);
}

//Запускает трансляцию видео
void VideoStream::StartStream(char *data, int lenght)
{
    CamInfoSend inf;
    memcpy(&inf, &data[1], sizeof(CamInfoSend));
    SetInfo(&inf);
    if (GetVideoStreamStatus() == false)
    {
        printf("Запуск новой трансляции %dX%d.\n", inf.Widht, inf.Height);
        SetVideoStreamStatus(true);
        pthread_t thread;
        pthread_create(&thread, NULL, this->Stream, this);
    }
    else
    {
        printf("Изменение параметров трансляции %dX%d.\n", inf.Widht, inf.Height);
    }
}

int VideoStream::CompressBufer(jpeg_compress_struct *cinfo, char *bufimg)
{
    jpeg_start_compress(cinfo, true);
    JOCTET *point = cinfo->dest->next_output_byte;
    size_t count =  cinfo->dest->free_in_buffer;
    JSAMPROW row_pointer[1];
    int row_stride = cinfo->image_width * 3;

    while (cinfo->next_scanline < cinfo->image_height)
    {
        row_pointer[0] = (JSAMPROW)&bufimg[cinfo->next_scanline * row_stride];
        jpeg_write_scanlines(cinfo, row_pointer, 1);
    }
    jpeg_finish_compress(cinfo);
    int res = (cinfo->image_width * cinfo->jpeg_height * 3) - cinfo->dest->free_in_buffer;
    cinfo->dest->next_output_byte = point;
    cinfo->dest->free_in_buffer = count;
    return res;
}

boolean my_empty_output_buffer(j_compress_ptr cinfo)
{
    return true;
}

void my_term_destination(j_compress_ptr cinfo)
{

}

void VideoStream::CompressSetBuffer(struct jpeg_compress_struct *cinfo, struct jpeg_destination_mgr *jdm, char *messagebuffer)
{
    jdm->free_in_buffer = cinfo->image_width * cinfo->image_height * 3;
    jdm->next_output_byte = (JOCTET *)messagebuffer;
    jdm->empty_output_buffer = &my_empty_output_buffer;
    jdm->term_destination = &my_term_destination;
    jdm->init_destination = &my_term_destination;
    cinfo->dest = jdm;
}

//инициализирует компрессор
void VideoStream::CompressInicialize(CamInfoSend *inf, struct jpeg_compress_struct *outcinfo, struct jpeg_error_mgr *jerr)
{
    outcinfo->err = jpeg_std_error(jerr);
    jpeg_create_compress(outcinfo);

    outcinfo->image_width = inf->Widht;
    outcinfo->image_height = inf->Height;
    outcinfo->input_components = 3;
    outcinfo->in_color_space = JCS_RGB;

    jpeg_set_defaults(outcinfo);
    jpeg_set_quality(outcinfo, inf->Quality, true);
}

void VideoStream::CompressClose(jpeg_compress_struct *cinfo)
{
    jpeg_destroy_compress(cinfo);
}

void *(VideoStream::Stream(void *link))
{
    VideoStream *vs = (VideoStream *)link;
    while (vs->GetVideoStreamStatus())
    {

        CamInfoSend Startinfo;
        int CamInfSendSize = sizeof(CamInfoSend);
        vs->GetInfo(&Startinfo);

        //инициализация буфера служащего для передачи готового сообщения
        char messagebuffer[1 + Startinfo.Widht * Startinfo.Height * 3]; //выделяем память без сжатия с запасом
        messagebuffer[0] = 37;                                                           //указываем тип сообщения "видеосигнал"
        char *messageData = &messagebuffer[1];                          //указатель на память для записи картинки

        //инициализируем компрессор
        struct jpeg_compress_struct cinfo;
        struct jpeg_error_mgr jerr;
        struct jpeg_destination_mgr jdm;
        vs->CompressInicialize(&Startinfo, &cinfo, &jerr);

        struct v4l2_format fmt;         //формат изображения
        struct v4l2_buffer buf;         //сохраняет соответствующую информацию о буфере команд
        struct v4l2_requestbuffers req; //уровень драйвера V4L2 выделяет сюда видеобуфер
        enum v4l2_buf_type type;        //тип видеобуфера
        fd_set fds;
        struct timeval tv;
        int r, fd = -1;
        unsigned int i, n_buffers;
        char dev_name[15];
        char out_name[256];
        FILE *fout;
        struct buffer *buffers; //свой самописный буфер хранящий указатель на массив пикселей
        memcpy(dev_name, "/dev/video", 10);
        char indexSTR[5];
        memset(indexSTR, '\0', sizeof(indexSTR));
        sprintf(indexSTR, "%d", Startinfo.DevIndex);
        memcpy(&dev_name[10], indexSTR, 5);
        fd = v4l2_open(dev_name, O_RDWR | O_NONBLOCK, 0); //открывает устройство в не блокирующем режиму
        CLEAR(fmt);
        fmt.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
        fmt.fmt.pix.width = Startinfo.Widht;          //разрешение
        fmt.fmt.pix.height = Startinfo.Height;        //разрешение
        fmt.fmt.pix.pixelformat = V4L2_PIX_FMT_RGB24; //24 бита на пиксель то есть 3 байта обычный rgb
        fmt.fmt.pix.field = V4L2_FIELD_INTERLACED;
        xioctl(fd, VIDIOC_S_FMT, &fmt); //задаем видеоформат

        CLEAR(req);    //чистим буфер который выделяется из памяти камеры
        req.count = 2; // Количество буферов приложения
        req.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
        req.memory = V4L2_MEMORY_MMAP;
        xioctl(fd, VIDIOC_REQBUFS, &req); //запросить драйвер V4L2 для выделения видеобуфера
        //VIDIOC_REQBUFS изменит значение счетчика req.count, а значение счетчика req.count возвращает фактическое количество успешно примененных видеобуфер

        buffers = (buffer *)calloc(req.count, sizeof(*buffers)); //выделяем память на колличество буферов приложения размером самописного буфера
        for (n_buffers = 0; n_buffers < req.count; ++n_buffers)
        {
            CLEAR(buf); //очищаем буфер команд

            buf.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
            buf.memory = V4L2_MEMORY_MMAP;
            buf.index = n_buffers;

            xioctl(fd, VIDIOC_QUERYBUF, &buf); //запросить соответствующую информацию о выделенном видеобуфере V4L2

            buffers[n_buffers].length = buf.length;
            buffers[n_buffers].start = v4l2_mmap(NULL, buf.length,
                                                 PROT_READ | PROT_WRITE, MAP_SHARED,
                                                 fd, buf.m.offset); //вызвать функцию mmap для сопоставления адреса пространства ядра с пространством пользователя,
                                                                    //чтобы приложение могло получить доступ к видеобуферу в пространстве ядра.
        }

        for (i = 0; i < n_buffers; ++i)
        {
            CLEAR(buf);
            buf.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
            buf.memory = V4L2_MEMORY_MMAP;
            buf.index = i;
            xioctl(fd, VIDIOC_QBUF, &buf); //Функция: Поместить пустой видеобуфер во входную очередь видеобуфера
        }
        type = V4L2_BUF_TYPE_VIDEO_CAPTURE;

        xioctl(fd, VIDIOC_STREAMON, &type); //Функция: запуск команды захвата видео, вызов приложенияVIDIOC_STREAMONПосле запуска команды
        // захвата видео драйвер видеоустройства начинает захват видеоданных и сохраняет захваченные видеоданные в видеобуфер видеодрайвера.
        while (true)
        {
            do
            {
                FD_ZERO(&fds);
                FD_SET(fd, &fds);

                /* Timeout. */
                tv.tv_sec = 2;
                tv.tv_usec = 0;

                r = select(fd + 1, &fds, NULL, NULL, &tv);
            } while ((r == -1 && (errno = EINTR)));

            CLEAR(buf);
            buf.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
            buf.memory = V4L2_MEMORY_MMAP;
            xioctl(fd, VIDIOC_DQBUF, &buf); //Функция: получить видеобуфер, в котором сохранен один кадр видеоданных, из очереди вывода видеобуфера

            vs->CompressSetBuffer(&cinfo, &jdm, messageData);
            int jpegSize = vs->CompressBufer(&cinfo, (char *)buffers[buf.index].start);
            vs->RC->CS->TCPserCon->SendMessage(messagebuffer, jpegSize + 1);

            xioctl(fd, VIDIOC_QBUF, &buf);

            CamInfoSend Testinfo;
            vs->GetInfo(&Testinfo);
            if (vs->RC->CS->TCPserCon->GetConnectionStatus() != 2)
            {
                vs->SetVideoStreamStatus(false);
                break;
            }
            else if (vs->GetVideoStreamStatus() == false)
            {
                break;
            }
            else if (memcmp(&Testinfo, &Startinfo, CamInfSendSize) != 0)
            {
                memcpy(&Startinfo, &Testinfo, CamInfSendSize);
                break;
            }
        }
        vs->CompressClose(&cinfo);
        type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
        xioctl(fd, VIDIOC_STREAMOFF, &type);
        for (i = 0; i < n_buffers; ++i)
            v4l2_munmap(buffers[i].start, buffers[i].length);
        v4l2_close(fd);
    }
    printf("Трансляция прервана.\n");
}

int VideoStream::xioctl(int fh, int request, void *arg)
{
    int r;
    do
    {
        r = v4l2_ioctl(fh, request, arg);
    } while (r == -1 && ((errno == EINTR) || (errno == EAGAIN)));
    return r;
}

int VideoStream::GetDevCount()
{
    char bufer[15];
    memcpy(bufer, "/dev/video", 10);
    for (int i = 0;; i += 1)
    {
        char indexSTR[5];
        memset(indexSTR, '\0', sizeof(indexSTR));
        sprintf(indexSTR, "%d", i);
        memcpy(&bufer[10], indexSTR, sizeof(indexSTR));
        FILE *file;
        if ((file = fopen(bufer, "r")))
        {
            fclose(file);
        }
        else
        {
            return i;
        }
    }
}
#endif