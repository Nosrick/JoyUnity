using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities.Statistics
{
    public class BasicValueContainer<T> where T : IBasicValue
    {
        private Dictionary<string, T> m_Values;

        public BasicValueContainer()
        {
            m_Values = new Dictionary<string, T>();
        }

        public BasicValueContainer(int capacity)
        {
            m_Values = new Dictionary<string, T>(capacity);
        }

        public BasicValueContainer(ICollection<T> collection)
        {
            m_Values = new Dictionary<string, T>();
            foreach(T item in collection)
            {
                m_Values.Add(item.Name, item);
            }
        }

        public bool Add(T value)
        {
            if(m_Values.ContainsKey(value.Name))
            {
                return false;
            }
            m_Values.Add(value.Name, value);
            return true;
        }

        public bool Remove(string value)
        {
            return m_Values.Remove(value);
        }

        public int Get(string name)
        {
            if(m_Values.ContainsKey(name))
            {
                return m_Values[name].Value;
            }
            return 0;
        }

        public bool Has(string name)
        {
            return m_Values.ContainsKey(name);
        }

        public T GetRawValue(string name)
        {
            if(m_Values.ContainsKey(name))
            {
                return m_Values[name];
            }
            throw new InvalidOperationException("Attempted to access " + this.GetType().Name + ".GetRawValue() with parameter of " + name);
        }

        public T this[string index]
        {
            get
            {
                return GetRawValue(index);
            }
        }

        public List<T> Collection
        {
            get
            {
                return m_Values.Values.ToList();
            }
        }
    }
}
