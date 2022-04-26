using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProtectedVal
{
    public struct V<T>
    {
        private Mutex gsm;
        private Mutex getSetMutex
        {
            get
            {
                if (gsm == null)
                    gsm = new Mutex(false);
                return gsm;
            }
        }
        private T value;
        public T Value
        {
            get
            {
                T res;
                getSetMutex.WaitOne();
                res = value;
                getSetMutex.ReleaseMutex();
                return res;
            }
            set
            {
                getSetMutex.WaitOne();
                this.value = value;
                getSetMutex.ReleaseMutex();
            }
        }
    }
}
