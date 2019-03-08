using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour {

    private GameObject _startButton;
    private GameObject _logOutButton;
    private GameObject _loadingBar;

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

        if (authManager.Auth.CurrentUser != null) {
            SetText(_startButton, string.Format("Start Game as {0}", authManager.Auth.CurrentUser.DisplayName));
        }

        Debug.Log("Performing startup checks...");

        //TODO: listen to certain authManager changes and toggle on change.
        //ToggleVisibleElements();
    }

    public void StartGame() {
        // If Firebase is active and user exists.
        if (AuthManager.Instance.FirebaseActive
            && AuthManager.Instance.Auth != null) {
            SceneManager.LoadScene("Assets/Scenes/Flat Map - Pokèmon GO Style.unity");
        } else {
            UnityEngine.Debug.LogError("ERROR -- Should not have start game option when not logged in.");
            // Loading login screen.
            SceneManager.LoadScene("");
        }
    }

    public void SetText(GameObject button, string newText) {
        button.GetComponentInChildren<Text>().text = newText;
    }

    public void ToggleVisibleElements() {
        if (AuthManager.Instance.Auth == null) {
            _startButton.SetActive(false);
            _logOutButton.SetActive(false);
            _loadingBar.SetActive(true);
        } else {
            _startButton.SetActive(true);
            _logOutButton.SetActive(true);
            _loadingBar.SetActive(false);
        }
    }

    // Update is called once per frame.
    void Update() {}
}
