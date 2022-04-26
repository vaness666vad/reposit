#ifndef TERMINALCONNECTDATA_HPP
#define TERMINALCONNECTDATA_HPP

#include <string>
#include <string.h>
#include <cstdlib>
using namespace std;

// Предоставляет информацию для сервера о созданной/текущей сессии 
class TerminalConnectData
{
private:
    char terminalGuid[16];
    char remDevGuid[16];
    char isEnable;

public:
    TerminalConnectData(char *terminalG, char *remDevG, bool isenable);
    //Возвращает размер сообщения
    int Size();
    void SetData(char *terminalG, char *remDevG, bool Isenable);
};
TerminalConnectData::TerminalConnectData(char *terminalG, char *remDevG, bool isenable)
{
    SetData(terminalG, remDevG, isenable);
}

int TerminalConnectData ::Size()
{
    return 33;
}
void TerminalConnectData ::SetData(char *terminalG, char *remDevG, bool Isenable)
{
    isEnable = Isenable? 1 : 0;
    memcpy(&terminalGuid[0], terminalG, 16);
    memcpy(&remDevGuid[0], remDevG, 16);
}
#endif