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
        private static DatabaseReference database = RealtimeDatabaseManager.Instance.DBReference;
        private static DatabaseReference hackpoints = database.Child("hackpoints");


        private static Task<HackpointData> RetrieveHackpointData(string ID)
        {
            return hackpoints.Child(ID).GetValueAsync().ContinueWith(
                    t => {
                        if (t.IsFaulted)
                        {
                            Debug.LogFormat("[HackpointDatabase] Failed retrieving hackpointdata for hackpoint {0}. {1}", ID, t.Exception);
                        }
                        else if (t.IsCompleted)
                        {
                            string json = t.Result.GetRawJsonValue();
                            //Debug.Log("Retrieved: "+json);
                            var data = JsonConvert.DeserializeObject<HackpointData>(json);
                            data.ID = ID;
                            return data;
                        }
                        return new HackpointData() {ID = ID};
                    }
                );
        }

        IEnumerator Start()
        {
            var task = RetrieveHackpointData("h1");
            yield return UtilityFunctions.RunTaskAsCoroutine(task);

            Debug.Log(task.Result.HighscoringTeam);
            Debug.Log(task.Result.Highscore);
            Debug.Log(task.Result.Position);
        }
    }
}