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
using UnityEngine;

namespace Assets.Scripts.Firebase {

    public class RealtimePositionHandler : MonoBehaviour {

        public GOMap goMap;

        public Material testLineMaterial;
        public Material testPolygonMaterial;
        public GOUVMappingStyle uvMappingStyle = GOUVMappingStyle.TopFitSidesRatio;


        // Use this for initialization
        IEnumerator Start() {
            //Waiting for the location manager to have the world origin set.
            yield return StartCoroutine(goMap.locationManager.WaitForOriginSet());

            //if user does not exists, throw an error -> should never happen
            if (AuthManager.Instance.CurrentUser == null) {
                Debug.LogError("User should exist when using RealtimePositionHandler");
                Application.Quit();
            } else {
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
            // Do something with the data in args.Snapshot
            Dictionary<string, object> snapshotVal = args.Snapshot.Value as Dictionary<string, object>;

            if (snapshotVal != null) {
                foreach (KeyValuePair<string, object> entry in snapshotVal) {
                    Debug.Log("Pos: " + JsonConvert.SerializeObject(snapshotVal));
                    Dictionary<string, object> memberVal = ((Dictionary<string, object>) entry.Value);
                    if(entry.Key != AuthManager.Instance.CurrentUser.UserId && memberVal.ContainsKey("position")){
                        Dictionary<string, object> posDict = (Dictionary<string, object>) memberVal["position"];
                        Position pos = new Position(
                            _objectToDouble(posDict["lat"]), 
                            _objectToDouble(posDict["lng"]),
                            _objectToDouble(posDict["alt"]));

                        Debug.Log("Position element: " + entry.Key + ": " + pos);
                        dropPin(pos);
                    }
                }
                Debug.Log(snapshotVal);

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

        void dropPin(Position pos) {
            dropPin(pos.Latitude, pos.Longitude);
        }

        void dropPin(double lng, double lat){
            //1) create game object (you can instantiate a prefab instead)
            GameObject aBigRedSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            aBigRedSphere.transform.localScale = new Vector3(10, 10, 10);
            aBigRedSphere.GetComponent<MeshRenderer>().material.color = Color.green;


            //2) make a Coordinate class with your desired latitude longitude
            //CHANGED TO GLØS COORDINATES
            Coordinates coordinates = new Coordinates(lng, lat);

            //3) call drop pin passing the coordinates and your gameobject
            goMap.dropPin(coordinates, aBigRedSphere);

        }
        
        void dropPolygon(){
            
            //Drop polygon is very similar to the drop line example, just make sure the coordinates will form a closed shape. 

            //1) Create a list of coordinates that will represent the polygon
            List<Coordinates> shape = new List<Coordinates>();
            shape.Add(new Coordinates(48.8744621276855, 2.29504323005676));
            shape.Add(new Coordinates(48.8744010925293, 2.29542183876038));
            shape.Add(new Coordinates(48.8747596740723, 2.29568862915039));
            shape.Add(new Coordinates(48.8748931884766, 2.29534268379211));
            shape.Add(new Coordinates(48.8748245239258, 2.29496765136719));

            //2) Set the line height
            float height = 20;

            //3) Choose a material for the line (this time we link the material from the inspector)
            Material material = testPolygonMaterial;

            //4) call drop line
            goMap.dropPolygon(shape, height, material, uvMappingStyle);

        }

    }
}