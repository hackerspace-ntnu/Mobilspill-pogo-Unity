using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts 
{
    public class TestMinigame : MonoBehaviour
    {
        public InputField inputfield;
        public MinigameScene minigameScene;
        

        void Start()
        {
            inputfield.onEndEdit.AddListener(OnEndEdit);
        }
        public void OnEndEdit(string text){
            
            int score;
            if(Int32.TryParse(text, out score))
            {
                minigameScene.EndScene(new MinigameScene.Outcome{highscore = score});
            }
        }
    }
}
