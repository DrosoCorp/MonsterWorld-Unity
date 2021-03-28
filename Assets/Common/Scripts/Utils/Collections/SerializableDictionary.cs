//-----------------------------------------------------------------
// File:         SerializableDictionary.cs
// Description:  A Dicitonary that can be stored with unity serialization
// Module:       Collection Utils
// Author:       https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBase/Collections/SerializableDictionaryBase.cs
// Date:         28/03/2021
//-----------------------------------------------------------------

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterWorld.Utils.Collections
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [NonSerialized] private Dictionary<TKey, TValue> _dictionary;
        [NonSerialized] private IEqualityComparer<TKey> _comparer;

        public SerializableDictionary() {}
        public SerializableDictionary(IEqualityComparer<TKey> comparer) { _comparer = comparer; }

        public IEqualityComparer<TKey> Comparer => _comparer;

        #region ISerializationCallbackReceiver

        [SerializeField] private TKey[] _keys;
        [SerializeField] private TValue[] _values;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_keys != null && _values != null)
            {
                if (_dictionary == null)
                {
                    _dictionary = new Dictionary<TKey, TValue>(_keys.Length, _comparer);
                }
                else
                {
                    _dictionary.Clear();
                }

                for (int i = 0; i < _keys.Length; i++)
                {
                    if (i < _values.Length)
                    {
                        _dictionary[_keys[i]] = _values[i];
                    }
                    else
                    {
                        _dictionary[_keys[i]] = default(TValue);
                    }

                }
            }

            _keys = null;
            _values = null;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_dictionary == null || _dictionary.Count == 0)
            {
                _keys = null;
                _values = null;
            }
            else
            {
                int cnt = _dictionary.Count;
                _keys = new TKey[cnt];
                _values = new TValue[cnt];
                int i = 0;
                foreach (var entry in _dictionary)
                {
                    _keys[i] = entry.Key;
                    _values[i] = entry.Value;
                    i++;
                }
            }
        }
        #endregion

        #region IDictionary
        public int Count => _dictionary != null ? _dictionary.Count : 0;

        public void Add(TKey key, TValue value)
        {
            if (_dictionary == null)
            {
                _dictionary = new Dictionary<TKey, TValue>(_comparer);
            }
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            if (_dictionary == null)
            {
                return false;
            }
            return _dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                if (_dictionary == null)
                {
                    _dictionary = new Dictionary<TKey, TValue>(_comparer);
                }
                return _dictionary.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            if (_dictionary == null)
            {
                return false;
            }
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dictionary == null)
            {
                value = default(TValue);
                return false;
            }
            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (_dictionary == null)
                {
                    _dictionary = new Dictionary<TKey, TValue>(_comparer);
                }
                return _dictionary.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_dictionary == null)
                {
                    throw new KeyNotFoundException();
                }
                return _dictionary[key];
            }
            set
            {
                if (_dictionary == null)
                {
                    _dictionary = new Dictionary<TKey, TValue>(_comparer);
                }
                _dictionary[key] = value;
            }
        }

        public void Clear()
        {
            if (_dictionary != null)
            {
                _dictionary.Clear();
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary == null)
            {
                _dictionary = new Dictionary<TKey, TValue>(_comparer);
            }
            (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary == null)
            {
                return false;
            }
            return (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (_dictionary == null)
            {
                return;
            }
            (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary == null)
            {
                return false;
            }
            return (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            if (_dictionary == null)
            {
                return default(Dictionary<TKey, TValue>.Enumerator);
            }
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_dictionary == null)
            {
                return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            }
            return _dictionary.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            if (_dictionary == null)
            {
                return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            }
            return _dictionary.GetEnumerator();
        }
        #endregion



    }
}
