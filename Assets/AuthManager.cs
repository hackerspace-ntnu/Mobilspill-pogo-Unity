using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class AuthManager {

    private AuthManager() {
        _instance = this;

        // Checking firebase dependencies.
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Set a flag here to indicate whether Firebase is ready to use by the app.
                _firebaseActive = true;
                // Initializing auth.
                this.Auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                // If user is not logged in, he is redirected to LoginScene.
                if (this.Auth.CurrentUser == null) {
                    SceneManager.LoadScene("Assets/Scenes/LoginScene.unity");
                }
            } else {
                Debug.LogError(string.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private static AuthManager _instance;
    public static AuthManager Instance {
        get { return _instance ?? new AuthManager(); }
    }

    private bool _firebaseActive = false;
    public bool FirebaseActive {
        get { return _firebaseActive; }
    }

    private FirebaseApp _app;
    public Firebase.Auth.FirebaseAuth Auth;
    public Dictionary<string, Firebase.Auth.FirebaseUser> UserByAuth = new Dictionary<string, Firebase.Auth.FirebaseUser>();

    public void RegisterWithEmail(string email, string password) {
        Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("[AuthManager] CreateUserWithEmailAndPasswordAsync was canceled.");
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
                SceneManager.LoadScene("Assets/Scenes/Main menu.unity");
            }
        });
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
                SceneManager.LoadScene("Assets/Scenes/Main menu.unity");
            }
        });
    }

    public void LogOut() {
        Debug.Log("[AuthManager] User signing out");
        Auth.SignOut();
        UserByAuth.Clear();
        if (Auth.CurrentUser == null) {
            SceneManager.LoadScene("Assets/Scenes/LoginScene.unity");
        }
    }
}
