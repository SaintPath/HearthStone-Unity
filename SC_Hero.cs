using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_Hero : MonoBehaviour
{
    SC_GlobalEnums.turns turns = 0;
    private int Health = 30;
    public SC_MenuController Controller;

    public void OnClick()
    {
        if (this.transform.name == "Player_Two_Hero" && turns.ToString() == "PlayerOne")
        {
            Health -= GlobalVariables.PlayerOneAttack;
            this.transform.Find("Health").GetComponent<Text>().text = Health.ToString();

            if (!SC_MenuLogic.Instance.toSend.ContainsKey("Hero"))
                SC_MenuLogic.Instance.toSend.Add("Hero", Health);

            SC_MenuLogic.Instance.toSend["Hero"] = Health;

            GlobalVariables.PlayerOneAttack = 0;
            if(Health <= 0)
            {
                Controller.gameover();
                Dictionary<string, object> toSend = new Dictionary<string, object>();
                toSend.Add("GameOver", true);
                string _send = MiniJSON.Json.Serialize(toSend);
                WarpClient.GetInstance().sendMove(_send);
                WarpClient.GetInstance().stopGame();
            }
                
        }

        if (this.transform.name == "Player_One_Hero" && turns.ToString() == "PlayerTwo")
        {
            Health -= GlobalVariables.PlayerTwoAttack;
            this.transform.Find("Health").GetComponent<Text>().text = Health.ToString();
            GlobalVariables.PlayerTwoAttack = 0;
            if (Health <= 0)
            {
                Controller.gameover();
                WarpClient.GetInstance().stopGame();
            }
        }
    }

    public void Restart()
    {
        Health = 30;
        this.transform.Find("Health").GetComponent<Text>().text = Health.ToString();
    }
}
