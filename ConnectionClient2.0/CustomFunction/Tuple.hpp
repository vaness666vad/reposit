#ifndef TUPLE_HPP
#define TUPLE_HPP

template <typename T1, typename T2>
class Tuple2
{
public:
    Tuple2(T1 item1, T2 item2);
    T1 *Item1();
    T2 *Item2();

private:
    T1 item1;
    T2 item2;
};

template <typename T1, typename T2>
Tuple2<T1, T2>::Tuple2(T1 item1, T2 item2)
{
    this->item1 = item1;
    this->item2 = item2;
}
template <typename T1, typename T2>
T1 *Tuple2<T1, T2>::Item1()
{
    return item1;
}
template <typename T1, typename T2>
T2 *Tuple2<T1, T2>::Item2()
{
    return item2;
}

template <typename T1, typename T2, typename T3>
class Tuple3 : public Tuple2<T1, T2>
{
public:
    Tuple3(T1 item1, T2 item2, T3 item3);
    T3 *Item3();

private:
    T3 item3;
};
template <typename T1, typename T2, typename T3>
Tuple3<T1, T2, T3>::Tuple3(T1 item1, T2 item2, T3 item3) : Tuple2<T1, T2>(item1, item2)
{
    this->item3 = item3;
}
template <typename T1, typename T2, typename T3>
T3 *Tuple3<T1, T2, T3>::Item3()
{
    return item3;
}

#endif