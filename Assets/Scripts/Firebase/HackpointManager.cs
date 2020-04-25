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

        class MenuState 
        {
            public string currentHackpointID;
            public DatabaseReference TeamHighscoresRef; //We have to save the reference so that we can properly deregister the callback HandleHighscoreChanged. --Thomas T 21/04/2020
            public int playerHighscore;

            public MenuState(string hackpointID)
            {
                currentHackpointID = hackpointID;
                TeamHighscoresRef = hackpointReference.Child(hackpointID).Child(FirebaseRefs.HackpointTeamHighscoresRef);
                playerHighscore = 0;
            }
        }

        private MenuState openMenuState = null;


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
                if (openMenuState != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == false)
                {
                    HackpointUI.SetActive(false);
                    startMinigameButton.onClick.RemoveAllListeners();
                    openMenuState.TeamHighscoresRef.ValueChanged -= HandleHighscoreChanged;
                    openMenuState = null;
                }
                else if (openMenuState == null)
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
            openMenuState = new MenuState(hackpointID);
            openMenuState.playerHighscore = await RetrievePlayerHighscore(hackpointID);

            //Fetch team highscores
            var snapshot = await openMenuState.TeamHighscoresRef.GetValueAsync();
            int[] teamHighscores = new int[]{0,0};
            if (snapshot.Value != null)
            {
                teamHighscores = JsonConvert.DeserializeObject<int[]>(snapshot.GetRawJsonValue());
            }
            

            highScoreTextObject.text = String.Format(formatText, teamHighscores[0], teamHighscores[1], openMenuState.playerHighscore);

            openMenuState.TeamHighscoresRef.ValueChanged += HandleHighscoreChanged;

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
                        MinigameScene.LoadMinigameScene(input, UploadHighscoreAtHackpoint);
                    });
        }

        public async void UploadHighscoreAtHackpoint(MinigameScene.Outcome outcome)
        {
            int highscore = outcome.highscore;

            if (openMenuState == null)
            {
                Debug.LogWarning("The value openMenuState is null. This is invalid state.");
                return;
            }

            if (highscore > openMenuState.playerHighscore)
            {
                openMenuState.playerHighscore = highscore;
                var reference = hackpointReference.Child(openMenuState.currentHackpointID).Child(FirebaseRefs.HackpointPlayerHighscoresRef).Child(AuthManager.Instance.CurrentUserID);
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

        private void HandleHighscoreChanged(object sender, ValueChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            if (openMenuState == null)
            {
                Debug.LogWarning("The value openMenuState is null. This is invalid state for this callback HandleHighscoreChanged.");
                return;
            }
            var snapshot = args.Snapshot;
            int[] teamHighscores = new int[]{0,0};

            if (snapshot.Value != null)
            {
                teamHighscores = JsonConvert.DeserializeObject<int[]>(snapshot.GetRawJsonValue());
            }

            highScoreTextObject.text = String.Format(formatText, teamHighscores[0], teamHighscores[1], openMenuState.playerHighscore);
            return;
        }
    }
}