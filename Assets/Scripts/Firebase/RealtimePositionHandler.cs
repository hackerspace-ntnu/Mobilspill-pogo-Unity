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
using System.Threading.Tasks;

namespace Assets.Scripts.Firebase {

    public class RealtimePositionHandler : MonoBehaviour {

        public GOMap goMap;

        public RemoteUser RemoteUserPrefab;

        private Dictionary<string, RemoteUser> remoteUsers;

        private Position CurrentUserPosition = new Position();
        private Position CurrentUserPreviousPosition = new Position();

        private List<string> LoggedInPlayers = new List<string>();

        private string CurrentUserId;


        IEnumerator Start() {
            //Waiting for the location manager to have the world origin set.
            yield return StartCoroutine(goMap.locationManager.WaitForOriginSet());

            

            remoteUsers = new Dictionary<string, RemoteUser>();

            goMap.locationManager.onLocationChanged.AddListener(coords => {OnLocationChanged(coords);});
            UserDatabase.Positions.ChildChanged += HandlePositionChanged;
            UserDatabase.LoggedIn.ChildChanged += HandleLoggedInChanged;

            CurrentUserId = AuthManager.Instance.CurrentUserID;

            if (string.IsNullOrEmpty(CurrentUserId))
            
            {
                Debug.LogError("[RealtimePositionHandler] UserId should not be null!!!");
                AuthManager.Instance.LogOut();
            }
            OnApplicationPause(false);

            yield return UtilityFunctions.RunTaskAsCoroutine(InitializeData());
        }

        private async Task InitializeData()
        {
            var snapshot = await UserDatabase.RetrievePropertyData(UserDatabase.Positions, CurrentUserId);
            
            string json = snapshot.GetRawJsonValue();
            var positionData = JsonConvert.DeserializeObject<Position>(json);
            CurrentUserPosition = positionData;
            CurrentUserPreviousPosition = positionData;


            var loggedInPlayers = await UserDatabase.LoggedIn.OrderByValue().EqualTo(true).GetValueAsync();

            foreach (var entry in loggedInPlayers.Value as Dictionary<string, object>)
            {
                UpdateRemoteUserList( entry.Key, (bool)entry.Value);
            }
        }

        private void OnLocationChanged(Coordinates newPos) {
            CurrentUserPosition.Coordinates = newPos;
            
            //If more than (i believe) approx. 10 meter difference: update and push to server.
            if (CurrentUserPreviousPosition.Coordinates.DistanceFromOtherGPSCoordinate(newPos) > 0.00001) {
                CurrentUserPreviousPosition.Coordinates = newPos;

                UserDatabase.UpdatePropertyData(UserDatabase.Positions, CurrentUserId, CurrentUserPosition);
            }
        }


        private void HandleLoggedInChanged(object sender, ChildChangedEventArgs args) {
            #if UNITY_EDITOR
            if (EditorApplication.isPlaying == false)
            {
                return;
            }
            #endif
            
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            if (gameObject.activeInHierarchy == false)
            {
                return;
            }
            //Debug.Log(args.Snapshot.Key + " " + args.Snapshot.Value);


            var key = (string) args.Snapshot.Key;
            var value = (bool) args.Snapshot.Value;

            UpdateRemoteUserList(key, value);
        }

        private void UpdateRemoteUserList(string key, bool value)
        {
            if (key == AuthManager.Instance.CurrentUserID)
                return;

            //Debug.Log(key + " " + value);

            if (value == true)
            {
                RemoteUser newUser = Instantiate(RemoteUserPrefab);
                UserDatabase.RetrievePropertyData(UserDatabase.Usernames, key).ContinueWith(t => newUser.SetDisplayName((string)t.Result.Value));
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
            if (gameObject.activeInHierarchy == false)
            {
                return;
            }
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            var key = (string) args.Snapshot.Key;

            if (remoteUsers.ContainsKey(key))
            {
                var value = args.Snapshot.GetRawJsonValue();
                var pos = JsonConvert.DeserializeObject<Position>(value);

                remoteUsers[key].UpdatePosition(pos);
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            // Debug.Log("Game has paused: "+ pauseStatus);
            UserDatabase.UpdatePropertyData(UserDatabase.LoggedIn, AuthManager.Instance.CurrentUserID, !pauseStatus);
        }

        void OnDestroy()
        {

            UserDatabase.Positions.ChildChanged -= HandlePositionChanged;
            UserDatabase.LoggedIn.ChildChanged -= HandleLoggedInChanged;
            
            //Very important that OnApplicationPause is called after unsubscribing to the LoggedIn event. Otherwise crash! -Thomas Thrane 10.03.2020
            OnApplicationPause(true);

            if (remoteUsers != null)
                remoteUsers.Clear();
        }

    }
}