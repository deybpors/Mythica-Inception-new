using System;
using System.Collections.Generic;
using Assets.Scripts._Core.Others;
using UnityEngine;

namespace Assets.Scripts.Databases.Scripts
{
    [CreateAssetMenu(menuName = "Database/New Database")]
    public class Database : ScriptableObject
    {
        public List<ScriptableObjectWithID> data  = new List<ScriptableObjectWithID>();
        protected Dictionary<string, ScriptableObjectWithID> dictionary = new Dictionary<string, ScriptableObjectWithID>();

        void Awake()
        {
            foreach (var obj in data)
            {
                try
                {
                    dictionary.Add(obj.ID, obj);
                }
                catch (ArgumentException)
                {
                    Debug.LogWarning("A data with ID = " + obj.ID + "  already exists.\nConflict: " + obj.name);
                }
            }
        }
        
        public ScriptableObjectWithID FindInDatabase(string id)
        {
            return dictionary.TryGetValue(id, out var value) ? value : null;
        }
    }
}
