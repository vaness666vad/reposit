#ifndef SUPPORTEDFORMAT_HPP
#define SUPPORTEDFORMAT_HPP

#include <string>
#include <string.h>
#include <cstdlib>
using namespace std;

// служит для передачи сообщения о поддерживаемом формате сжатия камеры
class SupportedFormat
{
private:
    int idLenght;
    int indexfmt;
    int nameLenght;
    char data[256];

public:
    SupportedFormat(string id, int fmt, string name);
    //Возвращает размер сообщения
    int Size();
    void SetData(string id, int fmt, string name);
};
SupportedFormat::SupportedFormat(string id, int fmt, string name)
{
    SetData(id, fmt, name);
}

int SupportedFormat ::Size()
{
    return 12 + nameLenght + idLenght;
}
void SupportedFormat ::SetData(string id, int fmt, string name)
{
    indexfmt = fmt;

    idLenght = id.length();
    memcpy(&data[0], id.c_str(), idLenght);

    nameLenght = name.length();
    memcpy(&data[idLenght], name.c_str(), nameLenght);
}
#endif