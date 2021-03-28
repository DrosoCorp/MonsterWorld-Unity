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
public class ArrayWrapper<T>
{
    public T[] innerArray;

    public int Length => innerArray.Length;

    public T this[int key]
    {
        get
        {
            return innerArray[key];
        }
        set
        {
            innerArray[key] = value;
        }
    }
}