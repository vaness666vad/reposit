using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication
{
    /// <summary>
    /// тип сообщения
    /// </summary>
    public enum DataType : byte
    {
        /// <summary>
        /// Пакет проверяет пинг
        /// </summary>
        ping = 0,//пинг
        /// <summary>
        /// Пакет возвращает отчет о возвращении пинга
        /// </summary>
        report = 1,//отчет о доставке
        /// <summary>
        /// регистрационное сообщение(запрос)
        /// </summary>
        registrationRequist = 2,//регистрационное сообщение(запрос)
        /// <summary>
        /// подтверждение регистрации
        /// </summary>
        registrationComlited = 3,//подтверждение регистрации
        /// <summary>
        /// информация о подключенном/отключенном клиенте к/от сервер(у/а)
        /// </summary>
        connectedClientsInfo = 4,//информация о подключенном/отключенном клиенте к/от сервер(у/а)
        /// <summary>
        /// присвоение временного id
        /// </summary>
        assignedID = 5,//присвоение id
        /// <summary>
        /// сообщение чата
        /// </summary>
        messageChat = 6,//сообщение чата
        /// <summary>
        /// информация о созданной/прерванной сессии
        /// </summary>
        sessionInfo = 7,//информация о созданной/прерванной сессии, а так же служит для запроса о создании сессии
        /// <summary>
        /// информация о пинге подключенного клиента
        /// </summary>
        connectionQuality = 8,//информация о качестве связи
        /// <summary>
        /// данные для создания сессии  
        /// </summary>
        createSessionIpPort = 9,//данные для подключения к терминалу для удаленного устройства   
        /// <summary>
        /// Запрос на создание новой сессии
        /// </summary>
        requistCreateSession = 10
    }
}
