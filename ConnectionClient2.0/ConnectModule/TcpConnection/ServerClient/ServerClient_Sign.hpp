#ifndef SERVERCLIENT_SIGN_HPP
#define SERVERCLIENT_SIGN_HPP

#include "../../ProtocolCommunication/ClientObject/ClientObject_Sign.hpp"

class ServerClient : public ClientObject
{
public:
    ServerClient(int c, string remName);
    ~ServerClient();
    void AceptMessage(shared_ptr<DataCover128kb> data) override;
    void AfterStopClient() override;

private:
    string remDevName;
    pthread_mutex_t sessionMutex;
    void ShowPing();
    void TryConnectToTermenal(shared_ptr<DataCover128kb> data);
};

#endif