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


    private bool _isInitialized = false;
    public bool IsInitialized => _isInitialized;

    private static AuthManager _instance;
    public static AuthManager Instance => _instance ?? new AuthManager();

    private bool _firebaseActive = false;
    public bool FirebaseActive => _firebaseActive;

    public UserData CurrentUser {get; private set;}

    private RealtimeDatabaseManager _rDBManager;

    private FirebaseApp _app;
    public Firebase.Auth.FirebaseAuth Auth = Firebase.Auth.FirebaseAuth.DefaultInstance;


    public Task GetAndInitAuthManagerTask() {
        return Task.Run(async() =>
        {
            // Checking firebase dependencies.
            var dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Setting a flag here to indicate whether Firebase is ready to use by the app.
                _firebaseActive = true;
                // Setting auth as initialized.
                _isInitialized = true;

                if (Auth.CurrentUser != null) {
                    CurrentUser = await UserDatabase.RetrieveUserData(Auth.CurrentUser.UserId);
                    Debug.Log("[AuthManager] Retrieved user data.");
                }
            } else {
                Debug.LogError(string.Format(
                    "[AuthManager]Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
            Debug.Log("[AuthManager] init task completed");
        });
    }

    public IEnumerator RegisterWithEmail(string email, string password) {
        var t1 = Auth.CreateUserWithEmailAndPasswordAsync(email, password);
        
        while (t1.IsCompleted == false)
        {
            yield return null;
        }
        if (t1.IsCanceled) {
            Debug.LogError("[AuthManager] Create UserWithEmailAndPasswordAsync was canceled.");
            yield break;
        }
        if (t1.IsFaulted) {
            Debug.LogError("[AuthManager] CreateUserWithEmailAndPasswordAsync encountered an error: " + t1.Exception);
            yield break;
        }
        Firebase.Auth.FirebaseUser newUser = t1.Result;
        Debug.LogFormat("[AuthManager] Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

        
        if (Auth.CurrentUser != null) {
            //creating user in DB with username (currently just start of email)
            CurrentUser = new UserData{
                UserId = Auth.CurrentUser.UserId,
                Username = email.Split('@')[0],
                Score = 0
            };


            Debug.Log("Current userId: " + Auth.CurrentUser.UserId);

            var t2 = UserDatabase.AddUser(CurrentUser);

            while (t2.IsCompleted == false)
            {
                yield return null;
            }
            if (t2.IsFaulted) {
                Debug.LogWarning("[AuthManager] Add user to database encountered an error: " + t2.Exception);
                yield break;
            }


            Debug.Log("Loading scene Main menu");
            SceneManager.LoadScene("Assets/Scenes/Main menu.unity");
        }
    }


    public IEnumerator LoginWithEmail(string email, string password) {
        var task = Auth.SignInWithEmailAndPasswordAsync(email, password);
        while (!task.IsCompleted)
        {
            yield return null;
        }
        
        if (task.IsCanceled) {
            Debug.LogError("SignInWithEmailAndPasswordAsync was cancelled.");
            yield break;
        }
        if (task.IsFaulted) {
            Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
            yield break;
        }

        var newUser = task.Result;
        Debug.LogFormat("[AuthManager] User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);

        var task2 = UserDatabase.RetrieveUserData(Auth.CurrentUser.UserId);
        while (!task2.IsCompleted)
        {
            yield return null;
        }
        if (task2.IsFaulted)
        {
            Debug.LogWarning(task2.Exception);
        }
        else{
            CurrentUser = task2.Result;
        }

        Debug.Log("Loading main menu scene");
        SceneManager.LoadScene("Assets/Scenes/Main menu.unity");
        
    }
    public void LogOut() {
        Debug.Log("[AuthManager] User signing out");
        Auth.SignOut();
        if (Auth.CurrentUser == null) {
            SceneManager.LoadScene("Assets/Scenes/LoginScene.unity");
        }
    }
}
