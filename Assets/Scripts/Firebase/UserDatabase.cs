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
    public class UserDatabase
    {
        private static DatabaseReference database = RealtimeDatabaseManager.Instance.DBReference;

        public static DatabaseReference Usernames = database.Child(FirebaseRefs.UsernamesRef);
        public static DatabaseReference Score = database.Child(FirebaseRefs.ScoreRef);
        public static DatabaseReference Modified = database.Child(FirebaseRefs.ModifiedRef);
        public static DatabaseReference Positions = database.Child(FirebaseRefs.PositionsRef);
        public static DatabaseReference LoggedIn = database.Child(FirebaseRefs.LoggedInRef);
        public static DatabaseReference TeamIndex = database.Child(FirebaseRefs.TeamIndexRef);
        public static async Task<DataSnapshot> RetrievePropertyData(DatabaseReference databaseReference, string userId)
        {
            return await databaseReference.Child(userId).GetValueAsync();
        }

        public static async void UpdatePropertyData(DatabaseReference databaseReference, string userId, object value)
        {
            string jsonValue = JsonConvert.SerializeObject(value);
            // Debug.Log("Updating: " +jsonValue);

            await databaseReference.Child(userId).SetRawJsonValueAsync(jsonValue);
            // var modified = new Dictionary<string, System.Object>();
            // modified[userId] = ServerValue.Timestamp;
            // Modified.SetValueAsync(modified);
        }

        public static async Task RemovePropertyData(DatabaseReference databaseReference, string userId)
        {
            Debug.Log("Removing property:" + databaseReference.ToString());
            await databaseReference.Child(userId).RemoveValueAsync();
        }
    }
}