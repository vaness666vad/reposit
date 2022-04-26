#ifndef CONNECTPROCEDURE_HPP
#define CONNECTPROCEDURE_HPP

#include <string>
#include <string.h>
#include <cstdlib>
#include <iostream>
#include <arpa/inet.h>
#include <stdio.h>
#include <sys/socket.h>
#include <resolv.h>
#include <unistd.h>
#include <memory>
#include <fcntl.h>
#include <sys/file.h>
using namespace std;

#include "TcpConnection.hpp"
#include "../ProtocolCommunication/Protocol.hpp"
#include "../ProtocolCommunication/Cover.hpp"
#include "../ProtocolCommunication/DataCover128kb.hpp"
#include "../ProtocolCommunication/DataTypeEnum.h"
#include "../ProtocolCommunication/MessagePack/RegistrationData.hpp"
#include "../ProtocolCommunication/MessagePack/SessionMess/TerminalRegRemDev.hpp"

class ConnectProcedure
{
private:
    //Метод авторизации на сервере
    static void authorizationOnServer(int fd, string login, string pass, string identifiator);
    //Метод авторизации с терминалом
    static void authorizationOnTerminal(int fd, char *devGuid, char *pass, string identifiator, char *outTermonalGuid);
    //Возаращает id сокета при успешном подключении
    static int connectSocket(char hostname[4], int port);
    //Возвращает id сокета при успешном асинхронном подключении
    static int connectAsync(char hostname[4], int port, sockaddr_in* endPoint);

public:
    static int ConnectToServer(char hostname[4], int port, string login, string pass, string identifiator);
    static int ConnectToTerminal(char hostname[4], int port, char *devGuid, char *pass, string identifiator, sockaddr_in* endPoint, char *outTermonalGuid);
};
int ConnectProcedure ::ConnectToTerminal(char hostname[4], int port, char *devGuid, char *pass, string identifiator, sockaddr_in* endPoint, char *outTermonalGuid)
{
    fprintf(stderr, "Процесс создания сессия с терминалом...\n");
    int soc = ConnectProcedure::connectAsync(hostname, port, endPoint);
    ConnectProcedure::authorizationOnTerminal(soc, devGuid, pass, identifiator, outTermonalGuid);
    fprintf(stderr, "Регистрация подтверждена, подключение к терминалу успешно установлено\n");
    return soc;
}
int ConnectProcedure ::connectAsync(char hostname[4], int port, sockaddr_in* endPoint)
{
    fprintf(stderr, "Процесс подключения сокета к терминалу...\n");
    fprintf(stderr, "%d.%d.%d.%d:%d\n", hostname[0], hostname[1], hostname[2], hostname[3], port);
    int fd = socket(AF_INET, SOCK_STREAM, 0);
    if (fd == -1)
        throw "Не удалось открыть сокет";

    int ival = 1;
    setsockopt(fd, SOL_SOCKET, SO_REUSEADDR, &ival, sizeof(ival));
    //fcntl(fd, F_SETFL, O_NONBLOCK);
    socklen_t len =sizeof(sockaddr);
    bind(fd, (struct sockaddr *)endPoint, len);

    struct sockaddr_in adr = {0};
    adr.sin_family = AF_INET;
    adr.sin_port = htons(port);
    adr.sin_addr.s_addr = *(int*)hostname;
    #char ip[16];
    #sprintf(ip, "%d.%d.%d.%d%c", hostname[0], hostname[1], hostname[2], hostname[3], '\0');
    #inet_pton(AF_INET, ip, &adr.sin_addr);

     //signal(SIGIO, hsig);
fcntl(fd, F_SETFL, fcntl(fd, F_GETFL) | O_ASYNC);
//fcntl(fd, F_SETOWN, getpid());

    int isConnect = connect(fd, (struct sockaddr *)&adr, sizeof adr);
    if (isConnect == -1)
        throw "Не удалось установить соединение сокета";

    fprintf(stderr, "Соединение сокетов установлено\n");

    return fd;
}

