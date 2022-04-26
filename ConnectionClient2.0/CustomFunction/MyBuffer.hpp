#ifndef DYNAMICBUFFER_HPP
#define DYNAMICBUFFER_HPP

#include <stdlib.h>

class MyBuffer
{
public:
    MyBuffer(int size, bool autofree = true);
    ~MyBuffer();
    int Size();
    void *Data();

private:
    int size;
    void *data;
    bool af;
};

MyBuffer ::MyBuffer(int size, bool autofree)
{
    af = autofree;
    this->size = size;
    data = malloc(size);
}
MyBuffer ::~MyBuffer()
{
    if (af)
        free(data);
}
int MyBuffer ::Size()
{
    return size;
}
void *MyBuffer ::Data()
{
    return data;
}

#endif