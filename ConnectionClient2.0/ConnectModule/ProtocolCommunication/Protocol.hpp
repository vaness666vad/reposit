#ifndef PROTOCOL_HPP
#define PROTOCOL_HPP

#include <arpa/inet.h>
#include <stdio.h>
#include <sys/socket.h>
#include <resolv.h>
#include <string.h>
#include <unistd.h>
#include <iostream>
#include <exception>
using namespace std;

#include "../../CustomFunction/MyBuffer.hpp"

class Protocol
{
public:
    //унифицированая функция чтения сокета tcp клиента
    static int ReadStream(int cl_nt, void *bufer, int offset, int size, int trycount = 10);
    //вернет границу сообщения
    static MyBuffer *GetBorder();
    static char border[6];
    static int BorderSize();
};
char Protocol::border[6] = {'\r', '\n', '\r', '\n', '\r', '\n'};
MyBuffer *Protocol::GetBorder()
{
    int size = sizeof(Protocol::border);
    MyBuffer *mb = new MyBuffer(size, false);
    memcpy(mb->Data(), &Protocol::border, size);
    return mb;
}
int Protocol::BorderSize()
{
    return sizeof(Protocol::border);
}
int Protocol::ReadStream(int cl_nt, void *bufer, int offset, int lenght, int trycount)
{
    int readLenght = 0;
    int bytes;
    int trypos = -1;
    if (lenght > 0)
    {
        do
        {
            char *c = (char *)bufer;
            bytes = read(cl_nt, &c[offset + readLenght], lenght - readLenght);
            if (bytes > 0)
            {
                readLenght += bytes;
                trypos = -1;
            }
            else
            {
                trypos += 1;
            }
        } while (trypos < trycount && readLenght < lenght);

        if (lenght != 0 && trypos >= trycount)
        {
            char mess[120];
            sprintf(mess, "Принят пустой буфер %d раз подряд. Ошибка чтения данных пакета%c", trycount, '\0');
            throw mess;
        }
    }

    return readLenght;
}
#endif