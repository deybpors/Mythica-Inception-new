using System.Collections;
using UnityEngine;

namespace Assets.Scripts._Core
{
    public class Health : MonoBehaviour
    {
        public EntityHealth health;
        [HideInInspector] public bool tookHit;
        public void ReduceHealth(int damage)
        {
            health.currentHealth -= damage;
            if (health.currentHealth < 0)
            {
                health.currentHealth = 0;
            }

            tookHit = true;
            StartCoroutine("TookDamage");
        }

        public void AddHealth(int amountToHeal)
        {
            health.currentHealth += amountToHeal;
            if (health.currentHealth > health.maxHealth)
            {
                health.currentHealth = health.maxHealth;
            }
        }

        public void UpdateHealth(int updatedMaxHealth, int updatedCurrentHealth)
        {
            health.currentHealth = updatedCurrentHealth;
            health.maxHealth = updatedMaxHealth;
        }

        IEnumerator TookDamage()
        {
            yield return new WaitForSeconds(.5f);
            tookHit = false;
        }
    }

    [System.Serializable]
    public class EntityHealth
    {
        public int maxHealth;
        public int currentHealth;
    }
}
