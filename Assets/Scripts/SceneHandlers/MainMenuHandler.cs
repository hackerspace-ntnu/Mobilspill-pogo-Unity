using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour {

    private GameObject _startButton;
    private GameObject _logOutButton;
    private GameObject _loadingBar;

    //properly logged in means we have both the db user values and the auth
    private bool _properlyLoggedIn = false;
    // Start is called before the first frame update.
    void Start() {
        AuthManager authManager = AuthManager.Instance;
        _startButton = GameObject.Find("StartGameButton");
        _logOutButton = GameObject.Find("LogOutButton");
        _loadingBar = GameObject.Find("LoadingImage");
        // Loading bar currently not needed - disabling for now.
        _loadingBar.SetActive(false);

        // Setting up button listeners.
        _startButton.GetComponent<Button>().onClick.AddListener(StartGame);
        _logOutButton.GetComponent<Button>().onClick.AddListener(() => {
            AuthManager.Instance.LogOut();
        });


        if (authManager.FirebaseActive 
            && authManager.Auth.CurrentUser != null) {
            if (authManager.CurrentUser != null) {
                _properlyLoggedIn = true;
                SetText(_startButton, string.Format("Start Game as {0}", authManager.CurrentUser.DisplayName));
            } else {
                _toggleVisibleElements();
            }
        } 

        Debug.Log("Performing startup checks...");

        //TODO: listen to certain authManager changes and toggle on change.
        //_toggleVisibleElements();
    }

    public void StartGame() {
        // If Firebase is active and user exists.
        if (AuthManager.Instance.FirebaseActive
            && AuthManager.Instance.Auth != null) {
            SceneManager.LoadScene("Assets/Scenes/PoGo.unity");
        } else {
            UnityEngine.Debug.LogError("ERROR -- Should not have start game option when not logged in.");
            
            // Loading login screen.
            SceneManager.LoadScene("Assets/Scenes/LoginScene.unity");
        }
    }

    // Update is called once per frame
    void Update() {
        if (!_properlyLoggedIn && AuthManager.Instance.CurrentUser != null) {
            _properlyLoggedIn = true;
            _toggleVisibleElements();
            SetText(_startButton, string.Format("Start Game as {0}", AuthManager.Instance.CurrentUser.DisplayName));
        }
    }

    public void SetText(GameObject button, string newText) {
        button.GetComponentInChildren<Text>().text = newText;
    }

    private void _toggleVisibleElements() {
        if (_properlyLoggedIn) {
            _startButton.SetActive(false);
            _logOutButton.SetActive(false);
            _loadingBar.SetActive(true);
        } else {
            _startButton.SetActive(true);
            _logOutButton.SetActive(true);
            _loadingBar.SetActive(false);
        }
    }
    
}
