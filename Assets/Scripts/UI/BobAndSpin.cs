using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobAndSpin : MonoBehaviour
{
    public float bob_speed = 5;
    public float bob_amplitude = 5;
    public float bob_height = 10;
    public float spin_speed = 3;

    private float time = 0;

    void Update()
    {
        time += Time.deltaTime;

        transform.rotation = Quaternion.Euler(0,time*spin_speed,0);

        var pos = transform.position;

        pos.y = bob_height + bob_amplitude * Mathf.Sin(bob_speed*time);

        transform.position = pos;

    }
}
