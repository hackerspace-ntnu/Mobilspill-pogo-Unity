using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasOrienter : MonoBehaviour
{
    // Start is called before the first frame update

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position,Vector3.up);
    }
}
