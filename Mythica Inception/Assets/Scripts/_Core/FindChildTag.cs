using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts._Core
{
    public static class FindChildTag
    {
        public static List<Transform> FindChildrenWithTag(this Transform parent, string tag)
        {
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.CompareTag(tag))
                {
                    children.Add(child);
                }
            }

            if (children.Count > 0)
            {
                return null;
            }

            return children;
        }
 
        public static Transform FindChildWithTag(this Transform parent, string tag)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.CompareTag(tag))
                {
                    return child;
                }
            }
            return null;
        }
        
    }
}