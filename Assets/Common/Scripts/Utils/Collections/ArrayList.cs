//-----------------------------------------------------------------
// File:         ArrayList.cs
// Description:  Fixed size array where we can add/remove elements
// Module:       Collection Utils
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ArrayList<T>
{
    private T[] _innerArray;
    private int _count;

    public int Capacity => _innerArray.Length;
    public int Count => _count;
    public bool IsReadOnly => false;
    public T[] InnerArray => _innerArray;

    public ArrayList(int capacity)
    {
        _innerArray = new T[capacity];
        _count = 0;
    }

    public T this[int key]
    {
        get
        {
            return _innerArray[key];
        }
        set
        {
            _innerArray[key] = value;
        }
    }

    public int IndexOf(T item)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_innerArray[i].Equals(item)) return i;
        }
        return -1;
    }

    public void RemoveAt(int index)
    {
        for (int i = index; i < _count - 1; i++)
        {
            _innerArray[i] = _innerArray[i + 1];
        }
        _count--;
    }

    public void Add(T item)
    {
        _innerArray[_count++] = item;
    }

    public void Clear()
    {
        _count = 0;
    }

    public bool Contains(T item)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_innerArray[i].Equals(item)) return true;
        }
        return false;
    }

    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        else
        {
            return false;
        }

    }
}