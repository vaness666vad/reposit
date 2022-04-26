#ifndef TERMINALCLIENT_SIGN_HPP
#define TERMINALCLIENT_SIGN_HPP

#include "../../ProtocolCommunication/ClientObject/ClientObject_Sign.hpp"

class TerminalClient : public ClientObject
{
public:
    TerminalClient(int c, char *terguid);
    void AceptMessage(shared_ptr<DataCover128kb> data) override;
    void AfterStopClient() override;
char TerminalGuid[16];

private:
    void ShowPing();
    void sendInfoSes(bool ses);
};

#endif