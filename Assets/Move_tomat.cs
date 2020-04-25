using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_tomat : MonoBehaviour
{
    private Vector3 LastMousePosition;
    private int MovementSpeed;
    public Camera Camera;

    // Start is called before the first frame update
    void Start()
    {
        SetMovementSpeed(1);
        LastMousePosition = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
             LastMousePosition = Input.mousePosition;
             //transform.position += GetMovementVector()*Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position,Camera.main.ScreenToWorldPoint(LastMousePosition),MovementSpeed*Time.deltaTime);
            
        }
        
    }

   /* Vector3 GetMovementVector()
    {
        Vector3 vectorSum = Camera.main.ScreenToViewportPoint(LastMousePosition) - this.transform.position;
        print(Camera.main.ScreenToViewportPoint(LastMousePosition));
        print(this.transform.position);
        vectorSum.Normalize();
        return vectorSum*MovementSpeed;
    }*/

    void SetMovementSpeed(int speed) //Hadde kankje vært gøy å gjøre den negativ? Nah
    {
        if (speed > 0 && speed < 5)
        {
            MovementSpeed = speed;
        } 
    }

}
