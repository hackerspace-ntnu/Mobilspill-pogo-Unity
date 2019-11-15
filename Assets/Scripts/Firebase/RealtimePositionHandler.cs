using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Models;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using GoMap;
using GoShared;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.Firebase {

    public class RealtimePositionHandler : MonoBehaviour {

        public GOMap goMap;

        public RemoteUser RemoteUserPrefab;

        private Dictionary<string, RemoteUser> remoteUsers;

        private Position CurrentUserPosition = new Position();
        private Position CurrentUserPreviousPosition = new Position();

        private List<string> LoggedInPlayers = new List<string>();

        private string CurrentUserId;

        // Use this for initialization
        IEnumerator Start() {
            //Waiting for the location manager to have the world origin set.
            yield return StartCoroutine(goMap.locationManager.WaitForOriginSet());

            

            remoteUsers = new Dictionary<string, RemoteUser>();

            goMap.locationManager.onLocationChanged.AddListener(coords => {OnLocationChanged(coords);});
            UserDatabase.Positions.ChildChanged += HandlePositionChanged;
            UserDatabase.LoggedIn.ChildChanged += HandleLoggedInChanged;

            CurrentUserId = AuthManager.Instance.CurrentUser.UserId;

            if (string.IsNullOrEmpty(CurrentUserId))
            
            {
                Debug.LogError("[RealtimePositionHandler] UserId should not be null!!!");
                AuthManager.Instance.LogOut();
            }
            OnApplicationPause(false);

            UserDatabase.RetrievePropertyData(UserDatabase.Positions, CurrentUserId).ContinueWith( task => {

                var positionData = task.Result as Dictionary<string,object>;
                CurrentUserPosition.FromDictionary( positionData);
                CurrentUserPreviousPosition.FromDictionary( positionData);
            });


            var userTask = UserDatabase.LoggedIn.OrderByValue().EqualTo(true).GetValueAsync();
            while (userTask.IsCompleted == false) {
                yield return null;
            }
            if (userTask.IsFaulted)
            {
                Debug.LogError(userTask.Exception);
            }

            foreach (var entry in userTask.Result.Value as Dictionary<string, object>)
            {
                UpdateRemoteUserList( entry.Key, (bool)entry.Value);
            }
        }

        private void OnLocationChanged(Coordinates newPos) {
            CurrentUserPosition.Coordinates = newPos;
            
            //If more than (i believe) approx. 10 meter difference: update and push to server.
            if (CurrentUserPreviousPosition.IsEmpty || CurrentUserPreviousPosition.Coordinates.DistanceFromOtherGPSCoordinate(newPos) > 0.00001) {
                CurrentUserPreviousPosition.Coordinates = newPos;

                UserDatabase.UpdatePropertyData(UserDatabase.Positions, CurrentUserId, CurrentUserPosition.ToDictionary());
            }
        }


        private void HandleLoggedInChanged(object sender, ChildChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            //Debug.Log(args.Snapshot.Key + " " + args.Snapshot.Value);


            var key = (string) args.Snapshot.Key;
            var value = (bool) args.Snapshot.Value;

            UpdateRemoteUserList(key, value);
        }

        private void UpdateRemoteUserList(string key, bool value)
        {
            if (key == AuthManager.Instance.CurrentUser.UserId)
                return;

            Debug.Log(key + " " + value);

            if (value == true)
            {
                RemoteUser newUser = Instantiate(RemoteUserPrefab);
                UserDatabase.RetrievePropertyData(UserDatabase.Usernames, key).ContinueWith(t => newUser.SetDisplayName((string)t.Result));
                remoteUsers.Add(key, newUser);
                //UserDatabase.Positions.Child(key).ValueChanged += remoteUsers[key].UpdatePosition;
            }
            else
            {
                if (remoteUsers.ContainsKey(key))
                {
                    
                    var remoteUser = remoteUsers[key];
                    //UserDatabase.Positions.Child(key).ValueChanged -= remoteUser.UpdatePosition;
                    remoteUsers.Remove(key);
                    Destroy(remoteUser.gameObject);
                }
            }
        }

        private void HandlePositionChanged (object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            var key = (string) args.Snapshot.Key;

            if (remoteUsers.ContainsKey(key))
            {
                var value = args.Snapshot.Value as Dictionary<string, object>;
                var pos = new Position();
                pos.FromDictionary(value);

                remoteUsers[key].UpdatePosition(pos);
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            // Debug.Log("Game has paused: "+ pauseStatus);

            UserDatabase.UpdatePropertyData(UserDatabase.LoggedIn, AuthManager.Instance.CurrentUser.UserId, !pauseStatus);
            
        }

        void OnDestroy()
        {
            OnApplicationPause(true);

            UserDatabase.Positions.ChildChanged -= HandlePositionChanged;
            UserDatabase.LoggedIn.ChildChanged -= HandleLoggedInChanged;
            remoteUsers.Clear();
        }

    }
}