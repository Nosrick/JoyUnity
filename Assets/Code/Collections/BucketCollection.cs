using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Collections
{
    public class BucketCollection<K, T> : IEnumerable<KeyValuePair<K, List<T>>>
    {
        protected Dictionary<K, List<T>> m_KeyValues;

        public BucketCollection()
        {
            m_KeyValues = new Dictionary<K, List<T>>();
        }

        public BucketCollection(int capacity)
        {
            m_KeyValues = new Dictionary<K, List<T>>(capacity);
        }

        public BucketCollection(BucketCollection<K, T> collection)
        {
            m_KeyValues = new Dictionary<K, List<T>>(collection.m_KeyValues);
        }

        public KeyValuePair<K, List<T>> Add(K key, T value)
        {
            foreach(KeyValuePair<K, List<T>> pair in m_KeyValues)
            {
                if(pair.Key.Equals(key))
                {
                    pair.Value.Add(value);
                    return pair;
                }
            }

            List<T> newList = new List<T>();
            newList.Add(value);
            KeyValuePair<K, List<T>> newPair = new KeyValuePair<K, List<T>>(key, newList);
            m_KeyValues.Add(key, newList);

            return newPair;
        }

        public bool Remove(K key)
        {
            foreach(KeyValuePair<K, List<T>> pair in m_KeyValues)
            {
                if(pair.Key.Equals(key))
                {
                    m_KeyValues.Remove(key);
                    return true;
                }
            }

            return false;
        }

        public int RemoveForValue(T value)
        {
            Dictionary<K, List<T>> removals = m_KeyValues.Where(x => x.Value.Equals(value))
                                                .ToDictionary(x => x.Key, x => x.Value);

            foreach(K key in removals.Keys)
            {
                m_KeyValues.Remove(key);
            }

            return removals.Count;
        }

        public bool ContainsKey(K key)
        {
            return m_KeyValues.Any(x => x.Key.Equals(key));
        }

        public bool ContainsValue(T value)
        {
            return m_KeyValues.Any(x => x.Value.Contains(value));
        }

        public int KeyCount(K key)
        {
            return m_KeyValues.Count(x => x.Key.Equals(key));
        }

        public void OrderBy(Func<KeyValuePair<K, List<T>>, object> func)
        {
            m_KeyValues = m_KeyValues.OrderBy(func).ToDictionary(x => x.Key, x => x.Value);
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

            foreach(KeyValuePair<K, List<T>> tuple in m_KeyValues)
            {
                if(tuple.Key.Equals(key))
                {
                    values.AddRange(tuple.Value);
                }
            }

            return values;
        }

        public List<K> FetchKeysForValue(T value)
        {
            List<K> keys = new List<K>();

            foreach(KeyValuePair<K, List<T>> tuple in m_KeyValues)
            {
                if(tuple.Value.Contains(value))
                {
                    keys.Add(tuple.Key);
                }
            }

            return keys;
        }

        public IEnumerator<KeyValuePair<K, List<T>>> GetEnumerator()
        {
            return m_KeyValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_KeyValues.GetEnumerator();
        }

        public List<K> Keys
        {
            get
            {
                return m_KeyValues.Select(x => x.Key).ToList();
            }
        }

        public List<T> Values
        {
            get
            {
                List<T> values = new List<T>();

                foreach(K key in Keys)
                {
                    values.AddRange(FetchValuesForKey(key));
                }

                return values;
            }
        }
    }
}
