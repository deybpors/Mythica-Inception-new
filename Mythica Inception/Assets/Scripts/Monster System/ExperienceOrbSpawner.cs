using System;
using System.Collections.Generic;
using _Core.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Monster_System
{
    public class ExperienceOrbSpawner : MonoBehaviour
    {
        public int totalExperience;
        public float radius;
        public int slotNum;
        public GameObject orbObject;
        public int maxSize = 5;
        private int experienceGiven = 0;
        public bool activated;
        private int _count;
        
        
        void OnEnable()
        {
            if(!activated) return;
            
            for (var i = 0; i < maxSize; i++)
            {
                _count = i;
                if ((totalExperience - experienceGiven) <= 0)
                {
                    break;
                }

                GameObject orb;
                
                if (GameManager.instance == null)
                {
                    orb = Instantiate(orbObject, InSphere() * radius, Quaternion.identity);
                }
                else
                {
                    orb = GameManager.instance.pooler.SpawnFromPool(null, orbObject.name, orbObject,
                        InSphere() * radius,
                        Quaternion.identity);
                }
                
                ActivateExperienceOrb(orb.GetComponent<ExperienceOrb>());
            }
            
            gameObject.SetActive(false);
        }

        public void SpawnerSpawned(int exp)
        {
            gameObject.SetActive(false);
            totalExperience = exp;
            activated = true;
            gameObject.SetActive(true);
        }

        private Vector3 InSphere()
        {
            var position = transform.position;
            var x = Random.Range(position.x - 1,position.x + 1);
            var y = Random.Range(position.y - 1, position.y + 1);
            var z = Random.Range(position.z - 1, position.z + 1);
 
            return new Vector3(x,y,z);
        }

        private void ActivateExperienceOrb(ExperienceOrb orb)
        {
            orb.gameObject.SetActive(false);
            int experienceToGive;
            
            if (_count == maxSize - 1)
            {
                experienceToGive = totalExperience - experienceGiven;
            }
            else
            {
                experienceToGive = Random.Range(1, (totalExperience + 1) - experienceGiven);
            }
            
            if(experienceToGive <= 0) return;

            experienceGiven += experienceToGive;
            orb.slotNum = slotNum;
            orb.experience = experienceToGive;
            
            orb.gameObject.SetActive(true);
        }
    }
}
