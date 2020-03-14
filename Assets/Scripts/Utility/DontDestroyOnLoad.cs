using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public GameObject [] prefabsToNotDestroy;
    private static bool hasSpawned = false;
    // Start is called before the first frame update

    void Awake()
    {
        if (hasSpawned == false)
        {
            foreach(var prefab in prefabsToNotDestroy)
            {
                var go = Instantiate(prefab);
                DontDestroyOnLoad(go);
            }
            hasSpawned = true;
        }  
    }
}
