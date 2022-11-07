using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    [Serializable]
    /// <summary>
    /// 实现一个泛型HashTable类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class THashTable<T> : IEnumerable
    {
        private Hashtable ht;

        private List<object> shadowIndex;

        public IEnumerator GetEnumerator()
        {
            return ht.GetEnumerator();
        }

        public THashTable()
        {
            ht = new Hashtable();
            shadowIndex = new List<object>();
        }

        public THashTable(int capacity)
        {
            ht = new Hashtable(capacity);
        }
        /// <summary>
        /// 增加一个索引及其对应的元素
        /// 索引以object形式存储
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public void Add(object key, T obj)
        {
            ht.Add(key, obj);
            shadowIndex.Add(key);
        }

        public void Insert(int index, object key, T obj)
        {
            ht.Add(key, obj);
            shadowIndex.Insert(index, key);
        }

        public void Remove(object key)
        {
            ht.Remove(key);
            shadowIndex.Remove(key);
        }
        /// <summary>
        /// 获取指定索引对应的值
        /// 若索引是int值，为与以下标形式获取值的方式区别，应使用(object)key的形式作为参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T this[object key]
        {
            get
            {
                T a;
                try
                {
                    a = (T)ht[key];

                }
                catch (Exception e)
                {
                    throw e;
                }
                return a;
            }
            set { ht[key] = value; }
        }
        /// <summary>
        /// 获取指定下标对应的值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new public T this[int index]
        {
            get
            {
                T a;
                try
                {
                    a = (T)ht[shadowIndex[index]];

                }
                catch (Exception e)
                {
                    throw e;
                }
                return a;
            }
            set
            {
                ht[shadowIndex[index]] = value;
            }
        }
        public int Count
        {
            get { return ht.Count; }
        }

        public bool ContainsKey(object key)
        {
            return ht.ContainsKey(key);
        }
        public ICollection Keys
        {
            get
            {
                return ht.Keys;
            }
        }
        public ICollection Values
        {
            get
            {
                return ht.Values;
            }
        }

    }
}
