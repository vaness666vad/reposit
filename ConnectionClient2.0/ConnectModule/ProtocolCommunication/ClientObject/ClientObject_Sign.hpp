#ifndef CLIENTOBJECT_SIGN_HPP
#define CLIENTOBJECT_SIGN_HPP

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
using namespace std;

#include "../Cover.hpp"
#include "../DataCover128kb.hpp"
#include "../Protocol.hpp"

class waitd
{
public:
    waitd(char *guid, void *(*act)(void *), void *arg)
    {
        memcpy(&GUID[0], guid, Cover::GuidCoverSize());
        ACT = act;
        ARG = arg;
    }
    char GUID[16];
    void *(*ACT)(void *);
    void *ARG;
};
class ClientObject
{
public:
    ClientObject(int c);
    void StopClient();
    virtual void AfterStopClient();
    ~ClientObject();
    bool IsConnect();
    bool Write(DataCover128kb *data, void *(*act)(void *) = NULL, void *arg = NULL);
    bool Write(const void *__buf, size_t __n);
    int Ping();
    bool IsDelevered(char *guid);
    //Виртуальный метод для переопределения
    virtual void AceptMessage(shared_ptr<DataCover128kb> data);
    void MREAD(bool inNewThread);
    int GetId();
protected:
    int client;

private:

    pthread_mutex_t writeMutex;
    pthread_mutex_t waitDeleveredMutex;
    pthread_mutex_t isConnectMutex;
    pthread_mutex_t pingMutex;
    pthread_t thread;
    pthread_t pinger;
    list<waitd> waitDelevered;
    void addWaitDelvered(waitd wd);
    void setDelvered(char *guid);
    bool isConnect;
    bool setisConnect(bool connected);
    static void *mread(void *arg);
    static void *aceptMessage(void *arg);
    static void *startPing(void *arg);
    static void *SavePing(void *arg);
    int ping;
    void setPing(int p);
    int idOnServer;
    pthread_mutex_t idMutex;
    void SetId(int id);
};

#endif