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
        private static DatabaseReference database = RealtimeDatabaseManager.Instance.DBReference;
        private static DatabaseReference hackpoints = database.Child("hackpoints");


        private static Task<HackpointData> RetrieveHackpointData(string ID)
        {
            return hackpoints.Child(ID).GetValueAsync().LogErrorOrContinueWith(
                    t => {
                        string json = t.Result.GetRawJsonValue();
                        //Debug.Log("Retrieved: "+json);
                        var data = JsonConvert.DeserializeObject<HackpointData>(json);
                        data.ID = ID;
                        return data;
                    }
                );
        }

        private static Task<Dictionary<string, HackpointData>> RetrieveAllHackpoints()
        {
            return hackpoints.GetValueAsync().LogErrorOrContinueWith(
                    t => {
                        string json = t.Result.GetRawJsonValue();
                        //Debug.Log("Retrieved: "+json);
                        return JsonConvert.DeserializeObject<Dictionary<string, HackpointData>>(json);
                    }
                );
        }

        IEnumerator Start()
        {
            var task = RetrieveAllHackpoints();
            yield return UtilityFunctions.RunTaskAsCoroutine(task);

            var hackpoints = task.Result;

            foreach(var hackpoint in hackpoints)
            {
                Debug.Log(hackpoint.Key);
                var pos = hackpoint.Value.Position.Coordinates.convertCoordinateToVector(10);
                Instantiate(hackpointPrefab, pos, Quaternion.Euler(-90,0,0));
            }
        }
    }
}