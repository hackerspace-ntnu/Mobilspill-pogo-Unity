﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.SceneHandlers {
    public class LoginHandler : MonoBehaviour {

        private GameObject _emailInput;
        private GameObject _passwordInput;
        private GameObject _loginButton;
        private GameObject _registerButton;
        private Text _errorMessageField;

        // Start is called before the first frame update.
        void Start(){
            _emailInput = GameObject.Find("EmailField");
            _passwordInput = GameObject.Find("PasswordField");
            _loginButton = GameObject.Find("LoginButton");
            _registerButton = GameObject.Find("RegisterButton");
            _errorMessageField = GameObject.Find("ErrorMessageField").GetComponent<Text>();

            Debug.Log("[LoginHandler] Email: " + _emailInput.GetComponent<InputField>().text);
            Debug.Log("[LoginHandler] Pass:  " + _passwordInput.GetComponent<InputField>().text);

            _loginButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                StartCoroutine(AuthManager.Instance.LoginWithEmail(
                    _emailInput.GetComponent<InputField>().text,
                    _passwordInput.GetComponent<InputField>().text,
                    _errorMessageField));
            });

            _registerButton.GetComponent<Button>().onClick.AddListener(() => {
                StartCoroutine(AuthManager.Instance.RegisterWithEmail(
                    _emailInput.GetComponent<InputField>().text,
                    _passwordInput.GetComponent<InputField>().text,
                    _errorMessageField));
            });
        }
    }
}
