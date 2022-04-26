#ifndef CONFIGINFO_HPP
#define CONFIGINFO_HPP

#include <stdio.h>
#include <string>
#include <string.h>
#include <cstdlib>
#include <memory>
using namespace std;

struct Config
{
    string hostname;
    string port;
    string login;
    string password;
    string identificator;
};

class ConfigInfo
{
public:
    //выполняет чтение конфиг файла, возвращает указатель на выделеную память ConfigInfo либо NULL
    static Config GetConf();
};

Config ConfigInfo::GetConf()
{
    Config conf;
    fprintf(stderr, "%s", "Чтение config файла\n");
    FILE *fp = fopen("config.txt", "r");
    if (fp == NULL)
        throw "Не удалось открыть файл config.txt";
    int status = 0;
    char ch;
    while ((ch = fgetc(fp)) != '\377')
    {
        if (ch == '\'')
        {
            status += 1;
        }
        if (ch != '\'')
        {
            if (status == 1)
            {
                conf.hostname += ch;
            }
            else if (status == 3)
            {
                conf.port += ch;
            }
            else if (status == 5)
            {
                conf.login += ch;
            }
            else if (status == 7)
            {
                conf.password += ch;
            }
            else if (status == 9)
            {
                conf.identificator += ch;
            }
        }
    }
    fclose(fp);

    fprintf(stderr, "%s", "Чтение завершено\n");

    return conf;
}
#endif