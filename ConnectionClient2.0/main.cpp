#include <stdio.h>
#include <string>
#include <string.h>
#include <cstdlib>
#include <iostream>
#include <memory>
#include <thread>
#include <unistd.h>
#include <list>
using namespace std;

//Заголовки смешанных классов
#include "CaptureView/CaptureView.h"
#include "ConnectModule/ProtocolCommunication/ClientObject/ClientObject.h"
#include "ConnectModule/TcpConnection/ServerClient/ServerClient.h"
#include "ConnectModule/TcpConnection/TerminalClient/TerminalClient.h"
//Заголовки смешанных классов

//Вспомогательные классы
#include "ConnectModule/ProtocolCommunication/Protocol.hpp"
#include "Config/ConfigInfo.hpp"
#include "ConnectModule/MailOptions.hpp"
#include "ConnectModule/TcpConnection/ConnectProcedure.hpp"
#include "ConnectModule/TcpConnection/TcpConnection.hpp"
//Вспомогательные классы

//Сигнатуры смешанных классов
#include "CaptureView/CaptureView_Sign.hpp"
#include "ConnectModule/ProtocolCommunication/ClientObject/ClientObject_Sign.hpp"
#include "ConnectModule/TcpConnection/ServerClient/ServerClient_Sign.hpp"
#include "ConnectModule/TcpConnection/TerminalClient/TerminalClient_Sign.hpp"
//Сигнатуры смешанных классов

//Методы смешанных классов
#include "CaptureView/CaptureView.hpp"
#include "ConnectModule/ProtocolCommunication/ClientObject/ClientObject.hpp"
#include "ConnectModule/TcpConnection/ServerClient/ServerClient.hpp"
#include "ConnectModule/TcpConnection/TerminalClient/TerminalClient.hpp"
//Методы смешанных классов

int main()
{
   TcpConnection TC;
   Config conf = ConfigInfo::GetConf();
   while (true)
   {
      try
      {
         IpPortData ippd = MailOptions::ReadServerIpPortFromMail(conf.hostname, atoi(conf.port.c_str()), conf.login, conf.password);
         int fd = ConnectProcedure::ConnectToServer(ippd.Ip, ippd.Port, conf.login, conf.password, conf.identificator);

         if (TC.ServerSession != NULL)
            delete TC.ServerSession;
         TC.ServerSession = new ServerClient(fd, conf.identificator);
         TC.ServerSession->MREAD(false);
      }
      catch (const char *str)
      {
         fprintf(stderr, "%s\n", str);
      }
   }
}