int ConnectProcedure ::ConnectToServer(char hostname[4], int port, string login, string pass, string identifiator)
{
    fprintf(stderr, "Процесс подключения и регистрации удаленного устройства на сервере...\n");
    int soc = ConnectProcedure::connectSocket(hostname, port);
    ConnectProcedure::authorizationOnServer(soc, login, pass, identifiator);
    fprintf(stderr, "Регистрация подтверждена, подключение к серверу успешно установлено\n");
    return soc;
}

int ConnectProcedure ::connectSocket(char hostname[4], int port)
{
    fprintf(stderr, "Процесс подключения сокета к серверу...\n");
    fprintf(stderr, "%d.%d.%d.%d:%d\n", hostname[0], hostname[1], hostname[2], hostname[3], port);
    int fd = socket(AF_INET, SOCK_STREAM, 0);
    if (fd == -1)
        throw "Не удалось открыть сокет";
    int ival = 1;
    setsockopt(fd, SOL_SOCKET, SO_REUSEADDR, &ival, sizeof(ival));

    struct sockaddr_in adr = {0};
    adr.sin_family = AF_INET;
    adr.sin_port = htons(port);
    adr.sin_addr.s_addr = *(int*)hostname;
    //char ip[16];
    //sprintf(ip, "%d.%d.%d.%d%c", hostname[0], hostname[1], hostname[2], hostname[3], '\0');
   //net_pton(AF_INET, ip, &adr.sin_addr);
    int isConnect = connect(fd, (struct sockaddr *)&adr, sizeof adr);
    if (isConnect == -1)
        throw "Не удалось установить соединение сокета";

    fprintf(stderr, "Соединение сокетов установлено\n");

    return fd;
}
void ConnectProcedure ::authorizationOnTerminal(int fd, char *devGuid, char *pass, string identifiator, char *outTermonalGuid)
{
    fprintf(stderr, "Процесс авторизации...\n");

    struct timeval tv;
    tv.tv_sec = 3;
    tv.tv_usec = 0;
    setsockopt(fd, SOL_SOCKET, SO_RCVTIMEO, (const char *)&tv, sizeof tv);

    Cover cov;
    int covsize = sizeof(Cover);
    if (Protocol::ReadStream(fd, &cov, 0, covsize) != covsize || cov.BufferType() != registrationRequist)
        throw "Получен не верный ответ";

    TerminalRegRemDev trrd;
    if (Protocol::ReadStream(fd, &trrd, 0, cov.DataSize()) != cov.DataSize())
        throw "Ошибка приема пакета";
 
    if (memcmp(trrd.GetPass(), pass, 16) != 0)
        throw "регистрационные данные не верны";
    memcpy(outTermonalGuid, trrd.GetSenderGuid(), 16);

    TerminalRegRemDev trrds(devGuid, pass, identifiator);
    DataCover128kb sendD(&trrds, trrds.Size(), registrationComlited);
    write(fd, &sendD, sendD.DataCoverSize());

    fprintf(stderr, "Процесс авторизации завершен\n");
}
void ConnectProcedure ::authorizationOnServer(int fd, string login, string pass, string identifiator)
{
    fprintf(stderr, "Процесс авторизации...\n");

    struct timeval tv;
    tv.tv_sec = 3;
    tv.tv_usec = 0;
    setsockopt(fd, SOL_SOCKET, SO_RCVTIMEO, (const char *)&tv, sizeof tv);
    RegistrationData rd(identifiator, login, pass, TcpConnection::ClientGuid, remote_device);
    DataCover128kb dc128(&rd, rd.Size(), registrationRequist);
    write(fd, &dc128, dc128.DataCoverSize());

    Cover answer;
    int covsize = sizeof(Cover);
    if (Protocol::ReadStream(fd, &answer, 0, covsize) != covsize)
        throw "Получен не верный ответ";

    if (answer.BufferType() != registrationComlited)
        throw "Удаленный хост отклонил запрос авторизации";

    fprintf(stderr, "Процесс авторизации завершен\n");
}
#endif
