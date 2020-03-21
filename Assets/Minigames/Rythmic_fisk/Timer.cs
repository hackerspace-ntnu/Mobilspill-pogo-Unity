using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Til neste gang: Sett en restriction på antall piler, så man ikke får uendelig poeng
//Finn ut hvordan å gi bedre feedback
//vite når man har failet
//Hvis to piler er oppå hverandre vil man ikke se når den underste skifter farge. Fiks.
//Ca. maxgrense på poeng

public class Timer : MonoBehaviour //MonoBehavior betyr basicly at jeg kan gjøre Unity-greier med klassen, som f.eks. Deltatime
{
    float time; //Selve tiden
    List<Arrow> arrowList = new List<Arrow>();//Liste som inneholder alle piler på skjermen
    public GameObject arrowPref; //Klonene av piler som dukker opp
    public GameObject circle; //Klonene av sirkler
    private float beatPerSecond; // Bestemmer hvor lenge pilene skal være på skjermen
    private float beatTimer; //Teller fra 0 til 1/beatperSecond
    private float SwipeSpace; // Tidsrom det er akseptabelt å swipe
    private float poeng;
    private float pointMultiplier;
    private float randomness; //Går fra 0 til 1 og bestemmer om pil dukker opp senere i spillet
    private bool newSwipe;
    private int numSpawnedArrows; //Antall arrows som totalt har spawnet i løpet av spillet
    //private SwipeDirCheck swipeDirCheck = new SwipeDirCheck();

    // Start is called before the first frame update
    void Start()
    {
        SetBeat(1f); // Setter beatPerSecond
        pointMultiplier = 1;
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

        if(Input.GetMouseButtonUp(0))
        {
            newSwipe = true;
        }

        BeatCountdown();
        ArrowCountDown(); 
    }

    void BeatCountdown() // Lager piler basert på rytme
    {
        if (time >= 10) //Bestemmer når vi begynner med å ikke-spawne random arrows på beaten
        {
            randomness = Random.Range(0, 2);
            if (beatTimer > 0)
            {
                beatTimer -= Time.deltaTime;
            }
            else if (beatTimer <= 0 && randomness > 0.5)
            {
                beatTimer = 1f / beatPerSecond;
                numSpawnedArrows++;
                MakeArrow();
            }
            else
            {
                beatTimer = 1f / beatPerSecond;
            }
        }
        else
        {
            if(beatTimer > 0)
            {
                beatTimer -= Time.deltaTime;
            }
            else if(beatTimer <= 0)
            {
                beatTimer = 1f / beatPerSecond;
                numSpawnedArrows++;
                MakeArrow();
            }
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
                SwipeChecker(arrow);

            }
            if (arrow.timer < -SwipeSpace) //Fjerner piler etter at timer i pilen er gått ut
            {
                deadArrows.Add(arrow);
            }
        }
        foreach (Arrow ded in deadArrows)
        {
            Destroy(ded.getArrowObject());
            arrowList.Remove(ded); //Fjerner pilen fra arrayet for eksistens på skjermen
        }
    }

    void SwipeChecker(Arrow arrow) //Gjør ting innenfor det akeptable tidsrommet å swipe i
    {
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

        if (GetComponent<SwipeDirCheck>().getDir() == arrow.getDirection() && newSwipe == true)
        {
            poeng += (1 - (Mathf.Round((arrow.timer/SwipeSpace) * 20) / 20f)) * pointMultiplier;
            print(poeng);

            Destroy(arrow.getArrowObject());
            Destroy(arrow.getCircle());

            arrow.timer = -SwipeSpace-1;

            newSwipe = false;
        }
    }

    void MakeArrow() //Lager piler
    {
        GameObject newarrowPref = Instantiate(arrowPref); // Lager selve klonepilene
        Arrow newArrow = new Arrow(Random.Range(0, 4), newarrowPref, (1/beatPerSecond) * 4)/*Sekunder du har på deg til å utføre action*/; //Setter opp parametere for piler
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
