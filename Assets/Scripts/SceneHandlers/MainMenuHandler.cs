using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Firebase;
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
    private async void Start() {
        Debug.Log("[MainMenuHandler] Starting main menu");

        _startButton = GameObject.Find("StartGameButton");
        _logOutButton = GameObject.Find("LogOutButton");
        _loadingBar = GameObject.Find("LoadingImage");

        _loadingBar.SetActive(true);
        _startButton.SetActive(false);
        _logOutButton.SetActive(false);

        if (!AuthManager.Instance.IsInitialized) {
            await AuthManager.Instance.GetAndInitAuthManagerTask();

           
            if (AuthManager.Instance.Auth.CurrentUser == null) {
                Debug.Log("Loading new scene");
                SceneManager.LoadScene("Assets/Scenes/LoginScene.unity");
            } else {

                if (AuthManager.Instance.FirebaseActive && AuthManager.Instance.Auth.CurrentUser != null) {
                    if (!string.IsNullOrEmpty(AuthManager.Instance.Auth.CurrentUser.UserId)) {
                        await FinishSetup();
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
            await FinishSetup();
        }
    }

    public async Task FinishSetup() {
        _properlyLoggedIn = true;

        var snapshot = await UserDatabase.RetrievePropertyData(UserDatabase.Usernames, AuthManager.Instance.CurrentUserID);


        SetText(_startButton, $"Start Game as {snapshot.Value.ToString()}");
        
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

    public void SetText(GameObject button, string newText) {
        button.GetComponentInChildren<Text>().text = newText;
    }
}
