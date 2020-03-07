using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Firebase;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts 
{
    public class MinigameScene : MonoBehaviour
    {
        private static Params loadSceneRegister = null;
    
        public Params sceneParams;

        public static void LoadMinigameScene(Params sceneParams, System.Action<Outcome> callback) 
        {
            var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            MinigameScene.loadSceneRegister = sceneParams;
            sceneParams.callback = (outcome) => 
                    { 
                        foreach(var rootObject in rootObjects)
                        {
                            rootObject.SetActive(true);
                        }
                        callback(outcome);
                    };
            SceneManager.LoadScene(sceneParams.sceneName, LoadSceneMode.Additive);
            foreach(var rootObject in rootObjects)
            {
                rootObject.SetActive(false);
            }
        }

        public void Awake() 
        {
            if (loadSceneRegister != null)
            {
                sceneParams = loadSceneRegister;
            }
            loadSceneRegister = null; // the register has served its purpose, clear the state
        }

        public void EndScene (Outcome outcome) 
        {
            SceneManager.UnloadSceneAsync(sceneParams.sceneName);
            if (sceneParams.callback != null) 
            {
                sceneParams.callback(outcome);
            }
            sceneParams.callback = null; // Protect against double calling;
        }

        [System.Serializable]
        public class Params
        {
            public System.Action<Outcome> callback;
            public string sceneName;
        }
        public class Outcome
        {
            public int highscore;
        }
        
    }
}
