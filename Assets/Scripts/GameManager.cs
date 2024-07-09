using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    private Player player;
    //private Enemy enemy;
    public int maxNumTurns;  // here for the example fight handler
    private Enemy enemy;
    private DeckManager deck;
    //private HUDManager HUD;
    //private Enemy enemy;
    //private GameObject fightManager;
    public int NumBattles;
    public int MaxFights;
    public List<Card> playerDeck = new List<Card>();
    public List<Card> enemyDeck = new List<Card>();
    public bool firstLoad = true;
    public AudioSource theme;
    private bool isPlaying;

    public AudioSource fightSound;
    private int playerCredits = 0;

    public List<GameObject> cardPrefabs = new List<GameObject>();
    private Dictionary<string, GameObject> cardPrefabDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        NumBattles = MaxFights;
        isPlaying = false;

        foreach(GameObject c in cardPrefabs)
        {
            cardPrefabDict[c.name] = c;
        }
    }

    public static GameManager instance()
    {
        return _instance;
    }

    void Start()
    {
        Debug.Log("Start called: " + SceneManager.GetActiveScene().name);
        switch (SceneManager.GetActiveScene().name)  // needs to be here to assure that player is initialized
        {
            case "MainMenu":
                Debug.Log("MainMenu");
                if (!isPlaying)
                {
                    Debug.Log("Playing theme");
                    Debug.Log(isPlaying);
                    theme.Play();
                    isPlaying = true;
                }
                break;
            case "FightScene":
                if (player == null)
                {
                    player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                    if(playerDeck.Count == 0) 
                    {
                        playerDeck = player.deck;
                    }
                }
                else
                {
                    //player.deck = playerDeck;
                }
                enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
                enemyDeck = enemy.enemyDeck;
                NumBattles--;
                playerCredits += 100;
                break;
            case "ShopScene":
                break;
            default: break;
        }
    }

    //Scene Functions
    public void LoadShop()
    {
        //playerDeck = player.deck;
        if(deck == null)
        {
            deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<DeckManager>();
        }
        deck.DisableDeckChildren();
        deck.UnparentDeck();
        deck.ReparentDeckToGM();
        SceneManager.LoadScene("Shop");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadWin()
    {
        deck.DisableDeckChildren();
        deck.UnparentDeck();
        deck.ReparentDeckToGM();
        SceneManager.LoadScene("WinScreen");
    }

    public void LoadLoss()
    {
        if (deck == null)
        {
            deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<DeckManager>();
        }
        deck.DisableDeckChildren();
        deck.UnparentDeck();
        deck.ReparentDeckToGM();
        SceneManager.LoadScene("LoseScreen");
    }

    public void LoadFightScene()
    {
        SceneManager.LoadScene("FightScene");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Start();
        if (scene.name == "Shop")
        {
            // Unsubscribe from the event to prevent memory leaks
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        else if (scene.name == "FightScene" /*&& deck != null && HUD!=null*/)
        {
            if (deck != null)
            {
                deck.UnparentDeck();
                deck.ReparentDeckToCanvas();
            }
            // Unsubscribe from the event to prevent memory leaks
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void playFightSound()
    {
        fightSound.Play();
    }

    public int getPlayerCredits() {  return playerCredits; }
    public void spendCredits(int amount) { playerCredits -= amount; }

    public void AddCardToDeck(Card c)
    {
        // create card
        GameObject obj = Instantiate(cardPrefabDict[c.name]) as GameObject;
        if (obj != null)
        {
            Debug.Log("Created " + c.name);
        }
        else
        {
            Debug.LogError(c.name + " failed to instantiate correctly");
        }
        // set inactive
        obj.SetActive(false);
        // make parent deck game object
        obj.transform.SetParent(this.transform.GetChild(0).gameObject.transform, false);
        // add to list?
        playerDeck.Add(c);
    }
}
