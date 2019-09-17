using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Collections
{
    public class NonUniqueDictionary<K, T> : IEnumerable<Tuple<K, T>>
    {
        protected List<Tuple<K, T>> m_KeyValues;

        public NonUniqueDictionary()
        {
            m_KeyValues = new List<Tuple<K, T>>();
        }

        public NonUniqueDictionary(int capacity)
        {
            m_KeyValues = new List<Tuple<K, T>>(capacity);
        }

        public NonUniqueDictionary(NonUniqueDictionary<K, T> collection)
        {
            m_KeyValues = new List<Tuple<K, T>>(collection.m_KeyValues);
        }

        public void Add(K key, T value)
        {
            m_KeyValues.Add(new Tuple<K, T>(key, value));
        }

        public bool Remove(K key, T value)
        {
            return m_KeyValues.Remove(new Tuple<K, T>(key, value));
        }

        public bool RemoveAll(K key)
        {
            List<Tuple<K, T>> toBeRemoved = new List<Tuple<K, T>>();

            foreach(Tuple<K, T> tuple in m_KeyValues)
            {
                if(tuple.Item1.Equals(key))
                {
                    toBeRemoved.Add(tuple);
                }
            }

            m_KeyValues.RemoveAll(x => toBeRemoved.Contains(x));
            return toBeRemoved.Count > 0;
        }

        public bool ContainsKey(K key)
        {
            return m_KeyValues.Any(x => x.Item1.Equals(key));
        }

        public bool ContainsValue(T value)
        {
            return m_KeyValues.Any(x => x.Item2.Equals(value));
        }

        public int KeyCount(K key)
        {
            return m_KeyValues.Count(x => x.Item1.Equals(key));
        }

        public bool RemoveAll(T value)
        {
            List<Tuple<K, T>> toBeRemoved = new List<Tuple<K, T>>();

            foreach (Tuple<K, T> tuple in m_KeyValues)
            {
                if (tuple.Item2.Equals(value))
                {
                    toBeRemoved.Add(tuple);
                }
            }

            m_KeyValues.RemoveAll(x => toBeRemoved.Contains(x));
            return toBeRemoved.Count > 0;
        }

        public void OrderBy(Func<Tuple<K, T>, object> func)
        {
            m_KeyValues = m_KeyValues.OrderBy(func).ToList();
        }

        public List<T> this[K key]
        {
            get
            {
                return FetchValuesForKey(key);
            }
        }

        public List<K> this[T value]
        {
            get
            {
                return FetchKeysForValue(value);
            }
        }

        public List<T> FetchValuesForKey(K key)
        {
            List<T> values = new List<T>();

            foreach(Tuple<K, T> tuple in m_KeyValues)
            {
                if(tuple.Item1.Equals(key))
                {
                    values.Add(tuple.Item2);
                }
            }

            return values;
        }

        public List<K> FetchKeysForValue(T value)
        {
            List<K> keys = new List<K>();

            foreach(Tuple<K, T> tuple in m_KeyValues)
            {
                if(tuple.Item2.Equals(value))
                {
                    keys.Add(tuple.Item1);
                }
            }

            return keys;
        }

        public IEnumerator<Tuple<K, T>> GetEnumerator()
        {
            return m_KeyValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_KeyValues.GetEnumerator();
        }

        public List<Tuple<K, T>> Collection
        {
            get
            {
                return m_KeyValues.ToList();
            }
        }

        public List<K> Keys
        {
            get
            {
                return m_KeyValues.Select(x => x.Item1).ToList();
            }
        }

        public List<T> Values
        {
            get
            {
                return m_KeyValues.Select(x => x.Item2).ToList();
            }
        }
    }
}
