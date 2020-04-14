using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Models;
using Firebase;
using Firebase.Database;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

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
        private TextMeshProUGUI highScoreTextObject;
        private string formatText;


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
            highScoreTextObject = HackpointUI.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            formatText = highScoreTextObject.text;
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
                    if (MinigameSceneNames.Length == 0)
                    {
                        return;
                    }
                    float minDistance = float.MaxValue;
                    string closestHackpoint = "";
                    foreach (var hackpointCollider in hackpointColliders)
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (hackpointCollider.Value.Raycast (ray, out hit, Mathf.Infinity)) {
                            if (hit.distance < minDistance)
                            {
                                minDistance = hit.distance;
                                closestHackpoint = hackpointCollider.Key;
                            }
                        }
                    }
                    if (closestHackpoint != "")
                    {
                        DisplayHackpointMenu(closestHackpoint);
                    }
                }
            }
        }

        public async void DisplayHackpointMenu(string hackpointID)
        {
            int playerHighscore = await UpdateHackpointMenu(hackpointID);
            HackpointUI.SetActive(true);


            var input = new MinigameScene.Params();

            var index = hackpointData[hackpointID].MinigameIndex;
            if (index < 0 || index >= MinigameSceneNames.Length)
            {
                Debug.LogWarning("The minigame index from the database is out of bounds.");
                input.sceneName = MinigameSceneNames[0];
            } 
            else 
            {
                input.sceneName = MinigameSceneNames[index];
            }
            //Lambda magic
            startMinigameButton.onClick.AddListener(()=>
                    {
                        startMinigameButton.onClick.RemoveAllListeners();
                        MinigameScene.LoadMinigameScene(input, 
                            async (outcome) => {
                                //This happens when the minigame scene is finished.
                                await UploadHighscoreAtHackpoint(playerHighscore, outcome.highscore, hackpointID);
                                await Task.Delay(200);
                                await UpdateHackpointMenu(hackpointID);
                            });
                    });
        }

        public async Task<int> UpdateHackpointMenu(string hackpointID)
        {
            int playerHighscore = await RetrievePlayerHighscore(hackpointID);

            int team0Highscore = 0;
            int team1Highscore = 0;
            var ref0 = hackpointReference.Child(hackpointID).Child(FirebaseRefs.HackpointTeamHighscoresRef).Child("0");
            var ref1 = hackpointReference.Child(hackpointID).Child(FirebaseRefs.HackpointTeamHighscoresRef).Child("1");
            var snap0 = await ref0.GetValueAsync();
            var snap1 = await ref1.GetValueAsync();
            if (snap0.Value != null)
            {
                team0Highscore = JsonConvert.DeserializeObject<int>(snap0.GetRawJsonValue());
            }
            if (snap1.Value != null)
            {
                team1Highscore = JsonConvert.DeserializeObject<int>(snap1.GetRawJsonValue());
            }


            highScoreTextObject.text = String.Format(formatText, team0Highscore, team1Highscore, playerHighscore);

            return playerHighscore;
        }

        public static async Task UploadHighscoreAtHackpoint(int previousHighscore, int highscore, string hackpointID)
        {
            if (highscore > previousHighscore)
            {
                var reference = hackpointReference.Child(hackpointID).Child(FirebaseRefs.HackpointPlayerHighscoresRef).Child(AuthManager.Instance.CurrentUserID);
                await reference.SetValueAsync(highscore);
            }
        }

        public static async Task<int> RetrievePlayerHighscore(string hackpointID)
        {
            int value = 0;
            var reference = hackpointReference.Child(hackpointID).Child(FirebaseRefs.HackpointPlayerHighscoresRef).Child(AuthManager.Instance.CurrentUserID);
            var snapshot = await reference.GetValueAsync();
            if (snapshot.Value != null)
            {
                value = JsonConvert.DeserializeObject<int>(snapshot.GetRawJsonValue());
            }
            return value;
        }
    }
}