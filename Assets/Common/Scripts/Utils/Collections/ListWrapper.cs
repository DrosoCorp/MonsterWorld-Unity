//-----------------------------------------------------------------
// File:         ListWrapper.cs
// Description:  Wrap a list, used in serialization (List of List)
// Module:       Collection Utils
// Author:       Noé Masse
// Date:         28/03/2021
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

[Serializable]
public class ListWrapper<T>
{
    public List<T> innerList;

    public int Count => innerList.Count;

    public T this[int key]
    {
        get
        {
            return innerList[key];
        }
        set
        {
            innerList[key] = value;
        }
    }
}