#ifndef DEVFORMATSIZE_HPP
#define DEVFORMATSIZE_HPP

#include <string>
#include <string.h>
#include <cstdlib>
using namespace std;

// служит для передачи сообщения о поддерживаемых разрешениях формата сжатия камеры
class DevFormatSize
{
private:
    int idlenght;
    int indexfmt;
    int widht;
    int height;
    char data[256];

public:
    DevFormatSize(string id, int fmt, int w, int h);
    //Возвращает размер сообщения
    int Size();
    void SetData(string id, int fmt, int w, int h);
};
DevFormatSize::DevFormatSize(string id, int fmt, int w, int h)
{
    SetData(id, fmt, w, h);
}

int DevFormatSize ::Size()
{
    return 16 + idlenght;
}
void DevFormatSize ::SetData(string id, int fmt, int w, int h)
{
    idlenght = id.length();
    memcpy(&data[0], id.c_str(), idlenght);

    indexfmt = fmt;
    widht = w;
    height = h;
}
#endif