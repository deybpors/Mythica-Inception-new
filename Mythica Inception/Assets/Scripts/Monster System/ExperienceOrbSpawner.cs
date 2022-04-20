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
        private GameObject _thisObject;
        private Transform _thisTransform;

        private Dictionary<GameObject, ExperienceOrb> _orbs = new Dictionary<GameObject, ExperienceOrb>();
        private Dictionary<ExperienceOrb, GameObject> _orbObjects = new Dictionary<ExperienceOrb, GameObject>();


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

                GameObject orbObj;
                
                if (GameManager.instance == null)
                {
                    orbObj = Instantiate(orbObject, InSphere() * radius, Quaternion.identity);
                }
                else
                {
                    orbObj = GameManager.instance.pooler.SpawnFromPool(null, orbObject.name, orbObject,
                        InSphere() * radius,
                        Quaternion.identity);
                }

                if (!_orbs.TryGetValue(orbObj, out var expOrb))
                {
                    expOrb = orbObj.GetComponent<ExperienceOrb>();
                    try
                    {
                        _orbs.Add(orbObj, expOrb);
                        _orbObjects.Add(expOrb, orbObj);
                    }
                    catch
                    {
                        //ignored
                    }
                }

                ActivateExperienceOrb(expOrb);
            }

            _thisObject.SetActive(false);
        }

        public void SpawnerSpawned(int exp)
        {
            if (_thisObject == null)
            {
                _thisObject = gameObject;
            }
            _thisObject.SetActive(false);
            totalExperience = exp;
            activated = true;
            _thisObject.SetActive(true);
        }

        private Vector3 InSphere()
        {
            if (_thisTransform == null)
            {
                _thisTransform = transform;
            }
            var position = _thisTransform.position;
            var x = Random.Range(position.x - 1,position.x + 1);
            var y = Random.Range(position.y - 1, position.y + 1);
            var z = Random.Range(position.z - 1, position.z + 1);
 
            return new Vector3(x,y,z);
        }

        private void ActivateExperienceOrb(ExperienceOrb expOrb)
        {
            if (!_orbObjects.TryGetValue(expOrb, out var orbObj))
            {
                orbObj = expOrb.gameObject;
                try
                {
                    _orbObjects.Add(expOrb, orbObj);
                }
                catch
                {
                    //ignored
                }
            }

            orbObj.SetActive(false);
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
            expOrb.slotNum = slotNum;
            expOrb.experience = experienceToGive;
            
            orbObj.SetActive(true);
        }
    }
}
