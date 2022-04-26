using ProtectedValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Terminal.TerminalWindow;

namespace Terminal.SessionWindow
{
    public class SessionModel : BaseModel
    {
        private static ProtectedVal<List<SessionModel>> sessions;
        private static ProtectedVal<List<SessionModel>> getsessions
        {
            get
            {
                if (sessions.Value == null)
                    sessions.Value = new List<SessionModel>();
                return sessions;
            }
        }
        public static List<SessionModel> GetSessions()
        {
            List<SessionModel> res = null;
            getsessions.ExecuteMethod((x) => res = new List<SessionModel>(x));
            return res;
        }
        public static void AddSessions(SessionModel ses)
        {
            getsessions.ExecuteMethod((x) => x.Add(ses));
        }
        public static void RemoveSessions(SessionModel ses)
        {
            getsessions.ExecuteMethod((x) => x.Remove(ses));
        }
        public string Title => $"{remoteName}:[{terminalGuid}]";
        private string remoteName { get; set; }
        private string terminalGuid => string.Join(":", _SessionClient.RemotelGuid);
        public SessionClient _SessionClient { get; private set; }
        public SessionW _SessionW { get; private set; }
        public SessionModel(SessionW sw, Socket c, string remName, byte[] remotelGuid)
        {
            AddSessions(this);
            remoteName = remName;
            _SessionW = sw;
            _SessionClient = new SessionClient(c, this, remotelGuid);
            baseClient = _SessionClient;
        }
        public void DispatcherSessionModel(Action<SessionModel> act)
        {
            _SessionW.Dispatcher.Invoke(act, this);
        }
        public void DispatcherInvokeSessionW(Action<SessionW> act)
        {
            _SessionW.Dispatcher.Invoke(act, _SessionW);
        }
    }
}
