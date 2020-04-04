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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Firebase 
{
    public class HackpointManager : MonoBehaviour
    {
        public GameObject hackpointPrefab;

        public GameObject HackpointUI;
        
        public string[] MinigameSceneNames;

        private static DatabaseReference database;
        private static DatabaseReference hackpointReference;

        private static Dictionary<string,Collider> hackpointColliders;
        public Dictionary<string, HackpointData> hackpointData;

        private Button startMinigameButton;


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
            hackpointReference = database.Child(FirebaseRefs.HackpointRef);
            hackpointData = await RetrieveAllHackpoints();
            
            hackpointColliders = new Dictionary<string, Collider>();

            foreach(var hackpoint in hackpointData)
            {
                var pos = hackpoint.Value.Position.Coordinates.convertCoordinateToVector(10);
                var instance = Instantiate(hackpointPrefab, pos, Quaternion.identity);
                
                hackpointColliders.Add(hackpoint.Key, instance.GetComponentInChildren<Collider>());
                
            }

            
            startMinigameButton = HackpointUI.transform.Find("PlayMinigameButton").GetComponent<Button>();
        }

        void Update()
        {
            if (UtilityFunctions.OnClickDown() && hackpointColliders != null) {
                if (HackpointUI.activeSelf && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == false)
                {
                    HackpointUI.SetActive(false);
                    startMinigameButton.onClick.RemoveAllListeners();
                }
                else
                {
                    foreach (var hackpointCollider in hackpointColliders)
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (hackpointCollider.Value.Raycast (ray, out hit, Mathf.Infinity) && MinigameSceneNames.Length > 0) {

                            var input = new MinigameScene.Params();

                            var index = hackpointData[hackpointCollider.Key].MinigameIndex;
                            if (index < 0 || index >= MinigameSceneNames.Length)
                            {
                                Debug.LogWarning("The minigame index from the database is out of bounds.");
                            } 
                            else 
                            {
                                input.sceneName = MinigameSceneNames[index];
                            }
                            HackpointUI.SetActive(true);
                            

                            //Lambda magic
                            startMinigameButton.onClick.AddListener(()=>
                                    {
                                        startMinigameButton.onClick.RemoveAllListeners();
                                        HackpointUI.SetActive(false);
                                        MinigameScene.LoadMinigameScene(input, 
                                            async (outcome) => {
                                                await UploadHighscoreAtHackpoint(outcome.highscore, hackpointCollider.Key);
                                            });
                                    });
                        }
                    }
                }
            }
        }

        public static async Task UploadHighscoreAtHackpoint(int highscore, string hackpointID)
        {
            var reference = hackpointReference.Child(hackpointID).Child(FirebaseRefs.HackpointPlayerHighscoresRef).Child(AuthManager.Instance.CurrentUserID);
            var snapshot = await reference.GetValueAsync();
            if (snapshot.Value != null)
            {
                var previousHighscore = JsonConvert.DeserializeObject<int>(snapshot.GetRawJsonValue());
                if (highscore < previousHighscore)
                    return;
            }
            await reference.SetValueAsync(highscore);
        }
    }
}