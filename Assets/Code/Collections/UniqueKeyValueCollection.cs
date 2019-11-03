using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;

namespace JoyLib.Code.Collections
{
    public class UniqueKeyValueCollection<K, T> : IEnumerable<KeyValuePair<K, T>>
    {
        protected HashSet<T> m_UniqueValues;

        protected Dictionary<K, T> m_KeyValues;

        public UniqueKeyValueCollection()
        {
            m_UniqueValues = new HashSet<T>();
            m_KeyValues = new Dictionary<K, T>();
        }

        public UniqueKeyValueCollection(int capacity)
        {
            m_UniqueValues = new HashSet<T>();
            m_KeyValues = new Dictionary<K, T>(capacity);
        }

        public UniqueKeyValueCollection(UniqueKeyValueCollection<K, T> collection)
        {
            m_UniqueValues = new HashSet<T>(collection.m_UniqueValues);
            m_KeyValues = new Dictionary<K, T>(collection.m_KeyValues);
        }

        public IEnumerator<KeyValuePair<K, T>> GetEnumerator()
        {
            return m_KeyValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_KeyValues.GetEnumerator();
        }

        public bool Add(K key, T value)
        {
            if(m_UniqueValues.Contains(value))
            {
                return false;
            }

            m_UniqueValues.Add(value);
            m_KeyValues.Add(key, value);
            return true;
        }

        public bool Remove(K key, T value)
        {
            if(m_KeyValues.ContainsKey(key))
            {
                m_KeyValues.Remove(key);
                m_UniqueValues.Remove(value);
                return true;
            }

            return false;
        }

        public bool ContainsKey(K key)
        {
            return m_KeyValues.ContainsKey(key);
        }

        public bool ContainsValue(T value)
        {
            return m_UniqueValues.Contains(value);
        }

        public void OrderBy(Func<KeyValuePair<K, T>, object> func)
        {
            m_KeyValues = m_KeyValues.OrderBy(func).ToDictionary(
                x => x.Key,
                x => x.Value);
        }

        public K FetchKeyForValue(T value)
        {
            if(m_KeyValues.ContainsValue(value))
            {
                return m_KeyValues.First(x => x.Value.Equals(value)).Key;
            }

            throw new InvalidOperationException("No such value entry.");
        }

        public T FetchValueForKey(K key)
        {
            if(m_KeyValues.ContainsKey(key))
            {
                return m_KeyValues[key];
            }

            throw new InvalidOperationException("No such key entry.");
        }

        public K this[T value]
        {
            get
            {
                return FetchKeyForValue(value);
            }
        }

        public T this[K key]
        {
            get
            {
                return FetchValueForKey(key);
            }
        }
    }
}