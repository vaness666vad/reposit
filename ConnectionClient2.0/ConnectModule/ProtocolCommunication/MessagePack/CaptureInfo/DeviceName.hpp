#ifndef DEVICENAME_HPP
#define DEVICENAME_HPP

#include <string>
#include <string.h>
#include <cstdlib>
using namespace std;

// служит для передачи сообщения о существующей камере
class DeviceName
{
private:
    int idLenght;
    int nameLenght;
    char data[256];

public:
    DeviceName(string id, string name);
    //Возвращает размер сообщения
    int Size();
    void SetData(string id, string name);
};
DeviceName::DeviceName(string id, string name)
{
    SetData(id, name);
}

int DeviceName ::Size()
{
    return 8 + nameLenght + idLenght;
}
void DeviceName ::SetData(string id, string name)
{
    idLenght = id.length();
    memcpy(&data[0], id.c_str(), idLenght);

    nameLenght = name.length();
    memcpy(&data[idLenght], name.c_str(), nameLenght);
}
#endif