using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public class Health : MonoBehaviour
    {
        public List<EntitiesHealth> entitiesHealth;
        [HideInInspector] public bool tookHit;

        public void ReduceHealth(int damage, int entity)
        {
            entitiesHealth[entity].currentHealth -= damage;
            if (entitiesHealth[entity].currentHealth < 0)
            {
                entitiesHealth[entity].currentHealth = 0;
            }

            tookHit = true;
            StartCoroutine("TookDamage");
        }

        public void AddHealth(int damage, int entity)
        {
            entitiesHealth[entity].currentHealth += damage;
            if (entitiesHealth[entity].currentHealth > entitiesHealth[entity].maxHealth)
            {
                entitiesHealth[entity].currentHealth = entitiesHealth[entity].maxHealth;
            }
        }

        public void UpdateMaxHealth(int updatedMaxHealth, int entity)
        {
            entitiesHealth[entity].maxHealth = updatedMaxHealth;
        }

        IEnumerator TookDamage()
        {
            yield return new WaitForSeconds(.5f);
            tookHit = false;
        }
    }

    [System.Serializable]
    public class EntitiesHealth
    {
        public int maxHealth;
        public int currentHealth;
    }
    
    [CustomEditor(typeof(Health))]
    public class DamageEditor : Editor 
    {
        public override void OnInspectorGUI()
        {
            Health myTarget = (Health)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Damage Monster", GUILayout.Height(40)))
            {
                if(Application.isPlaying)
                    myTarget.ReduceHealth(0, 0);
                else
                {
                    Debug.LogWarning("DamageObject script Warning: The project is currently not in GameMode");
                }
            }
        }
    }
}
