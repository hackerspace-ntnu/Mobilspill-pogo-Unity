using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Finn ut hvordan å sette den til å ta inn piltaster

public class SwipeDirCheck : MonoBehaviour
{
    //private float mouseTimeDown;
    private Vector3 firstMousePosition;
    // private Vector3 lastMousePosition;

    public int getDir()
    {

        if (Input.GetMouseButtonDown(0))
        {
            //mouseTimeDown = 0;
            //lastMousePosition = Input.mousePosition;
            firstMousePosition = Input.mousePosition;
     
        }

        if (Input.GetMouseButton(0) || Input.anyKey)
        {
            //mouseTimeDown += Time.deltaTime;
            //if (Input.mousePosition.x > lastMousePosition.x && Input.mousePosition.x > firstMousePosition.x+100f)

            Vector2 vectorSum = Input.mousePosition - firstMousePosition;
            vectorSum.Normalize();

            //lastMousePosition = Input.mousePosition;

            if (vectorSum.x > Mathf.Abs(vectorSum.y) || Input.GetKey(KeyCode.RightArrow))
            {
                //transform.position += new Vector3(1, 0, 0) * Time.deltaTime;
                
                return 1; //Høyre
            }

            else if (Mathf.Abs(vectorSum.x) > Mathf.Abs(vectorSum.y) || Input.GetKey(KeyCode.LeftArrow))
            {
                //transform.position += new Vector3(-1, 0, 0) * Time.deltaTime;
             
                return 3; //Venstre
            }
            else if (vectorSum.y > Mathf.Abs(vectorSum.x) || Input.GetKey(KeyCode.UpArrow))
            {
                //transform.position += new Vector3(0, 1, 0) * Time.deltaTime;
           
                return 2; //Opp
            }
            else if (Mathf.Abs(vectorSum.y) > Mathf.Abs(vectorSum.x) || Input.GetKey(KeyCode.DownArrow))
            {
                //transform.position += new Vector3(0, -1, 0) * Time.deltaTime;
               
                return 0; //Ned
            }

        }
        return -1;
    }



    // print(Input.mousePosition.x);

    /*if (Input.GetKey(KeyCode.D))
    {

        //gameObject.GetComponent<Transform>().position
        GameObject.Find("Test").GetComponent<Transform>().position += new Vector3(1, 0, 0) * Time.deltaTime;
    } */
}

    


