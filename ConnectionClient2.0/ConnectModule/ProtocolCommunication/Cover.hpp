#ifndef COVER_HPP
#define COVER_HPP

#include "DataTypeEnum.h"
#include "Protocol.hpp"
#include "../../CustomFunction/MyBuffer.hpp"

class Cover
{
public:
    Cover();
 
    static int GuidCoverSize();
    // Возвращает уникальный guid конверта
    char *Guid_Ptr();
    // Возвращает размер содержимого конверта
    int DataSize();
    // Возвращает тип содержимого конверта
    DataType BufferType();
    // Возвращает запрос очета о доставке
    bool DeliveryReport();

protected:
    // Граница обозначающая начало конверта
    char border[6];
    // Уникальный guid конверта
    char guidCover[16];
    // Требует ли отправитель от получателя подтверждения о доставке этого конверта
    char deliveryReport;
    // Тип содержимого конверта
    DataType dataType;
    // Размер конверта
    int dataSize;
};

Cover ::Cover()
{
    memcpy(&border, Protocol::border, sizeof(border));
}

int Cover ::GuidCoverSize()
{
    return sizeof(Cover().guidCover);
}
char *Cover ::Guid_Ptr()
{
    return guidCover;
}

int Cover ::DataSize()
{
    return dataSize;
}

DataType Cover ::BufferType()
{
    return dataType;
}

bool Cover ::DeliveryReport()
{
    return deliveryReport == 1 ? true : false;
}

#endif