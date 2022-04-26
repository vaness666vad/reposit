#ifndef SERVERCLIENT_HPP
#define SERVERCLIENT_HPP

#include <memory>

#include "../../ProtocolCommunication/DataCover128kb.hpp"
#include "../../ProtocolCommunication/DataTypeEnum.h"
#include "../../ProtocolCommunication/MessagePack/SessionMess/ConnectedInfo.hpp"
#include "../../ProtocolCommunication/MessagePack/SessionMess/TerminalConnectData.hpp"
#include "../../ProtocolCommunication/MessagePack/MessageChat.hpp"
#include "../../ProtocolCommunication/MessagePack/ValInt32.hpp"
#include "../TerminalClient/TerminalClient_Sign.hpp"
#include "../TcpConnection.hpp"
#include "../ConnectProcedure.hpp"
#include "ServerClient_Sign.hpp"

ServerClient::ServerClient(int c, string remName) : ClientObject(c)
{
    remDevName = remName;
    pthread_mutex_init(&sessionMutex, NULL);

    if (TcpConnection::TC->TerminalSession != NULL && TcpConnection::TC->TerminalSession->IsConnect())
    {
        TerminalConnectData tcd(&TcpConnection::TC->TerminalSession->TerminalGuid[0], &TcpConnection::TC->ClientGuid[0], true);
        DataCover128kb dc128(&tcd, tcd.Size(), sessionInfo);
        Write(&dc128, dc128.DataCoverSize());
    }
}
ServerClient::~ServerClient()
{
    pthread_mutex_destroy(&sessionMutex);
}
void ServerClient::AfterStopClient()
{
    fprintf(stderr, "%s", "Соединение с сервером прервано\n");
}
void ServerClient::AceptMessage(shared_ptr<DataCover128kb> data)
{
    ::ClientObject::AceptMessage(data);
    //команды полученые от сервера выполняются тут
    switch (data->BufferType())
    {
    case pingA:
        ShowPing();
        break;
    case createSessionIpPort:
        TryConnectToTermenal(data);
        break;
    default:
        break;
    }
}
void ServerClient::ShowPing()
{
    fprintf(stderr, "Сервер пинг %dms\n", Ping());
}
void ServerClient::TryConnectToTermenal(shared_ptr<DataCover128kb> data)
{
    if (pthread_mutex_trylock(&sessionMutex) == 0)
    {
        try
        {
            if (TcpConnection::TC->TerminalSession == NULL || !TcpConnection::TC->TerminalSession->IsConnect())
            {
                ConnectedInfo *ci = (ConnectedInfo *)(data->Data());
                int fd;
                char Terminalguid[16];
                

                struct sockaddr_in my_addr = {0};
                socklen_t len =sizeof(sockaddr);
                getsockname(client, (sockaddr*)&my_addr, &len);
                char myIP[16];
                inet_ntop(AF_INET, &my_addr.sin_addr, myIP, sizeof(myIP));
                u_int16_t myPort = ntohs(my_addr.sin_port);

                if ((fd = ConnectProcedure::ConnectToTerminal(ci->Ip(), ci->Port(), TcpConnection::ClientGuid, ci->Pass(), remDevName, &my_addr, &Terminalguid[0])) != -1)
                {
                    if (TcpConnection::TC->TerminalSession != NULL)
                        delete TcpConnection::TC->TerminalSession;

                    TcpConnection::TC->TerminalSession = new TerminalClient(fd, &Terminalguid[0]);
                    TcpConnection::TC->TerminalSession->MREAD(true);
                }
            }
            else
            {
                string mes = "Отказано в доступе, соединение уже установлено с другим терминалом";
                fprintf(stderr, "%s\n", mes.c_str());
                MessageChat mc(GetId(), remote_deviceST, mes);
                DataCover128kb dc128(&mc, mc.Size(), messageChat);
                Write(&dc128, dc128.DataCoverSize());
            }
        }
        catch (const char *str)
        {
            fprintf(stderr, "%s\n", str);
        }
        pthread_mutex_unlock(&sessionMutex);
    }
    else
    {
        string mes = "Отказано в доступе, в данный момент выполняется попытка подключения к терминалу";
        fprintf(stderr, "%s\n", mes.c_str());
        MessageChat mc(GetId(), remote_deviceST, mes);
        DataCover128kb dc128(&mc, mc.Size(), messageChat);
        Write(&dc128, dc128.DataCoverSize());
    }
}

#endif