using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.SceneHandlers {
    public class LoginHandler : MonoBehaviour {

        private GameObject _emailInput;
        private GameObject _passwordInput;
        private GameObject _loginButton;
        private GameObject _registerButton;

        // Start is called before the first frame update.
        void Start(){
            _emailInput = GameObject.Find("EmailField");
            _passwordInput = GameObject.Find("PasswordField");
            _loginButton = GameObject.Find("LoginButton");
            _registerButton = GameObject.Find("RegisterButton");

            Debug.Log("[LoginHandler] Email: " + _emailInput.GetComponent<InputField>().text);
            Debug.Log("[LoginHandler] Pass:  " + _passwordInput.GetComponent<InputField>().text);

            _loginButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                AuthManager.Instance.LoginWithEmail(
                    _emailInput.GetComponent<InputField>().text,
                    _passwordInput.GetComponent<InputField>().text);
            });

            _registerButton.GetComponent<Button>().onClick.AddListener(() => {
                AuthManager.Instance.RegisterWithEmail(
                    _emailInput.GetComponent<InputField>().text,
                    _passwordInput.GetComponent<InputField>().text);
            });
        }

        // Update is called once per frame.
        void Update() {
            // If properly logged in (User data retrieved by AuthManager),
            // Player should be redirected to main menu.
            // This must stay here because SceneManager.LoadScene doesn't work properly from within Continue in tasks.
            if (AuthManager.Instance.CurrentUser != null && AuthManager.Instance.CurrentUser.HasProperValues) {
                SceneManager.LoadScene("Assets/Scenes/Main menu.unity");
            }
        }
    }
}
