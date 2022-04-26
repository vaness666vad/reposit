#ifndef MAILOPTIONS_HPP
#define MAILOPTIONS_HPP

#include <string>
#include <string.h>
#include <cstdlib>
#include <netdb.h>
#include <unistd.h>
#include <openssl/ssl.h>
using namespace std;

class IpPortData
{
public:
    char Ip[4];
    int Port;
};

class MailOptions
{
public:
    static IpPortData ReadServerIpPortFromMail(string hostname, int port, string login, string pass);

private:
    static int Read(SSL *ssl, char *data);
    static void decodeBuf(char *input24, char *output8);
};
// Почтовый сервер gmail перед 10 добавляет 13, после 13 10. Надо декодировать сообщение в 0-9 байт
void MailOptions ::decodeBuf(char *input24, char *output8)
{
    char bufer[24];
    int count = 0;
    for (int i = 0; i < 24; i += 1)
    {
        if (input24[i] > 47 && input24[i] < 58)
        {
            bufer[count] = input24[i];
            count += 1;
        }
    }

    for (int i = 0; i < 8; i += 1)
    {
        output8[i] = (char)(((input24[i * 3 + 0] - 48) * 100) + ((input24[i * 3 + 1] - 48) * 10) + input24[i * 3 + 2] - 48);
    }
}

IpPortData MailOptions ::ReadServerIpPortFromMail(string hostname, int port, string login, string pass)
{
    fprintf(stderr, "Чтение данных о сервере с почтового сервера %s\n", login.c_str());
    IpPortData ippd = {0};
    SSL_CTX *ctx;
    int server;
    SSL *ssl;

    SSL_library_init();
    OpenSSL_add_all_algorithms();
    SSL_load_error_strings();
    ctx = SSL_CTX_new(TLSv1_2_client_method());

    if (ctx == NULL)
        throw "Не удалось сгенерировать SSL ключ";
    struct hostent *host;
    struct sockaddr_in addr;
    if ((host = gethostbyname(hostname.c_str())) == NULL)
        throw "Не верный hostname";

    server = socket(PF_INET, SOCK_STREAM, 0);
    memset(&addr, '_', sizeof(addr));
    addr.sin_family = AF_INET;
    addr.sin_port = htons(port);
    addr.sin_addr.s_addr = *(long *)(host->h_addr);
    if (connect(server, (struct sockaddr *)&addr, sizeof(addr)) != 0)
    {
        close(server);
        throw "Не удалось установить подключение";
    }
    if (server == -1)
        throw "Не удалось открыть соединение с почтовым сервером";

    ssl = SSL_new(ctx);
    SSL_set_fd(ssl, server);
    if (SSL_connect(ssl) == 0)
        throw "Не удалось инициировать защищенное соединение";

    X509 *cert;
    char *line;
    cert = SSL_get_peer_certificate(ssl); /* get the server's certificate */
    if (cert == NULL)
        throw "Не удалось обменяться ключами";

    line = X509_NAME_oneline(X509_get_subject_name(cert), 0, 0);
    free(line); /* free the malloc'ed string */
    line = X509_NAME_oneline(X509_get_issuer_name(cert), 0, 0);
    free(line);      /* free the malloc'ed string */
    X509_free(cert); /* free the malloc'ed certificate copy */

    struct timeval tv;
    tv.tv_sec = 2;
    tv.tv_usec = 0;
    setsockopt(server, SOL_SOCKET, SO_RCVTIMEO, (const char *)&tv, sizeof tv);

    string message = "A01 LOGIN " + login + ' ' + pass + "\r\nA02 SELECT INBOX\r\n";
    SSL_write(ssl, message.c_str(), message.length());

    char data[1024];
    int datalenght = Read(ssl, &data[0]);

    if (string(data).find("[AUTHENTICATIONFAILED]", 0) != string::npos)
        throw "[AUTHENTICATIONFAILED]";

    string strEXISTS(data);
    int index = strEXISTS.find("EXISTS");
    if (index == string::npos)
        throw "Получен не верный ответ от сервера";

    while (data[index] != '\n')
        index -= 1;
    index += 1;
    string coun;
    for (int i = index; i < datalenght; i += 1)
    {
        if (data[i] < 59 && data[i] > 47)
            coun.push_back(data[i]);
        if (data[i] == '\n')
            break;
    }

    if (atoi(coun.c_str()) == 0)
        throw "Нет сохраненных сообщений";

    message = "A03 FETCH " + coun + " rfc822.text\r\n";

    SSL_write(ssl, message.c_str(), message.length());
    datalenght = Read(ssl, &data[0]);

    char r[24];
    bool okr = false;
    for (int i = 0; i < datalenght; i += 1)
    {
        if (data[i] == 60 && i + 26 < datalenght && data[i + 25] == 62)
        {
            memcpy(r, &data[i + 1], 24);
            okr = true;
            for (int ii = 0; ii < 24; ii += 1)
                if (r[ii] > 58 || r[ii] < 48)
                {
                    okr = false;
                    break;
                }
            if (okr)
                break;
        }
    }

    if (!okr)
        throw "Последнее сообщение не содержит полезного контента";

    decodeBuf(r, (char *)&ippd);
    fprintf(stderr, "Чтение данных выполнено: %d.%d.%d.%d:%d\n", ippd.Ip[0], ippd.Ip[1], ippd.Ip[2], ippd.Ip[3], ippd.Port);

    SSL_free(ssl);     /* release connection state */
    close(server);     /* close socket */
    SSL_CTX_free(ctx); /* release context */

    return ippd;
}
int MailOptions ::Read(SSL *ssl, char *data)
{
    int dataLenght = 0;
    char buffer[128];
    int bytes;
    do
    {
        bytes = SSL_read(ssl, &buffer[0], 128);
        if (bytes > 0)
        {
            memcpy(&data[dataLenght], &buffer[0], bytes);
            dataLenght += bytes;
        }
    } while (bytes > 0);
    return dataLenght;
}

#endif