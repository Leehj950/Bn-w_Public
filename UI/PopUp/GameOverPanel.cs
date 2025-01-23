using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : UIPopUp
{
    public GameObject winUI;
    public GameObject loseUI;

    public void ShowWin()
    {
        winUI.SetActive(true);
    }

    public void ShowLose()
    {
        loseUI.SetActive(true); 
    }

    public void HideGameOver()
    {
        winUI.SetActive(false);
        loseUI.SetActive(false);
    }
}
