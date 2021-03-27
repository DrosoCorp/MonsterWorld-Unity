//-----------------------------------------------------------------
// File:         SystemDependencyAttribute.cs
// Description:  Used for dependency injection
// Module:       Core
// Author:       Noé Masse
// Date:         21/03/2021
//-----------------------------------------------------------------
using System;

namespace MonsterWorld.Core
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SystemDependencyAttribute : Attribute
    {

    }
}