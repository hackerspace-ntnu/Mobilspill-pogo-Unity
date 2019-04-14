using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;

namespace Assets.Scripts.Firebase {

    public class RealtimeDatabaseManager {

        private static RealtimeDatabaseManager _instance;
        public static RealtimeDatabaseManager Instance {
            get { return _instance ?? (_instance = new RealtimeDatabaseManager()); }
        }

        // Root reference location of the database.
        public DatabaseReference DBReference {get { return FirebaseDatabase.DefaultInstance.RootReference; }
        }
        
        // forward reference for default firebase intance.
        public FirebaseDatabase RealtimeDatabaseInstance {
            get { return FirebaseDatabase.DefaultInstance; }
        }

        private RealtimeDatabaseManager() {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://pogo-firebase.firebaseio.com/");
        }

       
    }
}