#ifndef CONNECTEDINFO_HPP
#define CONNECTEDINFO_HPP

#include <cstring>

//Предоставляет информацию терминалу и удаленному устройству для создания сессии
class ConnectedInfo
{
private:
    char ip[4];
    int port;
    char pass[16];

public:
    char *Ip();
    int Port();
    char *Pass();
};
char *ConnectedInfo::Ip()
{
    return &ip[0];
}
int ConnectedInfo::Port()
{
    return port;
}
char *ConnectedInfo::Pass()
{
    return &pass[0];
}

#endif