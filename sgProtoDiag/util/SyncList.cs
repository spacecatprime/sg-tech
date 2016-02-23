using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.util
{
    /// <summary>
    /// thread safe list type
    /// </summary>
    class SyncList<T>
    {
        private List<T> m_list = new List<T>();

        public int Count
        {
            get { return m_list.Count; }
        }

        public void Add(T aData)
        {
            lock (m_list)
            {
                m_list.Add(aData);
            }
        }

        public void Clear()
        {
            lock (m_list)
            {
                m_list.Clear();
            }
        }

        public bool Contains(T aData)
        {
            lock (m_list)
            {
                return m_list.Contains(aData);
            }
        }

        public void ForEach(Action<T> aAction)
        {
            lock (m_list)
            {
                m_list.ForEach(aAction);
            }
        }
    }
}
