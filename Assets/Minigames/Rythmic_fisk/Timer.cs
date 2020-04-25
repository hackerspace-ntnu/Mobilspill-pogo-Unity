using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;


//Til neste gang: Sett en restriction på antall piler, så man ikke får uendelig score (gjort)
//Finn ut hvordan å gi bedre feedback 
//vite når man har failet
//Hvis to piler er oppå hverandre vil man ikke se når den underste skifter farge. Fiks. (gjort)
//Ca. maxgrense på score
//Muligens legg til fisk
//Gjør om til knapper?
//Gi bedre signal før pilene dukker opp?
//Lag combometode (gjort)
//Fjer scoremultiplier når vi viser end screen (gjort)
//Få arrows til å spawne mellom beat også etterhvert
//Fikse bug som gjør at man får fail på første arrow
//Avrunde score som vises på skjermen
//Kankje balansere scorene litt bedre
//Skaff musikk
//Animer tomat
//Synk opp bedre med musikken

public class Timer : MonoBehaviour //MonoBehavior betyr basicly at jeg kan gjøre Unity-greier med klassen, som f.eks. Deltatime
{
    float time; //Selve tiden
    List<Arrow> arrowList = new List<Arrow>();//Liste som inneholder alle piler på skjermen
    public GameObject arrowPref; //Klonene av piler som dukker opp
    public GameObject circle; //Klonene av sirkler
    private float beatPerSecond; // Bestemmer hvor lenge pilene skal være på skjermen
    private float beatTimer; //Teller fra 0 til 1/beatperSecond
    private float SwipeSpace; // Tidsrom det er akseptabelt å swipe
    private float score;
    private float pointMultiplier;
    private float randomness; //Går fra 0 til 1 og bestemmer om pil dukker opp senere i spillet
    private float comboMultiplier;
    private bool newSwipe;
    private int numSpawnedArrows; //Antall arrows som totalt har spawnet i løpet av spillet
    public GameObject scoreTracker;
    public GameObject pointMultiplierTracker;
    public GameObject failText;
    public GameObject comboText;
    private int combo;
    private float baseBeat; //Standard beat som hastighet går ut fra
    //private SwipeDirCheck swipeDirCheck = new SwipeDirCheck();
    private string[] rythm;
    private int beatTracker; //Holder styr på hvor mange beats det har gått
    private int rythmIndex; //Index til det gjeldende elemnetet i rythm
    private List<float> beatMoments; //Tidspunkt det skal skje beats


    // Start is called before the first frame update
    void Start()
    {
        //baseBeat = 1f;
        //SetBeat(baseBeat); // Setter beatPerSecond
        //SetBeat(baseBeat*2f); // Test
        pointMultiplier = 1;//100f/695f;
        numSpawnedArrows = 0;
        comboMultiplier = 1;
        combo = 0;
        rythmIndex = 0;
        newSwipe = true;
        beatMoments = new List<float>();
        GameObject.Find("EndScreen").GetComponent<Canvas>().enabled = false;
        rythm = System.IO.File.ReadAllLines(Path.GetFullPath(Environment.CurrentDirectory+ "/Assets/Minigames/Rythmic_fisk/beatsouls_melody.txt"));
        foreach (string line in rythm)
        {
            if (line.Contains(","))
            {
                numSpawnedArrows++;
            }
        }
        SetBeat(float.Parse(rythm[rythmIndex])/ /*60f*/(float)7.5);
        BeatMomentsSetup();

    }

    void SetBeat(float beat)
    {
        beatPerSecond = beat;
        SwipeSpace = beat / 2f; //swipeSpace avghenger av beatPerSecond
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime; //Time.deltaTime *2 gjør at timeren gå dobbelt så fort
        combosetter();
        if(Input.GetMouseButtonUp(0))
        {
            newSwipe = true;
        }
        randomness = score / 100f;

        BeatCountdown();
        ArrowCountDown();

        
       /* else
        {
            pointMultiplierTracker.GetComponent<TextMeshProUGUI>().enabled = false;
            scoreTracker.GetComponent<TextMeshProUGUI>().fontSize = 90;
            GameObject.Find("EndScreen").GetComponent<Canvas>().enabled = true;
        }*/
        
    }

