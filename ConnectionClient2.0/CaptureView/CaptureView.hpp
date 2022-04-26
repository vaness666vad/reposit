#ifndef CAPTUREVIEW_HPP
#define CAPTUREVIEW_HPP

#include <stdlib.h>
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
#include <cstring>
#include <string>
#include <iostream>
#include <dirent.h>

#include "CaptureView_Sign.hpp"

#include "../ConnectModule/ProtocolCommunication/MessagePack/CaptureInfo/DeviceName.hpp"
#include "../ConnectModule/ProtocolCommunication/MessagePack/CaptureInfo/SupportedFormat.hpp"
#include "../ConnectModule/ProtocolCommunication/MessagePack/CaptureInfo/DevFormatSize.hpp"

#include "../ConnectModule/TcpConnection/TcpConnection.hpp"

string CaptureView::driverdir = "/dev/v4l/by-id/";
bool CaptureView::issupportFmt(unsigned int fmt, string *outName)
{
    switch (fmt)
    {
    case V4L2_PIX_FMT_MJPEG:
        *outName = "MJPEG";
        return true;
    case V4L2_PIX_FMT_JPEG:
        *outName = "JPEG";
        return true;
    case V4L2_PIX_FMT_DV:
        *outName = "DV";
        return true;
    case V4L2_PIX_FMT_MPEG:
        *outName = "MPEG";
        return true;
    case V4L2_PIX_FMT_H264:
        *outName = "H264";
        return true;
    case V4L2_PIX_FMT_H264_NO_SC:
        *outName = "H264_NO_SC";
        return true;
    case V4L2_PIX_FMT_H264_MVC:
        *outName = "H264_MVC";
        return true;
    case V4L2_PIX_FMT_H263:
        *outName = "H263";
        return true;
    case V4L2_PIX_FMT_MPEG1:
        *outName = "MPEG1";
        return true;
    case V4L2_PIX_FMT_MPEG2:
        *outName = "MPEG2";
        return true;
    case V4L2_PIX_FMT_MPEG2_SLICE:
        *outName = "MPEG2_SLICE";
        return true;
    case V4L2_PIX_FMT_MPEG4:
        *outName = "MPEG4";
        return true;
    case V4L2_PIX_FMT_XVID:
        *outName = "XVID";
        return true;
    case V4L2_PIX_FMT_VC1_ANNEX_G:
        *outName = "ANNEX_G";
        return true;
    case V4L2_PIX_FMT_VC1_ANNEX_L:
        *outName = "ANNEX_L";
        return true;
    case V4L2_PIX_FMT_VP8:
        *outName = "VP8";
        return true;
    case V4L2_PIX_FMT_VP9:
        *outName = "VP9";
        return true;
    case V4L2_PIX_FMT_HEVC:
        *outName = "HEVC";
        return true;
    case V4L2_PIX_FMT_FWHT:
        *outName = "FWHT";
        return true;
    case V4L2_PIX_FMT_FWHT_STATELESS:
        *outName = "FWHT_STATELESS";
        return true;
    default:
        return false;
    }
}
void CaptureView::SendDevicesInfo()
{
    if (TcpConnection::TC->TerminalSession->IsConnect())
    {
        DIR *dp = opendir(driverdir.c_str());
        struct dirent *dirp;
        if (dp != NULL)
        {
            while ((dirp = readdir(dp)) != NULL)
            {
                string id = string(dirp->d_name);
                string fullname = driverdir + id;
                int fd = v4l2_open(fullname.c_str(), O_RDWR | O_NONBLOCK, 0);
                if (fd != -1)
                {
                    v4l2_capability capaldata = {0};
                    if (v4l2_ioctl(fd, VIDIOC_QUERYCAP, &capaldata) == 0)
                    {
                        v4l2_fmtdesc fmtdata = {0};
                        fmtdata.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
                        while (v4l2_ioctl(fd, VIDIOC_ENUM_FMT, &fmtdata) == 0)
                        {
                            if(fmtdata.index == 0)
                            {
                                DeviceName devn = DeviceName(id, string((char*)&capaldata.card[0]));
                                TcpConnection::TC->TerminalSession->Write(&devn, devn.Size());
                            }
                            string name;
                            if (issupportFmt(fmtdata.pixelformat, &name))
                            {
                                SupportedFormat supf = SupportedFormat(id, fmtdata.pixelformat, name);
                                TcpConnection::TC->TerminalSession->Write(&supf, supf.Size());

                                v4l2_frmsizeenum frmData = {0};
                                frmData.pixel_format = fmtdata.pixelformat;
                                while (v4l2_ioctl(fd, VIDIOC_ENUM_FRAMESIZES, &frmData) == 0)
                                {
                                    DevFormatSize devfsize = DevFormatSize(id, fmtdata.pixelformat, frmData.discrete.width, frmData.discrete.height);
                                    TcpConnection::TC->TerminalSession->Write(&devfsize, devfsize.Size());
                                    
                                    frmData.index += 1;
                                }
                            }
                            fmtdata.index += 1;
                        }
                    }
                    v4l2_close(fd);
                }
            }
            closedir(dp);
        }
    }
}

#endif