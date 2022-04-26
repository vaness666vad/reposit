#ifndef TCPCONNECTION_HPP
#define TCPCONNECTION_HPP

#include "ServerClient/ServerClient_Sign.hpp"
#include "TerminalClient/TerminalClient_Sign.hpp"

class TcpConnection
{
public:
    TcpConnection();
    ~TcpConnection();
    ServerClient *ServerSession;
    TerminalClient *TerminalSession;
    static TcpConnection *TC;
    static char *ClientGuid;
};
TcpConnection *TcpConnection::TC = NULL;
char *TcpConnection::ClientGuid = NULL;
TcpConnection::TcpConnection()
{
    ClientGuid = new char[16];
    int i;
    i = rand();
    memcpy(&ClientGuid[0], &i, 4);
    i = rand();
    memcpy(&ClientGuid[4], &i, 4);
    i = rand();
    memcpy(&ClientGuid[8], &i, 4);
    i = rand();
    memcpy(&ClientGuid[12], &i, 4);

    ServerSession = NULL;
    TerminalSession = NULL;
    TC = this;
}
TcpConnection::~TcpConnection()
{
    if (ServerSession != NULL)
        delete ServerSession;
    if (TerminalSession != NULL)
        delete TerminalSession;
    delete ClientGuid;
}

#endif