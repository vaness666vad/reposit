#ifndef REGISTRATIONDATA_HPP
#define REGISTRATIONDATA_HPP

#include <string>
#include <string.h>
#include <cstdlib>
using namespace std;

enum ClientType : char
{
    terminal = 0,
    remote_device = 1
};

// служит для передачи регистрационного сообщения
class RegistrationData
{
private:
    int nameLenght;
    int loginLenght;
    int passLenght;
    char clientGuid[16];
    ClientType clientType;
    char data[256];

public:
    RegistrationData(string name, string login, string pass, char *clientG, ClientType type);
    //Возвращает размер сообщения
    int Size();
    void SetData(string name, string login, string pass, char *clientG, ClientType type);
};
RegistrationData::RegistrationData(string name, string login, string pass, char *clientG, ClientType type)
{
    SetData(name, login, pass, clientG, type);
}

int RegistrationData ::Size()
{
    return 29 + nameLenght + loginLenght + passLenght;
}
void RegistrationData ::SetData(string name, string login, string pass, char *clientG, ClientType type)
{
    nameLenght = name.length();
    memcpy(&data[0], name.c_str(), nameLenght);

    loginLenght = login.length();
    memcpy(&data[nameLenght], login.c_str(), loginLenght);

    passLenght = pass.length();
    memcpy(&data[nameLenght + loginLenght], pass.c_str(), passLenght);

    memcpy(&clientGuid[0], &clientG[0], 16);

    clientType = type;
}
#endif