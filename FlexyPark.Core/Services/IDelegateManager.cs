using System;
using System.Collections.Generic;

namespace FlexyPark.Core.Services
{
    public interface IDelegateManager
    {
        string SetAction<T, K>(Action<T> action, K parameter = default(K));
        Action<T> GetAction<T, K>(string key);
        K GetObject<T, K>(string key);
        void Remove(string key);
    }

    public class DelegateObject<T, K> 
    {
        public Action<T> Delegate { get;set; }        
        public K Parameter { get;set; }
    }

    public class DelegateManager : IDelegateManager
    {
        private IDictionary<string, object> mInternalDictionary = new Dictionary<string, object>();

        public string SetAction<T, K>(Action<T> action, K parameter = default(K))
        {
            var key = Guid.NewGuid().ToString();
            mInternalDictionary.Add(key, new DelegateObject<T, K>() { Delegate = action, Parameter = parameter }  );

            return key;
        }

        public Action<T> GetAction<T, K>(string key)
        {
            if (mInternalDictionary.Count > 0)
            {
                var _object = (DelegateObject<T, K>)mInternalDictionary[key];
                return _object.Delegate;
            }
            else
                return null;
        }

        public K GetObject<T, K>(string key)
        {
            if (mInternalDictionary.Count > 0)
            {
                var _object = (DelegateObject<T, K>)mInternalDictionary[key];
                return _object.Parameter;
            }
            else
                return default(K);
        }

        public void Remove(string key)
        {
            mInternalDictionary.Remove(key);
        }
    }
}

