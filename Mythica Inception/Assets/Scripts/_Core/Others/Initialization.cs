﻿using System;
using _Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Core.Others
{
    public class Initialization : MonoBehaviour
    {
        public ScenePicker starting;
        private AsyncOperation _managerOp;
        void Awake()
        {
            _managerOp = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(starting.path, LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }

        private void Update()
        {
            if (_managerOp.isDone)
            {
                GameManager.instance.currentWorldScenePath = starting.path;
            }
        }
    }
}