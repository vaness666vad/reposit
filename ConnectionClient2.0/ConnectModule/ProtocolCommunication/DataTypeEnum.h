#ifndef DATATYPEENUM_H
#define DATATYPEENUM_H

    /// тип сообщения
    enum DataType : char
    {
        //пинг
        pingA = 0,
        //отчет о доставке
        report = 1,
        //регистрационное сообщение(запрос)
        registrationRequist = 2,
        //подтверждение регистрации
        registrationComlited = 3,
        //информация о подключенном/отключенном клиенте к/от сервер(у/а)
        connectedClientsInfo = 4,
        //присвоение id от сервера либо запрос id терминала в сессии
        assignedID = 5,
        //сообщение чата
        messageChat = 6,
        //информация о созданной/прерванной сессии, а так же служит для запроса о создании сессии
        sessionInfo = 7,
        //информация о качестве связи
        connectionQuality = 8,
        //данные для подключения к терминалу для удаленного устройства   
        createSessionIpPort = 9,
        //Запрос на создание новой сессии
        requistCreateSession = 10
    };

#endif