    void BeatCountdown() // Lager piler basert på rytme
    {
        /*if () //Bestemmer når vi begynner med å ikke-spawne random arrows på beaten
        {
            randomness 
            randomness = Random.Range(0f, 2f);
            if (beatTimer > 0 && numSpawnedArrows != 0)
            {
                beatTimer -= Time.deltaTime;
            }
            else if (beatTimer <= 0 && randomness > 0.5f && numSpawnedArrows != 0)
            {
                beatTimer = 1f / beatPerSecond;
                numSpawnedArrows--;
                MakeArrow();
            }
            else
            {
                beatTimer = 1f / beatPerSecond;
            }
        }*/

        if (beatTimer > 0 && numSpawnedArrows != 0)
        {
            beatTimer -= Time.deltaTime;
        }
        else if (beatTimer <= 0 && numSpawnedArrows != 0)
        {
            beatTimer = 1f / beatPerSecond;
            // if(UnityEngine.Random.Range(0f,1.5f) >= randomness) //0 til 1,99999(osv.) siden det er float
            // {
            // SetBeat(baseBeat);
            if (beatTracker < 31)
            {
                beatTracker++;
            }
            else
            {
                BeatMomentsSetup();
                beatTracker = 0;
            }
            if (beatMoments.Contains(beatTracker))
            {
                MakeArrow();
                numSpawnedArrows--;
                print("SpawnedArrows: " + numSpawnedArrows);
            }
               


            //}
           /* else
            {
                if(UnityEngine.Random.Range(0,2) == 0)
                {
                    SetBeat(baseBeat*2f);
                    //beatTimer = (1f / beatPerSecond) / 2f;
                    //print("AHhhhhhhh!");
                    //print(beatTimer);
                    
                }
                else
                {
                    SetBeat(baseBeat);
                }
            }*/ 
        }
    }

    void BeatMomentsSetup()
    {
        beatMoments.Clear();
        if (rythmIndex < rythm.Length-1)
        {
            rythmIndex++;
            if (rythm[rythmIndex] == "{")
            {
                rythmIndex++;
                while (rythm[rythmIndex] != "}")
                {
                    beatMoments.Add(float.Parse(rythm[rythmIndex].Substring(0, rythm[rythmIndex].IndexOf(",")))*2);
                    rythmIndex++;
                }
                print(beatMoments);
            }
            else
            {
                beatMoments.Clear();
            }
        }
    }

    /*void SetRythm(string filename)
    {
        rythm = System.IO.File.ReadAllLines(Path.GetFullPath(filename));
    }*/

    void combosetter()
    {
        if (combo > 20)
        {
            comboMultiplier = 8;
            pointMultiplierTracker.GetComponent<TextMeshProUGUI>().text = "x8";
        }
        else if (combo > 10)
        {
            comboMultiplier = 4;
            pointMultiplierTracker.GetComponent<TextMeshProUGUI>().text = "x4";
        }
        else if (combo > 5)
        {
            comboMultiplier = 2;
            pointMultiplierTracker.GetComponent<TextMeshProUGUI>().text = "x2";

        }
        else
        {
            comboMultiplier = 1;
            pointMultiplierTracker.GetComponent<TextMeshProUGUI>().text = "x1";
        }
    }

    void ArrowCountDown()//Går inn i hver arrow og trekker fra timer for eksistens
    {
        List<Arrow> deadArrows = new List<Arrow>();
        foreach (Arrow arrow in arrowList)
        {
            arrow.timer -= Time.deltaTime;

            if (arrow.timer < 0 + SwipeSpace) //Sjekker om swiper innenfor akseptabelt tidsrom
            {
                if (SwipeChecker(arrow) != null)
                {
                    deadArrows.Add(arrow);
                }

            }
            if (arrow.timer < -SwipeSpace) //Fjerner piler etter at timer i pilen er gått ut
            {
                deadArrows.Add(arrow);
                if (failText.GetComponent<TextMeshProUGUI>().text == "")
                {
                    failText.GetComponent<TextMeshProUGUI>().text = "Fail";
                    GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = new Color32(203, 45, 43, 255);
                    combo = 0;
                    comboText.GetComponent<TextMeshProUGUI>().text = "Combo: " + combo;
                }
                else
                {
                    failText.GetComponent<TextMeshProUGUI>().text += "!";
                }
                
            }
        }
        foreach (Arrow ded in deadArrows)
        {
            Destroy(ded.getArrowObject());
            arrowList.Remove(ded); //Fjerner pilen fra arrayet for eksistens på skjermen
            //print(ded.getArrowObject().GetComponent<SpriteRenderer>().sortingOrder);
            if (ded.getArrowObject().GetComponent<SpriteRenderer>().sortingOrder <= 1)
            {
                pointMultiplierTracker.GetComponent<TextMeshProUGUI>().enabled = false;
                scoreTracker.GetComponent<TextMeshProUGUI>().fontSize = 90;
                GameObject.Find("EndScreen").GetComponent<Canvas>().enabled = true;
                GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = new Color32(43, 202, 131, 255);
            }
        }
    }

