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

    public string CurrentUserID => Instance.Auth.CurrentUser.UserId;

    private RealtimeDatabaseManager _rDBManager;

    private FirebaseApp _app;
    public Firebase.Auth.FirebaseAuth Auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

    public async Task GetAndInitAuthManagerTask() {
        // Checking firebase dependencies.
        var dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
            // Setting a flag here to indicate whether Firebase is ready to use by the app.
            _firebaseActive = true;
            // Setting auth as initialized.
            _isInitialized = true;
        } else {
            Debug.LogError(string.Format(
                "[AuthManager]Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        }
        Debug.Log("[AuthManager] init task completed");
    }

    public async Task RegisterWithEmail(string email, string password, Text errorMessageField) {
        Firebase.Auth.FirebaseUser newUser;
        try {
            newUser = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);
        }
        catch(Exception e)
        {
            errorMessageField.text = e.InnerException.Message;
            return;
        }
        Debug.LogFormat("[AuthManager] Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

        
        if (Auth.CurrentUser != null) {
            //creating user in DB with username (currently just start of email)
            Debug.Log("Current userId: " + CurrentUserID);

            UploadInitialUserData(email.Split('@')[0]);

            Debug.Log("Loading scene Main menu");
            SceneManager.LoadScene("Assets/Scenes/Main menu.unity");
        }
    }
    private void UploadInitialUserData(string username)
    {
        UserDatabase.UpdatePropertyData(UserDatabase.Usernames, CurrentUserID, username);
        UserDatabase.UpdatePropertyData(UserDatabase.Score, CurrentUserID, 0);
        UserDatabase.UpdatePropertyData(UserDatabase.Positions, CurrentUserID, new Position(0,0,0));
        UserDatabase.UpdatePropertyData(UserDatabase.LoggedIn, CurrentUserID, false);

    }


    public async Task LoginWithEmail(string email, string password, Text errorMessageField) {
        Firebase.Auth.FirebaseUser newUser;
        try {
            newUser = await  Auth.SignInWithEmailAndPasswordAsync(email, password);
        }
        catch(Exception e)
        {
            errorMessageField.text = e.InnerException.Message;
            return;
        }
        
        Debug.LogFormat("[AuthManager] User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);

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
