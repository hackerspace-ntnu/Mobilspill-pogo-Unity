using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Firebase;
using Firebase;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeamSelectorHandler : MonoBehaviour {
    public Button teamOneButton;
    public Button teamTwoButton;

    public Text teamOneButtonLabel;
    public Text teamTwoButtonLabel;

    private static DatabaseReference database;

    private async void Start() 
    {
        database = RealtimeDatabaseManager.Instance.DBReference;

        var teamComps = database.Child("team_comps");

        var t = await teamComps.GetValueAsync(); 
        string json = t.GetRawJsonValue();
        var snapshot = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        Debug.LogWarning(json);
        var currentIndex = snapshot["current_index"] as string;
        Debug.LogWarning("Got here");
        var teams = snapshot[currentIndex].Split(',');
        Debug.LogWarning("Got here 2");

        // teamOneButton.GetComponent<Text>().text = teams[0];
        // teamTwoButton.GetComponent<Text>().text = teams[1];
        teamOneButtonLabel.text = teams[0];
        teamTwoButtonLabel.text = teams[1];

        Debug.LogWarning("Got here 3");


        teamOneButton.onClick.AddListener(() => SelectTeam(0));
        teamTwoButton.onClick.AddListener(() => SelectTeam(1));
    }

    private async void SelectTeam(int index)
    {
        UserDatabase.UpdatePropertyData(UserDatabase.TeamIndex, AuthManager.Instance.CurrentUserID, index);

        SceneManager.LoadScene("Assets/Scenes/PoGo.unity");

    }
}