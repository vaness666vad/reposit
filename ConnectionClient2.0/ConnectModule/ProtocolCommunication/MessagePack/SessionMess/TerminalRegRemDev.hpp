#ifndef TERMINALREGREMDEV_HPP
#define TERMINALREGREMDEV_HPP

#include <string>
#include <string.h>
#include <cstdlib>
using namespace std;

// Предоставляет регистрационные данные для терминала к удаленному устройству и обратно
class TerminalRegRemDev
{
private:
    char senderGuid[16];
    char pass[16];
    int nameLeng;
    char senderName[128];

public:
    TerminalRegRemDev();
    TerminalRegRemDev(char *senderG, char *pass, string name);
    //Возвращает размер сообщения
    char *GetPass();
    char *GetSenderGuid();
    string SenderName();
    int Size();
    void SetData(char *senderG, char *pass, string name);
};
string TerminalRegRemDev::SenderName()
{
    return string().append(&senderName[0], 0, nameLeng);
}
char *TerminalRegRemDev::GetPass()
{
    return pass;
}
char *TerminalRegRemDev::GetSenderGuid()
{
    return senderGuid;
}
TerminalRegRemDev::TerminalRegRemDev(char *senderG, char *pass, string name)
{
    SetData(senderG, pass, name);
}
TerminalRegRemDev::TerminalRegRemDev()
{
}
int TerminalRegRemDev ::Size()
{
    return 36 + nameLeng;
}
void TerminalRegRemDev ::SetData(char *senderG, char *passs, string name)
{
    memcpy(&senderGuid[0], senderG, 16);
    memcpy(&pass[0], passs, 16);
    nameLeng = name.length();
    memcpy(&senderName[0], name.c_str(), nameLeng);
}
#endif