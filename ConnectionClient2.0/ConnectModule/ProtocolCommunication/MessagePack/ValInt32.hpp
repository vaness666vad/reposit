#ifndef VALINT32_HPP
#define VALINT32_HPP

class ValInt32
{
private:
    int val;

public:
    ValInt32(int value);
    //Возвращает размер сообщения
    int Size();
    void SetData(int Value);
    int GetValue();
};
int ValInt32::GetValue()
{
    return val;
}
ValInt32::ValInt32(int value)
{
    SetData(value);
}
void ValInt32::SetData(int Value)
{
    val = Value;
}
int Size()
{
    return 4;
}

#endif