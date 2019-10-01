using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_MenuLogic : MonoBehaviour
{
    public Dictionary<string, GameObject> screenList;
    private SC_GlobalEnums.Screens currentScreen;
    private Stack<SC_GlobalEnums.Screens> screenStack;
    public Dictionary<string, GameObject> sliders;
    private Dictionary<string, ScriptableObject> cards;
    private Stack deckOne;
    private Stack deckTwo;
    private Stack resetStack;
    public SC_Hero PlayerOneHero;
    public SC_Hero PlayerTwoHero;
    public SC_GlobalEnums.turns currentTurn;
    public bool isMyTurn;
    public Dictionary<string, object> toSend = new Dictionary<string, object>();
    public int attackNumber = 0;
    public bool singlePlayer = true;

    static SC_MenuLogic instance;
    public static SC_MenuLogic Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_MenuLogic").GetComponent<SC_MenuLogic>();

            return instance;
        }
    }

    private void OnEnable()
    {
        Listener.OnGameStarted += OnGameStarted;
        Listener.OnMoveCompleted += OnMoveCompleted;
        Listener.OnGameStopped += OnGameStopped;
    }

    private void OnDisable()
    {
        Listener.OnGameStarted -= OnGameStarted;
        Listener.OnMoveCompleted -= OnMoveCompleted;
        Listener.OnGameStopped -= OnGameStopped;
    }


    void Awake()
    {
        initiate();
    }

    private void initiate()
    {
        screenList = new Dictionary<string, GameObject>();
        sliders = new Dictionary<string, GameObject>();
        screenStack = new Stack<SC_GlobalEnums.Screens>();
        cards = new Dictionary<string, ScriptableObject>();
        deckOne = new Stack();
        deckTwo = new Stack();
        resetStack = new Stack();

        GameObject[] _screens = GameObject.FindGameObjectsWithTag("screenList");
        foreach (GameObject g in _screens)
            screenList.Add(g.name, g);
        GameObject[] _sliders = GameObject.FindGameObjectsWithTag("sliders");
        foreach (GameObject g in _sliders)
            sliders.Add(g.name, g);

        creatingDecks();

        screenList["GameOver"].SetActive(false);
        screenList["Screen_Menu"].SetActive(false);
        screenList["Screen_SinglePlayer"].SetActive(false);
        screenList["Screen_MultiPlayer"].SetActive(false);
        screenList["Screen_StudentInfo"].SetActive(false);
        screenList["Screen_Options"].SetActive(false);
        ChangeScreen(SC_GlobalEnums.Screens.Menu);
    }

    #region Menus
    public void ChangeScreen(SC_GlobalEnums.Screens _Screen)
    {
        screenStack.Push(currentScreen);
        if (_Screen == SC_GlobalEnums.Screens.SinglePlayer)
        {
            isMyTurn = true;
            screenList["Screen_SinglePlayer_Title"].GetComponent<Text>().text = "Player One";
        }
        screenList["Screen_" + currentScreen].SetActive(false);
        screenList["Screen_" + _Screen].SetActive(true);
        currentScreen = _Screen;
    }

    public void Back()
    {
        screenList["Screen_" + currentScreen].SetActive(false);
        screenList["Screen_" + screenStack.Peek()].SetActive(true);
        currentScreen = screenStack.Peek();
        if (screenStack.Count != 0)
            screenStack.Pop();

    }

    public void Screen_Multiplayer_Slider()
    {
        int _value = (int)(sliders["Screen_MultiPlayer_Slider"].GetComponent<Slider>().value * 10);
        sliders["Screen_MultiPlayer_Slider_Value"].GetComponent<Text>().text = _value.ToString();
    }

    public void Screen_Options_Slider_Music()
    {
        int _value = (int)(sliders["Screen_Options_Slider_Music"].GetComponent<Slider>().value * 10);
        sliders["Screen_Options_Slider_Value_Music"].GetComponent<Text>().text = _value.ToString();
    }

    public void Screen_Options_Slider_Sfx()
    {
        int _value = (int)(sliders["Screen_Options_Slider_Sfx"].GetComponent<Slider>().value * 10);
        sliders["Screen_Options_Slider_Value_Sfx"].GetComponent<Text>().text = _value.ToString();
    }

    public void openUrl()
    {
        Application.OpenURL("https://www.linkedin.com/in/yuval-ozeri-2149b3110/");
    }
    #endregion


    #region Single Player
    
    public void EndTurn()
    {
        if (currentTurn == SC_GlobalEnums.turns.PlayerOne)
        {
            currentTurn = SC_GlobalEnums.turns.PlayerTwo;
            GlobalVariables.CardsPlayedbyPlayerOne = 0;
            if (singlePlayer)
            {
                screenList["Screen_SinglePlayer_Title"].GetComponent<Text>().text = "Player Two";
                PlayerTwoDraw();
            }
        }
        else if (currentTurn == SC_GlobalEnums.turns.PlayerTwo)
        {
            currentTurn = SC_GlobalEnums.turns.PlayerOne;
            GlobalVariables.CardsPlayedbyPlayerTwo = 0;
            if (singlePlayer)
            {
                screenList["Screen_SinglePlayer_Title"].GetComponent<Text>().text = "Player One";
                PlayerOneDraw();
            }
        }

        string _send = MiniJSON.Json.Serialize(toSend);
        WarpClient.GetInstance().sendMove(_send);
        toSend.Clear();
        UpdateGui();
        if (!singlePlayer)
        {
            screenList["Button_End_Turn"].SetActive(false);
        }

        GameObject[] _cardsInGame = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject g in _cardsInGame)
            g.GetComponent<Button>().interactable = true;
    }

    public void PlayerOneDraw()
    {
        GameObject _card = Instantiate(Resources.Load("Card")) as GameObject;
        _card.transform.SetParent(GameObject.Find("Player_One_Hand").transform.transform);
        _card.GetComponent<SC_CardDisplay>().cardStats = deckOne.Pop() as Card;
        _card.GetComponent<RectTransform>().transform.localScale = new Vector3(0.3f, 0.3f, 0);
    }

    public void PlayerTwoDraw()
    {
        GameObject _card = Instantiate(Resources.Load("Card")) as GameObject;
        _card.transform.SetParent(GameObject.Find("Player_Two_Field").transform.transform);
        _card.GetComponent<SC_CardDisplay>().cardStats = deckTwo.Pop() as Card;
        _card.GetComponent<RectTransform>().transform.localScale = new Vector3(0.3f, 0.3f, 0);
    }


    public void creatingDecks()
    {
        cards.Add("Acidic Swamp Ooze", Resources.Load("Decks/Player_One/Acidic Swamp Ooze 1") as ScriptableObject);
        cards.Add("Amani War Bear", Resources.Load("Decks/Player_One/Amani War Bear 1") as ScriptableObject);
        cards.Add("Angry Chicken", Resources.Load("Decks/Player_One/Angry Chicken") as ScriptableObject);
        cards.Add("Arcane Servant", Resources.Load("Decks/Player_One/Arcane Servant 1") as ScriptableObject);
        cards.Add("Blink Fox", Resources.Load("Decks/Player_One/Blink Fox 1") as ScriptableObject);
        cards.Add("Bloodfen Raptor", Resources.Load("Decks/Player_One/Bloodfen Raptor 1") as ScriptableObject);
        cards.Add("Bluegill Warrior", Resources.Load("Decks/Player_One/Bluegill Warrior 1") as ScriptableObject);
        cards.Add("Boulderfirst Ogre", Resources.Load("Decks/Player_One/Boulderfirst Ogre 1") as ScriptableObject);
        cards.Add("Bronze Gatekeeper", Resources.Load("Decks/Player_One/Bronze Gatekeeper") as ScriptableObject);
        cards.Add("Chillwind Yeti", Resources.Load("Decks/Player_One/Chillwind Yeti 1") as ScriptableObject);
        cards.Add("Core Hound", Resources.Load("Decks/Player_One/Core Hound 1") as ScriptableObject);
        cards.Add("Edwin", Resources.Load("Decks/Player_One/Edwin") as ScriptableObject);

        fillStacks();
    }

    public void fillStacks()
    {
        foreach (var _card in cards.Values)
        {
            deckTwo.Push(_card);
            deckOne.Push(_card);
        }

        resetStack.Push(Resources.Load("Decks/Player_One/Blink Fox 1") as ScriptableObject);
        resetStack.Push(Resources.Load("Decks/Player_One/Ironbark Protector 1") as ScriptableObject);
        resetStack.Push(Resources.Load("Decks/Player_One/Lord of the Arena 1") as ScriptableObject);
    }

    public void gameover()
    {
        screenList["GameOver"].SetActive(true);
        if(currentTurn.ToString() == "PlayerOne")
        {
            screenList["GameOver"].GetComponentInChildren<Text>().text = "You won!";
        }
        else
        {
            screenList["GameOver"].GetComponentInChildren<Text>().text = "You lost!";
        }
    }

    private void UpdateGui()
    {
        if (currentTurn == SC_GlobalEnums.turns.PlayerOne)
            screenList["Screen_SinglePlayer_Title"].GetComponent<Text>().text = SC_GlobalEnums.turns.PlayerOne.ToString();
        else if (currentTurn == SC_GlobalEnums.turns.PlayerTwo)
            screenList["Screen_SinglePlayer_Title"].GetComponent<Text>().text = SC_GlobalEnums.turns.PlayerTwo.ToString();
    }

    public void Restart()
    {

        GameObject[] _Fields = GameObject.FindGameObjectsWithTag("Reset");
        foreach (GameObject g in _Fields)
        {
            foreach (Transform t in g.transform)
            {
                GameObject.Destroy(t.gameObject);
            }
        }

        if (PlayerOneHero != null && PlayerTwoHero != null)
        {
            PlayerOneHero.Restart();
            PlayerTwoHero.Restart();
        }

        while (resetStack.Count != 0) {
            GameObject _card = Instantiate(Resources.Load("Card")) as GameObject;
            _card.transform.SetParent(GameObject.Find("Player_One_Hand").transform.transform);
            _card.GetComponent<SC_CardDisplay>().cardStats = resetStack.Pop() as Card;
            _card.GetComponent<RectTransform>().transform.localScale = new Vector3(0.3f, 0.3f, 0);
        }

        toSend.Clear();
        attackNumber = 0;
        UpdateGui();
        fillStacks();
        GlobalVariables.PlayerOneAttack = 0;
        GlobalVariables.PlayerTwoAttack = 0;
        GlobalVariables.CardsPlayedbyPlayerOne = 0;
        GlobalVariables.CardsPlayedbyPlayerTwo = 0;
        screenList["GameOver"].SetActive(false);

        ChangeScreen(SC_GlobalEnums.Screens.Menu);
    }

    #endregion

    #region Events
    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        if (SC_Menu.Instance.userId == _NextTurn)
        {
            isMyTurn = true;
            currentTurn = SC_GlobalEnums.turns.PlayerOne;
            screenList["Screen_SinglePlayer_Title"].GetComponent<Text>().text = SC_GlobalEnums.turns.PlayerOne.ToString();
        }
        else
        {
            isMyTurn = false;
            currentTurn = SC_GlobalEnums.turns.PlayerTwo;
            screenList["Screen_SinglePlayer_Title"].GetComponent<Text>().text = SC_GlobalEnums.turns.PlayerTwo.ToString();
            screenList["Button_End_Turn"].SetActive(false);
        }

    }

    private void CardAttack(string _CardName,string  _HealthLeft)
    {
        GameObject _parent = GameObject.Find("Player_One_Field");
        foreach (Transform child in _parent.transform)
        {
            if (child.GetComponent<SC_CardDisplay>().card.name.ToString() == _CardName)
            {
                if (int.Parse(_HealthLeft) <= 0)
                {
                    Destroy(child.gameObject);
                }else child.transform.Find("Health").GetComponent<Text>().text = _HealthLeft;
            }
        }   
    }

    private void HeroAttack(int _heroHP)
    {
        screenList["Player_One_Hero"].GetComponentInChildren<Text>().text = _heroHP.ToString();
        if (_heroHP <= 0)
            gameover();
    }

    private void PlaceNewCard(string _cardName)
    {
        GameObject _card = Instantiate(Resources.Load("Card")) as GameObject;
        _card.transform.SetParent(GameObject.Find("Player_Two_Field").transform.transform);
        _card.GetComponent<SC_CardDisplay>().cardStats = Resources.Load("Decks/Player_One/Acidic Swamp Ooze 1") as Card;
        _card.GetComponent<SC_CardDisplay>().cardStats = Resources.Load("Decks/Player_One/" + _cardName) as Card;
        _card.GetComponent<RectTransform>().transform.localScale = new Vector3(0.3f, 0.3f, 0);
    }

    private void OnMoveCompleted(MoveEvent _Move)
    {
        if (_Move.getSender() != SC_Menu.Instance.userId)
        {
            Dictionary<string, object> _data = (Dictionary<string, object>)MiniJSON.Json.Deserialize(_Move.getMoveData());

            if (_data != null && _data.ContainsKey("Attack"))
            {
                attackNumber = int.Parse(_data["Attack"].ToString());
                for (int i = 0; i <= attackNumber; i++)
                {
                    string _cardName = _data["CardName" + i].ToString();
                    string _healthLeft = _data["HealthLeft" + i].ToString();
                    CardAttack(_cardName,_healthLeft);
                }
            }

            if (_data.ContainsKey("NewCard1"))
            {
                string _cardName = _data["NewCard1"].ToString();
                PlaceNewCard(_cardName);
            }

            if (_data.ContainsKey("NewCard2"))
            {
                string _cardName = _data["NewCard2"].ToString();
                PlaceNewCard(_cardName);
            }

            if (_data.ContainsKey("Hero"))
            {
                int _heroHP = int.Parse(_data["Hero"].ToString());
                HeroAttack(_heroHP);
            }

            if (_data.ContainsKey("GameOver"))
            {
                gameover();
            }

            if (currentTurn == SC_GlobalEnums.turns.PlayerOne)
                currentTurn = SC_GlobalEnums.turns.PlayerTwo;
            else if (currentTurn == SC_GlobalEnums.turns.PlayerTwo)
                currentTurn = SC_GlobalEnums.turns.PlayerOne;

            UpdateGui();
            PlayerOneDraw();
            attackNumber = 0;
            screenList["Button_End_Turn"].SetActive(true);
        }


        if (_Move.getNextTurn() == SC_Menu.Instance.userId)
            isMyTurn = true;
        else isMyTurn = false;
    }

    private void OnGameStopped(string _Sender, string _RoomId)
    {
        Debug.Log("Game Over");
    }
    #endregion
}
