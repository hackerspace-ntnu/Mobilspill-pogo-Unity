using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    private GameObject Match;
    private int MatchNumber;
    private bool Revealed;
    private bool Complete;
    private int[] Coordinates = new int[2];

    public void setMatch(GameObject Match)
    {
        this.Match = Match;
    }

    public GameObject getMatch()
    {
        return Match;
    }

    public void setMatchNumber(int MatchNumber)
    {
        this.MatchNumber = MatchNumber;
    }

    public int getMatchNumber()
    {
        return MatchNumber;
    }

    public void setRevealed(bool Revealed)
    {
        this.Revealed = Revealed;
    }

    public bool getRevealed()
    {
        return Revealed;
    }

    public void setComplete(bool Complete)
    {
        this.Complete = Complete;

        if(Complete == true)
        {
            float Percentage = ((float)Coordinates[0] + (float)Coordinates[1]) / 8f;
            //print(Percentage);
            GetComponent<Image>().color = GameObject.Find("Engine").GetComponent<Engine>().ColorCalculator(Percentage, 0, 1, 255);
            transform.GetChild(0).gameObject.SetActive(!Complete);
            GetComponent<Button>().enabled = false;
        }
    }

    public int[] getCoordinates()
    {
        return Coordinates;
    }

    public void setCoordinates(int x, int y)
    {
        this.Coordinates = new int[2] { x, y };
    }

    public bool getComplete()
    {
        return Complete;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        GameObject.Find("Engine").GetComponent<Engine>().OnClick(gameObject);
    }

    public void Reveal(bool Reveal)
    {
        transform.GetChild(1).gameObject.SetActive(!Reveal);
        setRevealed(Reveal);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
