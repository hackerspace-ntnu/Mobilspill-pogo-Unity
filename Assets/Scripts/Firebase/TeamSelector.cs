using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Models;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Assets.Scripts.Firebase
{
    public class TeamSelector : MonoBehaviour
    {
        public Text teamLabel;

        private static DatabaseReference database;
        private static DatabaseReference teamReference;

        private static DatabaseReference teamIndex;

        private static string currentIndex;

        private static string currentTeamsString;

        private static async Task<Dictionary<string, string>> RetrieveTeam()
        {
            var t = await teamIndex.GetValueAsync();
            string json = t.GetRawJsonValue();
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        async void Start()
        {
            Debug.Log(AuthManager.Instance.CurrentUserID);
            Debug.Log("Initialising database");
            database           = RealtimeDatabaseManager.Instance.DBReference;
            Debug.Log("Getting reference to team index");
            teamIndex          = database.Child("team_index");
            Debug.Log("Fetching team data");
            var indexData      = await RetrieveTeam();


            var teamComps = database.Child("team_comps");

            var t = await teamComps.GetValueAsync(); 
            string json = t.GetRawJsonValue();
            var snapshot = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var currentIndex = snapshot["current_index"] as string;
            var teams = snapshot[currentIndex].Split(',');
            int myIndex = Int32.Parse(indexData[AuthManager.Instance.CurrentUserID]);
            string myTeam = teams[myIndex];

            teamLabel.text = ("Team " + myTeam).ToUpper();
        }

        void Update()
        {

        }
    }


}