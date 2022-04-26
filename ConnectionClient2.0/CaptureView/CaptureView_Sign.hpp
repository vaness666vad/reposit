#ifndef CAPTUREVIEW_SIGN_HPP
#define CAPTUREVIEW_SIGN_HPP

#include <string.h>
#include <cstring>
#include <string>

class CaptureView
{
public:
    static void SendDevicesInfo();

private:
    //Проверяет подходит ли данный формат для транляции, принимает pixelformat и строку
    static bool issupportFmt(unsigned int fmt, string *outName);
    static string driverdir;
};

#endif