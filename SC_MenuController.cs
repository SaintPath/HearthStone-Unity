using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SC_MenuController : MonoBehaviour
{
    public SC_MenuLogic sc_MenuLogic;
    #region Menus
    public void ChangeScreen(string _Screen)
    {
        SC_GlobalEnums.Screens _screen = (SC_GlobalEnums.Screens)SC_GlobalEnums.Screens.Parse(typeof(SC_GlobalEnums.Screens), _Screen);
        if (sc_MenuLogic != null)
            sc_MenuLogic.ChangeScreen(_screen);
    }

    public void Back()
    {
        if (sc_MenuLogic != null)
            sc_MenuLogic.Back();
    }

    public void Screen_Multiplayer_Slider()
    {
        if (sc_MenuLogic != null)
            sc_MenuLogic.Screen_Multiplayer_Slider();
    }

    public void Screen_Options_Slider_Music()
    {
        if (sc_MenuLogic != null)
            sc_MenuLogic.Screen_Options_Slider_Music();
    }

    public void Screen_Options_Slider_Sfx()
    {
        if (sc_MenuLogic != null)
            sc_MenuLogic.Screen_Options_Slider_Sfx();
    }

    public void openUrl()
    {
        sc_MenuLogic.openUrl();
    }

    #endregion

    #region Single Player
    public void EndTurn()
    {
        if (sc_MenuLogic != null)
            sc_MenuLogic.EndTurn();
    }

    public void Restart()
    {
        if (sc_MenuLogic != null)
            sc_MenuLogic.Restart();
    }

    public void gameover()
    {
        if (sc_MenuLogic != null)
            sc_MenuLogic.gameover();
    }

    #endregion


    #region Multi Player
    public void Btn_Play()
    {
        SC_Menu.Instance.Btn_Play();
    }

    #endregion

}
