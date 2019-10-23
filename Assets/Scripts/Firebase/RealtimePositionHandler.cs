﻿using System;
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

                        if (true){// || memberVal.ContainsKey("isactive") && ((bool)memberVal["isactive"])) {
                            
                            if ( remoteUsers.ContainsKey(entry.Key) == false)
                            {
                                // Add new remoteuser gameobject
                                RemoteUser newUser = Instantiate(RemoteUserPrefab);
                                remoteUsers.Add(entry.Key, newUser);
                            }
                            if (memberVal.ContainsKey("position"))
                            {
                                Dictionary<string, object> posDict = (Dictionary<string, object>) memberVal["position"];
                                Position pos = new Position(
                                    _objectToDouble(posDict["lat"]), 
                                    _objectToDouble(posDict["lng"]),
                                    _objectToDouble(posDict["alt"]));


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

        private double _objectToDouble(object o) {
            double d = 0d;
            IConvertible convert = o as IConvertible;

            if (convert != null) {
                d = convert.ToDouble(null);
            } 
            return d;
        }

        public void PushPositionUpdates(User user, Position serverPosition) {
            
        }

        

    }
}