#ifndef MESSAGECHAT_HPP
#define MESSAGECHAT_HPP

#include <string>
#include <string.h>
#include <cstdlib>
using namespace std;

enum SendrType : char
{
    terminalST = 0,      //Отправитель терминал.
    remote_deviceST = 1, //Отправитель удаленное устройство.
    serverST = 2         //Отправитель сервер.
};
/// Передает сообщение в чат
class MessageChat
{
private:
    int senderId;
    int dataLenght;
    SendrType sendrType;
    char data[1024];

public:
    MessageChat();
    MessageChat(int senderId, SendrType sender, string text);
    int Size();
    string Text();
    SendrType TypeSender();
    int Id();
    void SetData(int senderId, SendrType sender, string text);
};
MessageChat::MessageChat()
{
}
MessageChat::MessageChat(int senderId, SendrType sender, string text)
{
    SetData(senderId, sender, text);
}
int MessageChat::Size()
{
    return 9 + dataLenght;
}
string MessageChat::Text()
{
    return string().append(&data[0], 0, dataLenght);
}
SendrType MessageChat::TypeSender()
{
    return sendrType;
}
int MessageChat::Id()
{
    return senderId;
}
void MessageChat::SetData(int SenderId, SendrType Sender, string Text)
{
    senderId = SenderId;
    sendrType = Sender;
    dataLenght = Text.length();
    memcpy(&data[0], Text.c_str(), dataLenght);
}
#endif