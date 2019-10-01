using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SC_CardDisplay: MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler
{
    public Card cardStats;
    public Card card;
    public Transform returnTo = null;
    public Transform parantHolder = null;
    GameObject placeholder = null;

    void Start()
    {
        card = (Card)ScriptableObject.CreateInstance("Card");
        Text[] _children = GetComponentsInChildren<Text>();
        Image[] _image = GetComponentsInChildren<Image>();

        _image[1].sprite = cardStats.artwork;
        _children[0].text = cardStats.name;
        _children[1].text = cardStats.description;
        _children[2].text = cardStats.manaCost.ToString();
        _children[3].text = cardStats.attack.ToString();
        _children[4].text = cardStats.health.ToString();

        card.artwork = cardStats.artwork;
        card.name = cardStats.name;
        card.description = cardStats.description;
        card.manaCost = cardStats.manaCost;
        card.attack = cardStats.attack;
        card.health = cardStats.health;

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (this.transform.parent.name == "Player_One_Field" || this.transform.parent.name == "Player_Two_Field")
            return;

        if (GlobalVariables.CardsPlayedbyPlayerOne == 2 && SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerOne")
            return;

        if (GlobalVariables.CardsPlayedbyPlayerTwo == 2 && SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerTwo")
            return;

        if (this.transform.parent.name == "Player_One_Hand" && SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerTwo")
            return;

        if (this.transform.parent.name == "Player_Two_Hand" && SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerOne")
            return;

        if (!SC_MenuLogic.Instance.isMyTurn)
            return;

        #region Creating a placeholder
        placeholder = new GameObject();
        placeholder.transform.SetParent(this.transform.parent);
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleWidth = 0;
        #endregion

        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex()); //Order of the sibling whithin a parent


        Cursor.visible = false;
        returnTo = this.transform.parent;
        parantHolder = returnTo;
        this.transform.SetParent(this.transform.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (this.transform.parent.name == "Player_One_Field" || this.transform.parent.name == "Player_Two_Field")
            return;

        if (GlobalVariables.CardsPlayedbyPlayerOne == 2 && SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerOne")
            return;

        if (GlobalVariables.CardsPlayedbyPlayerTwo == 2 && SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerTwo")
            return;

        if (this.transform.parent.name == "Player_One_Hand" && SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerTwo")
            return;

        if (this.transform.parent.name == "Player_Two_Hand" && SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerOne")
            return;

        if (!SC_MenuLogic.Instance.isMyTurn)
            return;

        int _sibilingIndex = parantHolder.childCount;

        this.transform.position = eventData.position;

        if (placeholder.transform.parent != parantHolder)
            placeholder.transform.SetParent(parantHolder);

        for (int i = 0; i < parantHolder.childCount; i++)
        {
            if (this.transform.position.x < parantHolder.GetChild(i).position.x)
            {
                _sibilingIndex = i;

                if(placeholder.transform.GetSiblingIndex() < _sibilingIndex)
                    _sibilingIndex--;

                break;
            }
        }
        placeholder.transform.SetSiblingIndex(_sibilingIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (this.transform.parent.name == "Player_One_Field" || this.transform.parent.name == "Player_Two_Field")
            return;

        if (GlobalVariables.CardsPlayedbyPlayerOne == 2 && SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerOne")
            return;

        if (GlobalVariables.CardsPlayedbyPlayerTwo == 2 && SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerTwo")
            return;

        if (this.transform.parent.name == "Player_One_Hand" && SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerTwo")
            return;

        if (this.transform.parent.name == "Player_Two_Hand" && SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerOne")
            return;

        if (!SC_MenuLogic.Instance.isMyTurn)
            return;

        if (this.parantHolder.name == "Player_One_Field")
        {
            GlobalVariables.CardsPlayedbyPlayerOne++;
            SC_MenuLogic.Instance.toSend.Add("NewCard"+ GlobalVariables.CardsPlayedbyPlayerOne, this.card.name);
        }

        if (this.parantHolder.name == "Player_Two_Field")
        {
            GlobalVariables.CardsPlayedbyPlayerTwo++;
        }

        Cursor.visible= true;
        this.transform.SetParent(returnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        this.transform.localScale = new Vector3(0.3f, 0.3f, 0);
        Destroy(placeholder);   
    }

   
    public void OnClick()
    {
        if (this.transform.parent.name == "Player_One_Hand")
            return;

        if (!SC_MenuLogic.Instance.isMyTurn)
            return;

        if (this.transform.parent.name == "Player_One_Field" && 
            SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerOne")
        {
            GlobalVariables.PlayerOneAttack = this.card.attack;
            this.GetComponent<Button>().interactable = false;
            GlobalVariables.cardSelected = true;
        }

        if(this.transform.parent.name == "Player_Two_Field" && 
            SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerOne" && 
            GlobalVariables.cardSelected == true)
        {
            this.card.health -= GlobalVariables.PlayerOneAttack;
            this.transform.Find("Health").GetComponent<Text>().text = this.card.health.ToString();
            GlobalVariables.PlayerOneAttack = 0;
            if (this.card.health <= 0)
                Destroy(this.gameObject);


            if (!SC_MenuLogic.Instance.toSend.ContainsKey("Attack"))
            {
                SC_MenuLogic.Instance.toSend.Add("Attack", SC_MenuLogic.Instance.attackNumber);
            }
            SC_MenuLogic.Instance.toSend.Add("CardName" + SC_MenuLogic.Instance.attackNumber, this.card.name);
            SC_MenuLogic.Instance.toSend.Add("HealthLeft" + SC_MenuLogic.Instance.attackNumber, this.card.health);
            SC_MenuLogic.Instance.toSend["Attack"] = SC_MenuLogic.Instance.attackNumber;
            SC_MenuLogic.Instance.attackNumber++;
            GlobalVariables.cardSelected = false;
            
        }

        if (this.transform.parent.name == "Player_Two_Field" && 
            SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerTwo" && 
            SC_MenuLogic.Instance.singlePlayer)
        {
            GlobalVariables.PlayerTwoAttack = this.card.attack;
            this.GetComponent<Button>().interactable = false;
            GlobalVariables.cardSelected = true;
        }

        #region Single Player
        if (this.transform.parent.name == "Player_One_Field" && 
            SC_MenuLogic.Instance.currentTurn.ToString() == "PlayerTwo" && 
            GlobalVariables.cardSelected == true && SC_MenuLogic.Instance.singlePlayer)
        {
            this.card.health -= GlobalVariables.PlayerTwoAttack;
            this.transform.Find("Health").GetComponent<Text>().text = this.card.health.ToString();
            GlobalVariables.PlayerTwoAttack = 0;
            if (this.card.health <= 0)
                Destroy(this.gameObject);
            GlobalVariables.cardSelected = false;
        }
        #endregion
    }
}
