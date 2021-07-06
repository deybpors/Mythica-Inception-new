using System.Collections.Generic;
using System.Linq;
using Assets.Scripts._Core;
using Assets.Scripts.Items_and_Barter_System.Scripts;
using UnityEngine;

namespace Assets.Scripts.Databases.Scripts
{
    [CreateAssetMenu(menuName = "Databases/Items Database")]
    public class ItemDatabase : Database
    {
        public List<ItemObject> items;
        
        public override object FindInDatabase(ScriptableObjectWithID obj, string id)
        {
            return items.FirstOrDefault(item => obj.ID.Equals(id));
        }
    }
}
