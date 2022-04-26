#ifndef VIDEOSTREAM_SIGN_HPP
#define VIDEOSTREAM_SIGN_HPP

#include "libSTRUCTScode.h"
#include "BufferOptions_Sign.hpp"
#include "RemoteControlled_Sign.hpp"

//класс выполняет обработку данных с камер
class VideoStream
{
public:
    //конструктор класса VideoStream
    VideoStream(BufferOptions *bo, RemoteControlled *rc);
    //выполняет отправку сообщения серверу о камерах
    void SendDevsInfo();
    //Запускает трансляцию видео
    void StartStream(char *data, int lenght);
    bool GetVideoStreamStatus();
    void SetVideoStreamStatus(bool val);
    void GetInfo(CamInfoSend *cis);
    void SetInfo(CamInfoSend *cis);

private:
    BufferOptions *BO;
    RemoteControlled *RC;
    pthread_mutex_t mutexinfo;
    struct CamInfoSend info;
    pthread_mutex_t mutexVideoStreamIsEnable;
    bool VideoStreamIsEnable;
    int DevsCount;
    DevInfo Devs[100];
    static int xioctl(int fh, int request, void *arg);
    int Sizes[15][2] = {{320, 240}, {640, 360}, {640, 480}, {800, 600}, {848, 480}, {960, 540}, {1280, 720}, {1280, 1024}, {1600, 1200}, {1920, 1080}, {2048, 1536}, {2560, 1440}, {2591, 1944}, {3840, 2160}, {4096, 2160}};
    int GetDevCount();
    void CompressInicialize(CamInfoSend *inf, struct jpeg_compress_struct *outcinfo, struct jpeg_error_mgr *jerr);
    void CompressSetBuffer(struct jpeg_compress_struct *cinfo, struct jpeg_destination_mgr *jdm, char *messagebuffer);
    void CompressClose(jpeg_compress_struct *cinfo);
    static void *Stream(void *link);
    int CompressBufer(jpeg_compress_struct *cinfo, char *bufimg);
};
#endif