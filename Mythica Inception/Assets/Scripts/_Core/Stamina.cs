﻿using UnityEngine;

namespace Assets.Scripts._Core
{
    public class Stamina : MonoBehaviour
    {
        public EntityStamina stamina;
        
        public void ReduceStamina(int amountToReduce)
        {
            stamina.currentStamina -= amountToReduce;
            if (stamina.currentStamina < 0)
            {
                stamina.currentStamina = 0;
            }
        }

        public void AddStamina(int amountToAdd)
        {
            stamina.currentStamina += amountToAdd;
            if (stamina.currentStamina > stamina.fullStamina)
            {
                stamina.currentStamina = stamina.fullStamina;
            }
        }

        public void UpdateHealth(int updatedFullStamina, int updatedCurrentStamina)
        {
            stamina.currentStamina = updatedCurrentStamina;
            stamina.fullStamina = updatedFullStamina;
        }
    }

    public class EntityStamina
    {
        public int currentStamina;
        public int fullStamina;
    }
}