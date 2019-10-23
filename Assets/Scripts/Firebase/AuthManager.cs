using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Firebase;
using Assets.Scripts.Models;
using Firebase;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class AuthManager {

    private AuthManager() {
        _instance = this;
        _rDBManager = RealtimeDatabaseManager.Instance;
    }

    public Task GetAndInitAuthManagerTask() {
        return Task.Run(() =>
        {
            // Checking firebase dependencies.
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available) {
                    // Setting a flag here to indicate whether Firebase is ready to use by the app.
                    _firebaseActive = true;
                    // Setting auth as initialized.
                    _isInitialized = true;

                    // If user is not logged in, he is redirected to LoginScene.
                    if (_instance.Auth.CurrentUser != null) {
                        _getUserWithAuthId(Auth.CurrentUser.UserId).ContinueWith(t =>
                        {
                            Debug.Log("Value with AuthId: " + t.Result.GetRawJsonValue());
                            _isInitialized = true;

                            Dictionary<string, object> snapshotVal = t.Result.Value as Dictionary<string, object>;
                            if (snapshotVal.ContainsKey("groups") && snapshotVal["groups"].GetType() == typeof(Dictionary<string, System.Object>)) {
                                Debug.Log("[AuthManager] Creating user with groups");
                                Debug.Log(snapshotVal["groups"] + " -- " + snapshotVal["groups"].GetType());
                                _currentUser = new User((string)snapshotVal["displayname"], Auth.CurrentUser.UserId,
                                    (Dictionary<string, System.Object>)snapshotVal["groups"]);
                            } else {
                                Debug.Log("[AuthManager] Creating user without groups");
                                _currentUser = new User((string)snapshotVal["displayname"], Auth.CurrentUser.UserId);
                            }

                        });
                    }

                } else {
                    Debug.LogError(string.Format(
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            }).ContinueWith(t => { Debug.Log("done"); });
        });
    }


    private bool _isInitialized = false;
    public bool IsInitialized => _isInitialized;

    private static AuthManager _instance;
    public static AuthManager Instance => _instance ?? new AuthManager();

    private bool _firebaseActive = false;
    public bool FirebaseActive => _firebaseActive;

    private User _currentUser;
    public User CurrentUser => _currentUser;

    private RealtimeDatabaseManager _rDBManager;

    private FirebaseApp _app;
    public Firebase.Auth.FirebaseAuth Auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    public Dictionary<string, Firebase.Auth.FirebaseUser> UserByAuth = new Dictionary<string, Firebase.Auth.FirebaseUser>();

    public void RegisterWithEmail(string email, string password) {
        Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("[AuthManager] Create    UserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("[AuthManager] CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            UserByAuth.Add(newUser.DisplayName, newUser);

            Debug.LogFormat("[AuthManager] Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            if (Auth.CurrentUser != null) {
                //creating user in DB with username (currently just start of email)
                _currentUser = new User(email.Split('@')[0], Auth.CurrentUser.UserId, true);
                Debug.Log("Current userId: " + Auth.CurrentUser);

                _addUserToRDB(_currentUser).ContinueWith(t => {

                    if (task.IsCanceled) {
                        Debug.LogError("[AuthManager] AddUserToDb was canceled.");
                        return;
                    }
                    if (t.IsFaulted) {
                        Debug.LogError("[AuthManager] AddUserToDb encountered an error: " + task.Exception.ToString());
                        return;
                    }
                    //everything worked -> we can redirect to main menu. 
                    SceneManager.LoadScene("Assets/Scenes/Main menu.unity");
                });

            }
        });
    }

    private Task _addUserToRDB(User user) {


        Dictionary<string, System.Object> createUserDict = new Dictionary<string, System.Object>();
        createUserDict["/usernames/" + user.UserName] = user.UserId;
        createUserDict["/users/" + user.UserId] = user.ToDictionary();

        Debug.Log("[AuthManager] user json: " + JsonConvert.SerializeObject(createUserDict));
    
        return RealtimeDatabaseManager.Instance.DBReference.UpdateChildrenAsync(createUserDict);
    }

    private Task<DataSnapshot> _getUserWithAuthId(string AuthId) {
        Debug.Log("[AuthManager] retrieving user info with id: " + AuthId);
        return RealtimeDatabaseManager.Instance.RealtimeDatabaseInstance
            .GetReference("users/" + AuthId)
            .GetValueAsync();
    }

    public Task<DataSnapshot> GetUserGroupMembership(string groupId) {
        return RealtimeDatabaseManager.Instance.RealtimeDatabaseInstance
            .GetReference("groups/map/" + groupId + "/protected/members/" + Auth.CurrentUser.UserId)
            .GetValueAsync();
    }

    public void LoginWithEmail(string email, string password) {
        Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            UserByAuth.Add(newUser.DisplayName, newUser);

            Debug.LogFormat("[AuthManager] User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            if (Auth.CurrentUser != null) {
                GetUserWithAuthId();
            }
        });
    }

    public void GetUserWithAuthId() {
        GetUserWithAuthId(Auth.CurrentUser.UserId);
    }

    public void GetUserWithAuthId(string AuthId) {
        _getUserWithAuthId(Auth.CurrentUser.UserId).ContinueWith(t => {
            if (t.IsFaulted) {
                // Handle the error...
                Debug.Log("[AuthManager] Failed getting user: " + t.Exception);
            } else if (t.IsCompleted) {
                if (t.Result.Value is Dictionary<string, object> snapshotVal) {
                    Debug.Log("[AuthManager] Creating user from: " + JsonConvert.SerializeObject(snapshotVal));
                    // Setting _currentUser to the snapshot, then returning to Main menu.
                    if (snapshotVal.ContainsKey("groups") && snapshotVal["groups"].GetType() == typeof(Dictionary<string, System.Object>)) {
                        Debug.Log("[AuthManager] Creating user with groups");
                        Debug.Log(snapshotVal["groups"] + " -- " + snapshotVal["groups"].GetType());
                        _currentUser = new User((string)snapshotVal["displayname"], Auth.CurrentUser.UserId,
                            (Dictionary<string, System.Object>)snapshotVal["groups"]);
                    } else {
                        Debug.Log("[AuthManager] Creating user without groups");
                        _currentUser = new User((string)snapshotVal["displayname"], Auth.CurrentUser.UserId);
                    }


                    _currentUser.TotalScore = (int)(long)snapshotVal["total_score"];
                    _currentUser.UserSince = (long)snapshotVal["user_since"];
                    //_currentUser.Groups = (List<>)

                    Debug.Log("[AuthManager] user values: " + JsonConvert.SerializeObject(snapshotVal));
                    Debug.Log("Loading main menu scene");

                    //SceneManager.LoadScene("Assets/Scenes/Main menu.unity");
                    //SceneManager.LoadScene("Assets/Scenes/PoGo.unity");
                }
            }
        });
    }

    public void LogOut() {
        Debug.Log("[AuthManager] User signing out");
        Auth.SignOut();
        UserByAuth.Clear();
        _currentUser = null;
        if (Auth.CurrentUser == null) {
            SceneManager.LoadScene("Assets/Scenes/LoginScene.unity");
        }
    }
}
