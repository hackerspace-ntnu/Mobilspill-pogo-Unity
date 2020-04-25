using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseCleanup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SendIt());
    }
    IEnumerator SendIt() {
        UnityWebRequest www = UnityWebRequest.Get("https://us-central1-pogo-65145.cloudfunctions.net/updateDatabase");
	yield return www.SendWebRequest();
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log(www.downloadHandler.text);
        }
    }

}
