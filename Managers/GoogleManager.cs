using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using System;


public class GoogleManager : Singleton<GoogleManager>
{
    public TextMeshProUGUI logText;

    //void Start()
    //{
    //    PlayGamesPlatform.DebugLogEnabled = true;
    //    PlayGamesPlatform.Activate();
    //}

    //public void SignIn()
    //{
    //    PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) =>
    //    {
    //        if (result == SignInStatus.Success)
    //        {
    //            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
    //            string id = PlayGamesPlatform.Instance.GetUserId();
    //            string imgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();

    //            logText.text = "Sucess \n" + name;
    //        }
    //        else
    //        {
    //            logText.text = "Failed ";
    //        }
    //    });

    //}
}
