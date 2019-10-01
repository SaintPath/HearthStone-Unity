using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;
using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using UnityEngine.UI;

public class SC_Menu : MonoBehaviour
{
    private string apiKey = "9a689b442474fc98705e870785bb4b95c98cd5f6bffcc459421a6c397e910138";
    private string secretKey = "c689fd7144f3affd8fa120e21a5f596c12ab3e2d11b143079a06f66cdd353b42";
    public Listener listen;
    public string userId = "";
    private Dictionary<string, object> matchRoomData;
    private Dictionary<string, GameObject> unityObjects;
    private List<string> roomIds;
    private int roomIdx = 0;
    private string curRoomId;
    private bool Connected = false;

    static SC_Menu instance;
    public static SC_Menu Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_Menu").GetComponent<SC_Menu>();

            return instance;
        }
    }


    private void OnEnable()
    {
        Listener.OnConnect += OnConnect;
        Listener.OnRoomsInRange += OnRoomsInRange;
        Listener.OnCreateRoom += OnCreateRoom;
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfo;
        Listener.OnJoinRoom += OnJoinRoom;
        Listener.OnUserJoinRoom += OnUserJoinRoom;
        Listener.OnGameStarted += OnGameStarted;
    }

    private void OnDisable()
    {
        Listener.OnConnect -= OnConnect;
        Listener.OnRoomsInRange -= OnRoomsInRange;
        Listener.OnCreateRoom -= OnCreateRoom;
        Listener.OnGetLiveRoomInfo -= OnGetLiveRoomInfo;
        Listener.OnJoinRoom -= OnJoinRoom;
        Listener.OnUserJoinRoom -= OnUserJoinRoom;
        Listener.OnGameStarted -= OnGameStarted;
    }

    void Start()
    {
        //JsonTest();
        MenuInit();
    }

    private void JsonTest()
    {
        Dictionary<string, object> _test = new Dictionary<string, object>();
        _test.Add("Color", "Red");
        _test.Add("Number", 1);

        string _toSend = Json.Serialize(_test);
        Debug.Log(_toSend);

        _test = (Dictionary<string, object>)Json.Deserialize(_toSend);
        Debug.Log(_test.Count);
    }

    private void MenuInit()
    {
        unityObjects = new Dictionary<string, GameObject>();
        unityObjects = SC_MenuLogic.Instance.screenList;
        Debug.Log(unityObjects.Count);

        unityObjects["Btn_Play"].GetComponent<Button>().interactable = false;
        unityObjects["Screen_SinglePlayer"].SetActive(false);

        if (listen == null)
            listen = new Listener();

        WarpClient.initialize(apiKey, secretKey);
        WarpClient.GetInstance().AddConnectionRequestListener(listen);
        WarpClient.GetInstance().AddChatRequestListener(listen);
        WarpClient.GetInstance().AddUpdateRequestListener(listen);
        WarpClient.GetInstance().AddLobbyRequestListener(listen);
        WarpClient.GetInstance().AddNotificationListener(listen);
        WarpClient.GetInstance().AddRoomRequestListener(listen);
        WarpClient.GetInstance().AddZoneRequestListener(listen);
        WarpClient.GetInstance().AddTurnBasedRoomRequestListener(listen);

        matchRoomData = new Dictionary<string, object>();

        userId = System.DateTime.Now.Ticks.ToString();
        unityObjects["Txt_UserId"].GetComponent<Text>().text = userId;
        Debug.Log(userId);
        WarpClient.GetInstance().Connect(userId);
        UpdateStatus("Connecting...");
    }

    void Update()
    {
        if (Connected == false)
            WarpClient.GetInstance().Connect(userId);
    }

    #region Events

    private void OnConnect(bool _IsSuccess)
    {
        Debug.Log(_IsSuccess);
        if (_IsSuccess)
        {
            UpdateStatus("Connected!");
            unityObjects["Btn_Play"].GetComponent<Button>().interactable = true;
            Connected = true;
        }
        else UpdateStatus("Connection Error");
    }

    private void OnRoomsInRange(bool _IsSuccess, MatchedRoomsEvent eventObj)
    {
        Debug.Log(_IsSuccess + " " + "" + eventObj.getRoomsData().Length);
        if (_IsSuccess)
        {
            UpdateStatus("Parsing Rooms");
            roomIds = new List<string>();
            foreach (var roomData in eventObj.getRoomsData())
            {
                Debug.Log("RoomId " + roomData.getId());
                Debug.Log("Room Owner " + roomData.getRoomOwner());
                roomIds.Add(roomData.getId());
            }

            roomIdx = 0;
            DoRoomSearchLogic();
        }
        else UpdateStatus("Error Fetching Rooms in Range");
    }

    private void DoRoomSearchLogic()
    {
        if (roomIdx < roomIds.Count)
        {
            UpdateStatus("Get Room Details (" + roomIds[roomIdx] + ")");
            WarpClient.GetInstance().GetLiveRoomInfo(roomIds[roomIdx]);
        }
        else
        {
            UpdateStatus("Create Room...");
            WarpClient.GetInstance().CreateTurnRoom("Test", userId, 2, matchRoomData, 60);
        }
    }

    private void OnCreateRoom(bool _IsSuccess, string _RoomId)
    {
        Debug.Log("OnCreateRoom " + _IsSuccess + " " + _RoomId);
        if (_IsSuccess)
        {
            UpdateStatus("Room Created, waiting for opponent...");
            curRoomId = _RoomId;
            WarpClient.GetInstance().JoinRoom(curRoomId);
            WarpClient.GetInstance().SubscribeRoom(curRoomId);
        }
        else UpdateStatus("Failed to create Room");
    }

    private void OnGetLiveRoomInfo(LiveRoomInfoEvent eventObj)
    {
        Dictionary<string, object> _prams = eventObj.getProperties();
        if (_prams != null && _prams.ContainsKey("Password"))
        {
            string _pass = _prams["Password"].ToString();
            if (_pass == matchRoomData["Password"].ToString())
            {
                curRoomId = eventObj.getData().getId();
                UpdateStatus("Joining Room " + curRoomId);
                WarpClient.GetInstance().JoinRoom(curRoomId);
                WarpClient.GetInstance().SubscribeRoom(curRoomId);
            }
            else
            {
                roomIdx++;
                DoRoomSearchLogic();
                unityObjects["Btn_Play"].SetActive(true);
            }
        }
    }

    private void OnJoinRoom(bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess)
            UpdateStatus("Succefully Joined Room " + _RoomId);
        else UpdateStatus("Failed to Joined Room " + _RoomId);
    }


    private void OnUserJoinRoom(RoomData eventObj, string _UserId)
    {
        if (userId != _UserId)
        {
            UpdateStatus(_UserId + " Have joined the room");
            WarpClient.GetInstance().startGame();
        }
    }
    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        UpdateStatus("Started Game, " + _NextTurn + " Turn to Play");
        unityObjects["Screen_Menu"].SetActive(false);
        unityObjects["Screen_SinglePlayer"].SetActive(true);
    }

    #endregion

    #region Logic

    private void UpdateStatus(string _NewStatus)
    {
        unityObjects["Status"].GetComponent<Text>().text = _NewStatus;
    }

    #endregion

    #region Controller

    public void Btn_Play()
    {
        Debug.Log("Play");
        unityObjects["Btn_Play"].SetActive(false);
        int _value = (int)(SC_MenuLogic.Instance.sliders["Screen_MultiPlayer_Slider"].GetComponent<Slider>().value * 10);
        if (!matchRoomData.ContainsKey("Password"))
        {
            matchRoomData.Add("Password", _value);
        }
        matchRoomData["Password"] = _value;
        UpdateStatus("Searching for room...");
        WarpClient.GetInstance().GetRoomsInRange(1, 2);
    }

    #endregion
}


