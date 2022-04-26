#ifndef CLIENTOBJECT_HPP
#define CLIENTOBJECT_HPP

#include <arpa/inet.h>
#include <stdio.h>
#include <sys/socket.h>
#include <resolv.h>
#include <string.h>
#include <unistd.h>
#include <list>
#include <memory>
#include <iostream>
#include <chrono>
#include <inttypes.h>
using namespace std::chrono::_V2;
using namespace std;

#include "../Cover.hpp"
#include "../DataCover128kb.hpp"
#include "../Protocol.hpp"
#include "ClientObject_Sign.hpp"
#include "../MessagePack/ValInt32.hpp"

ClientObject::ClientObject(int c)
{
    client = c;
    idOnServer = -1;
    pthread_mutex_init(&writeMutex, NULL);
    pthread_mutex_init(&waitDeleveredMutex, NULL);
    pthread_mutex_init(&isConnectMutex, NULL);
    pthread_mutex_init(&pingMutex, NULL);
    pthread_mutex_init(&idMutex, NULL);

    struct timeval tv;
    tv.tv_sec = 0;
    tv.tv_usec = 0;
    setsockopt(c, SOL_SOCKET, SO_RCVTIMEO, (const char *)&tv, sizeof tv);

    setisConnect(true);

    /*pthread_create(&(thread), NULL, ClientObject::mread, this);
    pthread_detach(thread);*/

    pthread_create(&(pinger), NULL, ClientObject::startPing, this);
    pthread_detach(pinger);

    // mread(this);
}
ClientObject::~ClientObject()
{
    StopClient();
    pthread_mutex_destroy(&writeMutex);
    pthread_mutex_destroy(&waitDeleveredMutex);
    pthread_mutex_destroy(&isConnectMutex);
    pthread_mutex_destroy(&pingMutex);
    pthread_mutex_destroy(&idMutex);
}
void ClientObject::StopClient()
{
    if (setisConnect(false))
    {
        shutdown(client, SHUT_RDWR);

        pthread_mutex_lock(&waitDeleveredMutex);
        _List_iterator<waitd> it = waitDelevered.begin();
        while (it != waitDelevered.end())
        {
            if (it->ARG != NULL)
                free(it->ARG);
            ++it;
        }
        waitDelevered.clear();
        pthread_mutex_unlock(&waitDeleveredMutex);

        AfterStopClient();
    }
}
void ClientObject::AfterStopClient()
{
}
bool ClientObject::IsConnect()
{
    bool res;
    pthread_mutex_lock(&isConnectMutex);
    res = isConnect;
    pthread_mutex_unlock(&isConnectMutex);
    return res;
}
bool ClientObject::Write(DataCover128kb *data, void *(*act)(void *), void *arg)
{
    bool res;
    if (data->DeliveryReport())
        addWaitDelvered(waitd(data->Guid_Ptr(), act, arg));
    res = Write(data, data->DataCoverSize());
    return res;
}
bool ClientObject::Write(const void *__buf, size_t __n)
{
    bool res;
    pthread_mutex_lock(&writeMutex);
    try
    {
        //res = write(client, __buf, __n) == -1 ? false : true;
        res = send(client, __buf, __n, MSG_NOSIGNAL) == -1 ? false : true;
    }
    catch (const char *str)
    {
        fprintf(stderr, "%s\n", str);
    }

    pthread_mutex_unlock(&writeMutex);
    return res;
}
int ClientObject::Ping()
{
    int res;
    pthread_mutex_lock(&pingMutex);
    res = ping;
    pthread_mutex_unlock(&pingMutex);
    return res;
}
bool ClientObject::IsDelevered(char *guid)
{
    bool res = true;
    pthread_mutex_lock(&waitDeleveredMutex);
    _List_iterator<waitd> it = waitDelevered.begin();
    while (it != ClientObject::waitDelevered.end())
    {
        if (0 == memcmp(it->GUID, guid, 16))
        {
            res = false;
            break;
        }
        ++it;
    }
    pthread_mutex_unlock(&waitDeleveredMutex);
    return res;
}
void ClientObject::addWaitDelvered(waitd wd)
{
    pthread_mutex_lock(&waitDeleveredMutex);
    waitDelevered.push_back(wd);
    pthread_mutex_unlock(&waitDeleveredMutex);
}
void ClientObject::setDelvered(char *guid)
{
    pthread_mutex_lock(&waitDeleveredMutex);
    _List_iterator<waitd> it = waitDelevered.begin();
    while (it != waitDelevered.end())
    {
        if (0 == memcmp(it->GUID, guid, 16))
        {
            if (it->ACT != NULL)
            {
                pthread_t t;
                pthread_create(&t, NULL, (void *(*)(void *))it->ACT, it->ARG);
                pthread_detach(t);
            }
            else
            {
                if (it->ARG != NULL)
                    free(it->ARG);
            }
            ClientObject::waitDelevered.erase(it);
            break;
        }
        ++it;
    }
    pthread_mutex_unlock(&waitDeleveredMutex);
}
bool ClientObject::setisConnect(bool connected)
{
    bool res;
    pthread_mutex_lock(&isConnectMutex);
    res = isConnect;
    isConnect = connected;
    pthread_mutex_unlock(&isConnectMutex);
    return res;
}
void *ClientObject::mread(void *arg)
{
    ClientObject *co = (ClientObject *)arg;
    int bordersize = Protocol::BorderSize();
    DataCover128kb dc128memory;
    memset(&dc128memory, '_', bordersize);
    try
    {
        while (co->IsConnect())
        {
            Protocol::ReadStream(co->client, &dc128memory, bordersize - 1, 1);

            if (memcmp(&dc128memory, Protocol::border, bordersize) == 0)
            {
                Protocol::ReadStream(co->client, &dc128memory, bordersize, sizeof(Cover) - bordersize);
                Cover *cov = (Cover *)&dc128memory;
                if (cov->DataSize() <= DataCover128kb::MaxDataSize())
                {
                    Protocol::ReadStream(co->client, dc128memory.Data(), 0, cov->DataSize());

                    if (dc128memory.BufferType() == report)
                    {
                        co->setDelvered((char *)dc128memory.Data());
                    }
                    else
                    {
                        if (dc128memory.DeliveryReport())
                        {
                            DataCover128kb dc8rep = DataCover128kb(dc128memory.Guid_Ptr(), Cover::GuidCoverSize(), report);
                            co->Write(&dc8rep, dc8rep.DataSize() + sizeof(Cover));
                        }
                        pthread_t thr;

                        char *acept = (char *)malloc(sizeof(co) + sizeof(DataCover128kb));
                        memcpy(&acept[0], &co, sizeof(co));
                        memcpy(&acept[sizeof(co)], &dc128memory, sizeof(DataCover128kb));

                        pthread_create(&thr, NULL, co->aceptMessage, acept);
                        pthread_detach(thr);
                    }
                }
                else
                    fprintf(stderr, "%s", "Получен пакет не верного размера\n");
                memset(&dc128memory, '_', bordersize);
            }
            else
            {
                char *adrr = (char *)(&dc128memory);
                memcpy(adrr, &adrr[1], bordersize - 1);
            }
        }
    }
    catch (const char *str)
    {
        fprintf(stderr, "%s\n", str);
    }
    co->StopClient();
    return NULL;
}
void ClientObject::MREAD(bool inNewThread)
{
    if (inNewThread)
    {
        pthread_t mreader;
        pthread_create(&(mreader), NULL, ClientObject::mread, this);
        pthread_detach(thread);
    }
    else
        mread(this);
}
void *ClientObject::aceptMessage(void *arg)
{
    DataCover128kb *dc128 = new DataCover128kb();
    shared_ptr<DataCover128kb> sp = shared_ptr<DataCover128kb>(dc128);
    ClientObject *co;
    memcpy(&co, &((char *)arg)[0], sizeof(co));
    memcpy(dc128, &((char *)arg)[sizeof(co)], sizeof(DataCover128kb));
    free(arg);
    co->AceptMessage(sp);
    return NULL;
}
void ClientObject::AceptMessage(shared_ptr<DataCover128kb> data)
{
    //Виртуальный метод для переопределения
    switch (data->BufferType())
    {
    case assignedID:
        SetId(((ValInt32 *)data->Data())->GetValue());
    default:
        break;
    }
}
void *ClientObject::startPing(void *arg)
{
    ClientObject *co = (ClientObject *)arg;
    while (co->IsConnect())
    {
        char *savaarg = (char *)malloc(sizeof(co) + sizeof(system_clock::time_point));
        memcpy(&savaarg[0], &co, sizeof(co));
        system_clock::time_point tp = system_clock::now();
        memcpy(&savaarg[sizeof(co)], &tp, sizeof(system_clock::time_point));

        DataCover128kb dc128(NULL, 0, pingA, true);

        co->Write(&dc128, ClientObject::SavePing, savaarg);
        this_thread::sleep_for(chrono::milliseconds(5000));
    }
    return NULL;
}
void *ClientObject::SavePing(void *arg)
{
    system_clock::time_point tp = system_clock::now();

    ClientObject *co;
    system_clock::time_point *tps = (system_clock::time_point *)(&((char *)arg)[sizeof(co)]);
    memcpy(&co, &((char *)arg)[0], sizeof(co));

    co->setPing(std::chrono::duration_cast<std::chrono::milliseconds>(tp - *tps).count());

    free(arg);
    return NULL;
}
void ClientObject::setPing(int p)
{
    pthread_mutex_lock(&pingMutex);
    ping = p;
    pthread_mutex_unlock(&pingMutex);
}
int ClientObject::GetId()
{
    int res = -1;
    if (IsConnect())
    {
        pthread_mutex_lock(&idMutex);
        res = idOnServer;
        pthread_mutex_unlock(&idMutex);
    }
    return res;
}
void ClientObject::SetId(int id)
{
    pthread_mutex_lock(&idMutex);
    idOnServer = id;
    pthread_mutex_unlock(&idMutex);
}

#endif