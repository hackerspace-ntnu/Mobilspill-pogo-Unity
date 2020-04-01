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
        private static DatabaseReference database;
        private static DatabaseReference teamReference;

        private static string currentIndex;

        private static string currentTeamsString;

        // public GameObject teamS elector = GameObject.Find("TeamSelectionTempMenu");


        private static async Task<Dictionary<string, string>> RetrieveTeams()
        {
            var t = await teamReference.GetValueAsync();
            string json = t.GetRawJsonValue();
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        async void Start()
        {
            database           = RealtimeDatabaseManager.Instance.DBReference;
            teamReference      = database.Child("team_comps");
            var teamData       = await RetrieveTeams();
            currentIndex       = teamData["current_index"];
            currentTeamsString = teamData[currentIndex];
            var currentTeams   = currentTeamsString.Split(',');
            Debug.Log("Current index " +  currentIndex);
            Debug.Log("Current team string " + currentTeamsString);

            Dropdown selector = GameObject.Find("TeamSelectionTempMenu").GetComponent<Dropdown>();

            List<string> selectorOptions = selector.options;

            selectorOptions[0] = currentTeams[0];
            selectorOptions[1] = currentTeams[1];
            selector.options = selectorOptions as Dropdown.OptionData;
            // Debug.Log(selector.options.ToString());
        }

        void Update()
        {

        }
    }


}