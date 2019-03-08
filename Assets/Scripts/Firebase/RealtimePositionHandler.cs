using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using GoMap;
using GoShared;
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
            dropPin(63.418148, 10.403391);
            

        }

        void dropPin(double lng, double lat){
            //1) create game object (you can instantiate a prefab instead)
            GameObject aBigRedSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            aBigRedSphere.transform.localScale = new Vector3(10, 10, 10);
            aBigRedSphere.GetComponent<MeshRenderer>().material.color = Color.red;


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