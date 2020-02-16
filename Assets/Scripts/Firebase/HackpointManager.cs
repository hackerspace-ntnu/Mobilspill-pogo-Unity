using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Models;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Scripts.Firebase 
{
    public class HackpointManager : MonoBehaviour
    {
        public GameObject hackpointPrefab;
        
        public string[] MinigameSceneNames;

        private static DatabaseReference database;
        private static DatabaseReference hackpointReference;

        private static Dictionary<string,Collider> hackpointColliders;


        private static async Task<HackpointData> RetrieveHackpointData(string ID)
        {
            var t = await hackpointReference.Child(ID).GetValueAsync();
            string json = t.GetRawJsonValue();
            var data = JsonConvert.DeserializeObject<HackpointData>(json);
            return data;
        }

        private static async Task<Dictionary<string, HackpointData>> RetrieveAllHackpoints()
        {
            var t = await hackpointReference.GetValueAsync();
            string json = t.GetRawJsonValue();
            return JsonConvert.DeserializeObject<Dictionary<string, HackpointData>>(json);
        }

        async void Start()
        {
            database = RealtimeDatabaseManager.Instance.DBReference;
            hackpointReference = database.Child("hackpoints");

            // var task = RetrieveAllHackpoints();
            // yield return UtilityFunctions.RunTaskAsCoroutine(task);

            var hackpointData = await RetrieveAllHackpoints();
            
            hackpointColliders = new Dictionary<string, Collider>();
            Debug.Log(hackpointData.Count);
            foreach(var hackpoint in hackpointData)
            {
                var pos = hackpoint.Value.Position.Coordinates.convertCoordinateToVector(10);
                var instance = Instantiate(hackpointPrefab, pos, Quaternion.identity);
                
                hackpointColliders.Add(hackpoint.Key, instance.GetComponentInChildren<Collider>());
                
            }
        }

        void Update()
        {
            if (UtilityFunctions.OnClickDown()) {

                foreach (var hackpoint in hackpointColliders)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (hackpoint.Value.Raycast (ray, out hit, Mathf.Infinity) && MinigameSceneNames.Length > 0) {

                        var input = new MinigameScene.Params();

                        input.sceneName = MinigameSceneNames[0];

                        MinigameScene.LoadMinigameScene(input, 
                            async (outcome) => {
                                await UploadHighscoreAtHackpoint(outcome.highscore, hackpoint.Key);
                            });
                    }
                }
            }

        }

        public static async Task UploadHighscoreAtHackpoint(int highscore, string hackpointID)
        {
            var reference = hackpointReference.Child(hackpointID).Child(HackpointData.PlayerHighscoresRef).Child(AuthManager.Instance.CurrentUserID);
            var previousHighscore = JsonConvert.DeserializeObject<int>((await reference.GetValueAsync()).GetRawJsonValue());
            if (highscore >= previousHighscore)
                await reference.SetValueAsync(highscore);
        }
    }
}