using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProtectedVal
{
    public class ProtectedList<T>
    {
        private List<T> list = new List<T>();
        private Mutex mutex = new Mutex(false);
        public void Add(T item)
        {
            mutex.WaitOne();
            list.Add(item);
            mutex.ReleaseMutex();
        }
        public bool Remove(T item)
        {
            bool res;
            mutex.WaitOne();
            res = list.Remove(item);
            mutex.ReleaseMutex();
            return res;
        }
        public List<T> CopyList
        {
            get
            {
                List<T> res;
                mutex.WaitOne();
                res = new List<T>(list);
                mutex.ReleaseMutex();
                return res;
            }
        }
    }
}
