#ifndef DATACOVER128KB_HPP
#define DATACOVER128KB_HPP

#include <cstring>

#include "../../CustomFunction/MyBuffer.hpp"
#include "Cover.hpp"

class DataCover128kb : public Cover
{
public:
    DataCover128kb();
    DataCover128kb(void *mem, int lenght, DataType type, bool report = false);
    // Возвращает содержимое конверта
    void *Data();
    // Задает содержимое конверту
    void SetData(void *mem, int lenght, DataType type, bool report = false);
    // Возвращает размер упакованного сообщения
    int DataCoverSize();
    static int MaxDataSize();

private:
    // содержимое сообщения
    char data[131072];
    void generateGuid();
};
void DataCover128kb::generateGuid()
{
    int i;
    i = rand();
    memcpy(&guidCover[0], &i, 4);
    i = rand();
    memcpy(&guidCover[4], &i, 4);
    i = rand();
    memcpy(&guidCover[8], &i, 4);
    i = rand();
    memcpy(&guidCover[12], &i, 4);
}
int DataCover128kb ::MaxDataSize()
{
    return sizeof(data);
}
DataCover128kb ::DataCover128kb()
{
    generateGuid();
    dataSize = 0;
}
DataCover128kb ::DataCover128kb(void *mem, int lenght, DataType type, bool report)
{
    generateGuid();
    dataSize = 0;
    SetData(mem, lenght, type, report);
}

void *DataCover128kb ::Data()
{
    return &data;
}

void DataCover128kb ::SetData(void *mem, int lenght, DataType type, bool report)
{
    int maxSize = sizeof(data);
    int bufsize = lenght > maxSize ? maxSize : lenght;
    memcpy(&data[0], mem, bufsize);
    dataType = type;
    dataSize = bufsize;
    deliveryReport = report ? 1 : 0;
}

int DataCover128kb ::DataCoverSize()
{
    return sizeof(DataCover128kb) - sizeof(data) + dataSize;
}

#endif