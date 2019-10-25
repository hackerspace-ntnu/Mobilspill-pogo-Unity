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


        // Use this for initialization
        IEnumerator Start() {
            //Waiting for the location manager to have the world origin set.
            yield return StartCoroutine(goMap.locationManager.WaitForOriginSet());

            //if user does not exists, throw an error -> should never happen
            if (AuthManager.Instance.CurrentUser == null) {
                Debug.LogError("User should exist when using RealtimePositionHandler");
                Application.Quit();
            } else {
                remoteUsers = new Dictionary<string, RemoteUser>();
		        goMap.locationManager.onLocationChanged.AddListener((Coordinates) => {AuthManager.Instance.CurrentUser.OnLocationChanged(Coordinates);});
                
                //Setting up subscription to positions in all of the user's groups
                //TODO: make sure not to repeat/duplicate users who are in multiple groups.
                
                foreach (string groupId in AuthManager.Instance.CurrentUser.Groups.Values) {
                    Console.WriteLine("Connecting to " + groupId);
                    RealtimeDatabaseManager.Instance.RealtimeDatabaseInstance
                        .GetReference("groups/map/" + groupId + "/protected/members")
                        .ValueChanged += HandlePositionChanged;

                    
                    RealtimeDatabaseManager.Instance.RealtimeDatabaseInstance
                        .GetReference("groups/map/" + groupId + "/protected/members/"+AuthManager.Instance.CurrentUser.UserId+"/position")
                        .ValueChanged += Test;
                }
            }
        }

        void HandlePositionChanged(object sender, ValueChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            if (EditorApplication.isPlaying == false)
            {
                return;
            }
            // Do something with the data in args.Snapshot
            Dictionary<string, object> snapshotVal = args.Snapshot.Value as Dictionary<string, object>;

            if (snapshotVal != null) {
                foreach (KeyValuePair<string, object> entry in snapshotVal) {
                    //Debug.Log("Pos: " + JsonConvert.SerializeObject(snapshotVal));
                    Dictionary<string, object> memberVal = ((Dictionary<string, object>) entry.Value);
                    if(entry.Key != AuthManager.Instance.CurrentUser.UserId){

                        if (true){// || memberVal.ContainsKey("is_active") && ((bool)memberVal["is_active"])) {
                            
                            if ( remoteUsers.ContainsKey(entry.Key) == false)
                            {
                                // Add new remoteuser gameobject
                                RemoteUser newUser = Instantiate(RemoteUserPrefab);

                                Debug.Log(string.Join(", ", memberVal.Keys.Select( key => key.ToString())));
                                if (memberVal.ContainsKey("score"))
                                {
                                    newUser.Initialize(entry.Key);
                                }
                                remoteUsers.Add(entry.Key, newUser);
                            }
                            if (memberVal.ContainsKey("position"))
                            {
                                Dictionary<string, object> posDict = (Dictionary<string, object>) memberVal["position"];
                                Position pos = new Position();
                                pos.FromDictionary(posDict);


                                remoteUsers[entry.Key].UpdatePosition(pos);
                            }
                        }
                        else
                        {
                            //Player is not active on the server

                            if (remoteUsers.ContainsKey(entry.Key))
                            {
                                //Delete the remoteuser gameobject
                                Destroy(remoteUsers[entry.Key]);
                                remoteUsers.Remove(entry.Key);
                            }
                        }
                    }
                }

            }
        }

        public void PushPositionUpdates(User user, Position serverPosition) {
            
        }

        void Test(object sender, ValueChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            if (EditorApplication.isPlaying == false)
            {
                return;
            }
            Dictionary<string, object> posDict = args.Snapshot.Value as Dictionary<string, object>;
            Debug.Log(posDict["lat"].ToString());
        }
        

    }
}