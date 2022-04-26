using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProtectedValues
{
    public struct ProtectedVal<T>
    {
        private class destruct
        {
            ~destruct()
            {
                try { waitHandler.Close(); } catch { }
                try { waitHandler.Dispose(); } catch { }
            }
            public AutoResetEvent waitHandler;
        }
        private destruct des;
        private AutoResetEvent wH;
        private AutoResetEvent waitHandler
        {
            get
            {
                if(wH is null)
                {
                    wH = new AutoResetEvent(true);
                    des = new destruct();
                    des.waitHandler = wH;
                }
                return wH;
            }
        }
        private void WaitOne()
        {
            try { waitHandler.WaitOne(); } catch { }
        }
        private void Set()
        {
            try { waitHandler.Set(); } catch { }
        }
        private T mval;
        /// <summary>
        /// возвращает или задает объект
        /// </summary>
        public T Value
        {
            get
            {
                T v;
                WaitOne();
                v = mval;
                Set();
                return v;
            }
            set
            {
                WaitOne();
                mval = value;
                Set();
            }
        }
        /// <summary>
        /// Задает новое значение и возвращает предыдущее
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public T SetAndReturnOld(T val)
        {
            T res;
            WaitOne();
            res = mval;
            mval = val;
            Set();
            return res;
        }
        /// <summary>
        /// Позволяет безопасно выполнить действия с хранящимся объектом через синхронизацию потоков
        /// </summary>
        /// <param name="act"></param>
        public void ExecuteMethod(Action<T> act)
        {
            WaitOne();
            act(mval);
            Set();
        }
    }
}
