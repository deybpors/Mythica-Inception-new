using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Databases.Scripts
{
    public abstract class Database : ScriptableObject
    { 
        public abstract object FindInDatabase(ScriptableObjectWithID obj, string id);
    }
}
