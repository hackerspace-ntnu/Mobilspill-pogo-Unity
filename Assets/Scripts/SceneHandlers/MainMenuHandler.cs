using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private void Start() {
        Debug.Log("[MainMenuHandler] Starting main menu");

        _startButton = GameObject.Find("StartGameButton");
        _logOutButton = GameObject.Find("LogOutButton");
        _loadingBar = GameObject.Find("LoadingImage");

        _loadingBar.SetActive(true);
        _startButton.SetActive(false);
        _logOutButton.SetActive(false);

        StartCoroutine(_init());
    }

    private IEnumerator _init() {

        if (!AuthManager.Instance.IsInitialized) {
            Task initAuthTask = AuthManager.Instance.GetAndInitAuthManagerTask();
            //waiting for AuthManager async initiation to complete
            while (!initAuthTask.IsCompleted) yield return null;

           
            if (AuthManager.Instance.Auth.CurrentUser == null) {
                Debug.Log("Loading new scene");
                SceneManager.LoadScene("Assets/Scenes/LoginScene.unity");
            } else {

                if (AuthManager.Instance.FirebaseActive && AuthManager.Instance.Auth.CurrentUser != null) {
                    if (!string.IsNullOrEmpty(AuthManager.Instance.Auth.CurrentUser.UserId)) {
                        if (AuthManager.Instance.CurrentUser != null && AuthManager.Instance.CurrentUser.HasProperValues) {
                            FinishSetup();
                        } /*else {
                            Debug.Log("Getting user..........");                        
                            AuthManager.Instance.GetUserWithAuthId();
                    }   */
                    }
                }

                Debug.Log("Performing startup checks...");

                //TODO: listen to certain authManager changes and toggle on change.
            }


        }
        else {
            Debug.Log("Initialized!!");
            // Loading bar currently not needed - disabling for now.
            _loadingBar.SetActive(false);

            // Setting up button listeners.
            _startButton.GetComponent<Button>().onClick.AddListener(StartGame);
            _logOutButton.GetComponent<Button>().onClick.AddListener(() => { AuthManager.Instance.LogOut(); });

        }
    }

    public void FinishSetup() {
        _properlyLoggedIn = true;
        SetText(_startButton, $"Start Game as {AuthManager.Instance.CurrentUser.DisplayName}");
        
        // Loading bar currently not needed - disabling for now.
        _loadingBar.SetActive(false);
        _startButton.SetActive(true);
        _logOutButton.SetActive(true);

        // Setting up button listeners.
        _startButton.GetComponent<Button>().onClick.AddListener(StartGame);
        _logOutButton.GetComponent<Button>().onClick.AddListener(() => { AuthManager.Instance.LogOut(); });
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
        // If not yet properly logged in (User data not yet retrieved by AuthManager),
        // check if user data retrieved properly every frame and if so, finish setup.
        if (!_properlyLoggedIn && AuthManager.Instance.CurrentUser != null && AuthManager.Instance.CurrentUser.HasProperValues) {
            FinishSetup();
        }
    }

    public void SetText(GameObject button, string newText) {
        button.GetComponentInChildren<Text>().text = newText;
    }
}
