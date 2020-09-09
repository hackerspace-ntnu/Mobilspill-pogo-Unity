using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_tomat : MonoBehaviour
{
    private Vector3 LastMousePosition;
    private int MovementSpeed;
    public Camera Camera;
    public string movementType;


    //TODO Lag tomat left- og right-idle animasjon

    // Start is called before the first frame update
    void Start()
    {
        SetMovementSpeed(3);
        LastMousePosition = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        print(movementType);
        if (Input.GetMouseButton(0))
        {
            //Vector3 newPosition = Input.mousePosition;
            LastMousePosition = Input.mousePosition;
            //transform.position += GetMovementVector()*Time.deltaTime;
            Vector3 newPositionInWorld = Camera.main.ScreenToWorldPoint(LastMousePosition);
             transform.position = Vector2.MoveTowards(transform.position,newPositionInWorld/*Camera.main.ScreenToWorldPoint(LastMousePosition)*/,MovementSpeed*Time.deltaTime);
            movementType = MovementEvaluator(transform.position, LastMousePosition);//transform.position henter posisjonen til dette objektet

        }
        else
        {
            movementType = IdleEvaluator();
        }

        
    }

    private string MovementEvaluator(Vector3 currentPosition, Vector3 newPosition)
    {
        Vector3 vectorSum = newPosition - currentPosition;
        vectorSum.Normalize();
        print(vectorSum);

        if (vectorSum.x > Mathf.Abs(vectorSum.y))
        {
            //transform.position += new Vector3(1, 0, 0) * Time.deltaTime;

            return "right"; //Høyre
        }

        else if (Mathf.Abs(vectorSum.x) > Mathf.Abs(vectorSum.y))
        {
            //transform.position += new Vector3(-1, 0, 0) * Time.deltaTime;

            return "left"; //Venstre
        }
        else if (vectorSum.y > Mathf.Abs(vectorSum.x))
        {
            //transform.position += new Vector3(0, 1, 0) * Time.deltaTime;

            return "up"; //Opp
        }
        else if (Mathf.Abs(vectorSum.y) > Mathf.Abs(vectorSum.x))
        {
            //transform.position += new Vector3(0, -1, 0) * Time.deltaTime;

            return "down"; //Ned
        }

        return null;
    }

    private string IdleEvaluator()
    {
        if(movementType == "right" || movementType == "right_idle")
        {
            return "right_idle";
        }
        else if (movementType == "left" || movementType == "left_idle")
        {
            return "left_idle";
        }
        else if (movementType == "up" || movementType == "up_idle")
        {
            return "up_idle";
        }
        else if (movementType == "down" || movementType == "down_idle")
        {
            return "down_idle";
        }
        return null;
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
