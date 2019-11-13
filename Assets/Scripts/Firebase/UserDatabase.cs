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

        public static DatabaseReference Usernames => database.Child("/usernames/");
        public static DatabaseReference Score => database.Child("/scores/");
        public static DatabaseReference Modified => database.Child("/modified/");
        public static DatabaseReference Positions => database.Child("/positions/");
        public static DatabaseReference LoggedIn => database.Child("/logged_in/");
        //public static DatabaseReference UserSince => database.Child("/user_since/");

    
        public static async Task<UserData> RetrieveUserData(string userId)
        {
            var username = (string)await RetrievePropertyData(Usernames, userId);
            //var score = (int) await RetrievePropertyData(Score, userId);
            
            return new UserData{ Username = username , UserId = userId, Score = 0};
        }
        public static Task UpdateUserData(UserData userData)
        {
            var t1 = UpdatePropertyData(Usernames, userData.UserId, userData.Username);
            var t2 = UpdatePropertyData(Score, userData.UserId, userData.Score);
            return Task.WhenAll(t1,t2);
        }

        public static Task AddUser(UserData userData)
        {
            var t1 = UpdatePropertyData(Usernames, userData.UserId, userData.Username);
            var t2 = UpdatePropertyData(Score, userData.UserId, userData.Score);
            var t3 = UpdatePropertyData(Positions, userData.UserId, new Position(0,0,0).ToDictionary());
            var t4 = UpdatePropertyData(LoggedIn, userData.UserId, false);
            return Task.WhenAll(t1,t2, t3, t4);
        }

        public static Task<object> RetrievePropertyData(DatabaseReference databaseReference, string userId)
        {
            return databaseReference.Child(userId).GetValueAsync().ContinueWith(
                        t => {
                            if (t.IsFaulted)
                            {
                                Debug.LogFormat("[UserDatabase] Failed retrieving property data {0} for user {1}: {2}", databaseReference.Key, userId,  t.Exception);
                            }
                            else if (t.IsCompleted)
                            {
                                var snapshot = t.Result;
                                return snapshot.Value;
                            }
                            return null;
                        }
                    );
        }

        public static Task UpdatePropertyData(DatabaseReference databaseReference, string userId, object value)
        {
            Debug.Log("Updating property:" + databaseReference.ToString());
            var dict = new Dictionary<string, System.Object>();
            dict[userId] = value;
            return databaseReference.UpdateChildrenAsync(dict).ContinueWith(t=> {
                            if (t.IsFaulted)
                            {
                                Debug.LogFormat("[UserDatabase] Failed updating property data {0} for user {1}: {2}", databaseReference.Key, userId,  t.Exception);
                            }
                        });


            // var modified = new Dictionary<string, System.Object>();
            // modified[userId] = ServerValue.Timestamp;
            // Modified.SetValueAsync(modified);
        }

        public static Task RemovePropertyData(DatabaseReference databaseReference, string userId)
        {
            Debug.Log("Removing property:" + databaseReference.ToString());
            return databaseReference.Child(userId).RemoveValueAsync();
        }
    }
}