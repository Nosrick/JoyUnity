using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code.Collections
{
    public class BasicValueContainer<T> : IDictionary<string, T> where T : IBasicValue
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

        public BasicValueContainer(IDictionary<string, T> collection)
        {
            m_Values = new Dictionary<string, T>();
            foreach (KeyValuePair<string, T> pair in collection)
            {
                m_Values.Add(pair.Key, pair.Value);
            }
        }

        public void Add(KeyValuePair<string, T> item)
        {
            m_Values.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            m_Values.Clear();
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return m_Values.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            if (arrayIndex >= this.Count)
            {
                throw new InvalidOperationException("Cannot specify arrayIndex greater than the collection length");
            }
            
            array = new KeyValuePair<string, T>[this.Count];

            List<T> values = this.Values.ToList();
            for (int i = arrayIndex; i < this.Count; i++)
            {
                T item = values[i];
                array[i] = new KeyValuePair<string, T>(item.Name, item);
            }
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            if (m_Values.ContainsKey(item.Key))
            {
                return m_Values.Remove(item.Key);
            }

            return false;
        }

        public int Count { get; }
        public bool IsReadOnly { get; }


        public void Add(string key, T value)
        {
            m_Values.Add(key, value);
        }

        public void Add(T value)
        {
            m_Values.Add(value.Name, value);
        }

        public bool ContainsKey(string key)
        {
            return m_Values.ContainsKey(key);
        }

        public bool Remove(string value)
        {
            return m_Values.Remove(value);
        }

        public bool TryGetValue(string key, out T value)
        {
            if (m_Values.ContainsKey(key))
            {
                value = m_Values[key];
                return true;
            }

            value = default(T);
            return false;
        }

        public int Get(string name)
        {
            return this.Has(name) ? this.GetRawValue(name).Value : 0;
        }

        public bool Has(string name)
        {
            return m_Values.Any(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public T GetRawValue(string name)
        {
            if(m_Values.Any(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return m_Values.First(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value;
            }
            throw new InvalidOperationException("Attempted to access " + this.GetType().Name + ".GetRawValue() with parameter of " + name);
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return m_Values.GetEnumerator();
        }

        public T this[string index]
        {
            get => GetRawValue(index);
            set => m_Values[index] = value;
        }

        public ICollection<string> Keys => m_Values.Keys;
        public ICollection<T> Values => m_Values.Values;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
