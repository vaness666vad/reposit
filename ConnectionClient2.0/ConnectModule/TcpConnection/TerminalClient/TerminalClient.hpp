#ifndef TERMINALCLIENT_HPP
#define TERMINALCLIENT_HPP

#include <memory>

#include "../../ProtocolCommunication/DataTypeEnum.h"
#include "../../ProtocolCommunication/DataCover128kb.hpp"
#include "../../ProtocolCommunication/MessagePack/ValInt32.hpp"
#include "../../ProtocolCommunication/MessagePack/SessionMess/TerminalConnectData.hpp"
#include "TerminalClient_Sign.hpp"

TerminalClient::TerminalClient(int c, char *terguid) : ClientObject(c)
{
    memcpy(&TerminalGuid[0], terguid, 16);
    sendInfoSes(true);
}
void TerminalClient::sendInfoSes(bool ses)
{
    if (TcpConnection::TC->ServerSession != NULL && TcpConnection::TC->ServerSession->IsConnect())
    {
        TerminalConnectData tcd(&TerminalGuid[0], &TcpConnection::TC->ClientGuid[0], ses);
        DataCover128kb dc128(&tcd, tcd.Size(), sessionInfo);
        TcpConnection::TC->ServerSession->Write(&dc128, dc128.DataCoverSize());
    }
}
void TerminalClient::AfterStopClient()
{
    sendInfoSes(false);
    fprintf(stderr, "%s", "Соединение с терминалом прервано\n");
}
void TerminalClient::AceptMessage(shared_ptr<DataCover128kb> data)
{
    ::ClientObject::AceptMessage(data);
    //команды полученые от сервера выполняются тут
    switch (data->BufferType())
    {
    case pingA:
        ShowPing();
        break;
    default:
        break;
    }
}
void TerminalClient::ShowPing()
{
    fprintf(stderr, "Терминал пинг %dms\n", Ping());
}

#endif