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
        private static DatabaseReference hackpointReference = database.Child("hackpoints");

        //private static Dictionary<string,HackpointData> hackpoints;


        private static Task<HackpointData> RetrieveHackpointData(string ID)
        {
            return hackpointReference.Child(ID).GetValueAsync().ContinueWith(
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
            return hackpointReference.GetValueAsync().ContinueWith(
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
                Instantiate(hackpointPrefab, pos, Quaternion.identity);
                yield return UtilityFunctions.RunTaskAsCoroutine( UploadHighscoreAtHackpoint(100,hackpoint.Key) );
            }
        }

        public static async Task UploadHighscoreAtHackpoint(int highscore, string hackpointID)
        {
            var reference = hackpointReference.Child(hackpointID).Child(HackpointData.PlayerHighscoresRef).Child(AuthManager.Instance.CurrentUserID);
            //var previousHighscore = (int)(await reference.GetValueAsync()).Value;
            await reference.SetValueAsync(highscore);

        }
    }
}