    Arrow SwipeChecker(Arrow arrow) //Gjør ting innenfor det akeptable tidsrommet å swipe i
    {
        if (arrow.swipable == false && GetComponent<SwipeDirCheck>().getDir() != -1)
        {
            combo = 0;
            comboText.GetComponent<TextMeshProUGUI>().text = "Combo: " + combo;
        }
        if(arrow.swipable == false)
        {
            arrow.swipable = true; //Arrow blir farget kun en gang
            arrow.getArrowObject().GetComponent<SpriteRenderer>().color = new Color32(255, 0, 255, 255);
            GameObject circlePref =  Instantiate(circle);
            circlePref.transform.position = arrow.getArrowObject().transform.position;
            arrow.setCircle(circlePref);
        }
        
        if(arrow.timer <= 0 && arrow.getCircle() != null)
        {
            Destroy(arrow.getCircle());
        }
        else if(arrow.getCircle() != null)
        {
            arrow.getCircle().transform.localScale = new Vector3(((arrow.timer / SwipeSpace) * 2) + 1, ((arrow.timer / SwipeSpace) * 2) + 1, ((arrow.timer / SwipeSpace) * 2) + 1);
        }

        if (GetComponent<SwipeDirCheck>().getDir() == arrow.getDirection() && newSwipe == true && arrow.swipable == true)
        {
            score += (float)System.Math.Round(1 - Mathf.Round(arrow.timer / SwipeSpace * 20) / 20f * pointMultiplier * comboMultiplier,2); //Nå med kun to desimaler!
            //print(score);

            scoreTracker.GetComponent<TextMeshProUGUI>().text = "Score: "+ score.ToString("n2");
            failText.GetComponent<TextMeshProUGUI>().text = "";
            combo++;
            comboText.GetComponent<TextMeshProUGUI>().text = "Combo: " + combo;
            GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = new Color32(43, 202, 131, 255);

            Destroy(arrow.getArrowObject());
            Destroy(arrow.getCircle());

            //arrow.timer = -SwipeSpace-1;

            newSwipe = false;
            return arrow;
        }
        return null;
    }

    void MakeArrow() //Lager piler
    {
        GameObject newarrowPref = Instantiate(arrowPref); // Lager selve klonepilene
        Arrow newArrow = new Arrow(UnityEngine.Random.Range(0, 4), newarrowPref, (1/beatPerSecond) * 4)/*Sekunder du har på deg til å utføre action*/; //Setter opp parametere for piler
        newArrow.getArrowObject().GetComponent<SpriteRenderer>().sortingOrder = numSpawnedArrows;
        arrowList.Add(newArrow);

        newArrow.getArrowObject().GetComponent<Transform>().eulerAngles = new Vector3(0, 0, 90 * newArrow.getDirection()); // Roterer pilene riktig i forhold til retning
        newArrow.getArrowObject().GetComponent<Transform>().position = new Vector2(((newArrow.getDirection() % 2) * 1.5f * (2 - newArrow.getDirection())),
                                                                                    ((newArrow.getDirection() + 1) % 2) * 1.5f * (newArrow.getDirection() - 1)); //Plasserer pilene på riktig sted
    }
}

public class Arrow
{
    private int direction;
    private GameObject arrowObject; //Manifestasjonen av pilen på skjermen
    public float timer; //Hvor lenge pilen eksisterer
    public bool swipable;
    private GameObject circle;

    public Arrow(int direction, GameObject arrowObject, float timer)
    {
        this.direction = direction;
        this.arrowObject = arrowObject;
        this.timer = timer;
    }

    public void setCircle(GameObject circle)
    {
        this.circle = circle;
    }

    public GameObject getCircle()
    {
        return circle;
    }
    
    public int getDirection()
    {
        return direction;
    }

    public GameObject getArrowObject()
    {
        return arrowObject;
    }
}

/*public class SwipeDirCheck: MonoBehaviour
{
    private float mouseTimeDown;
    private Vector3 firstMousePosition;
   // private Vector3 lastMousePosition;

    public int getDir()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            mouseTimeDown = 0;
            //lastMousePosition = Input.mousePosition;
            firstMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            mouseTimeDown += Time.deltaTime;
            //if (Input.mousePosition.x > lastMousePosition.x && Input.mousePosition.x > firstMousePosition.x+100f)

            Vector2 vectorSum = Input.mousePosition - firstMousePosition;
            vectorSum.Normalize();

            //lastMousePosition = Input.mousePosition;

            if (vectorSum.x > Mathf.Abs(vectorSum.y))
            {
                //transform.position += new Vector3(1, 0, 0) * Time.deltaTime;
                return 1; //Høyre
            }

            else if (Mathf.Abs(vectorSum.x) > Mathf.Abs(vectorSum.y))
            {
                //transform.position += new Vector3(-1, 0, 0) * Time.deltaTime;
                return 3; //Venstre
            }
            else if (vectorSum.y > Mathf.Abs(vectorSum.x))
            {
                //transform.position += new Vector3(0, 1, 0) * Time.deltaTime;
                return 2; //Opp
            }
            else if (Mathf.Abs(vectorSum.y) > Mathf.Abs(vectorSum.x))
            {
                //transform.position += new Vector3(0, -1, 0) * Time.deltaTime;
                return 0; //Ned
            }
            
        }
        return -1;
    }*/
