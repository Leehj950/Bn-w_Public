using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopUp : UIBase
{
    public override void Init()
    {
    }

    public virtual void ClosePopupUI()
    {
        UIManager.Instance.ClosePopUpUI(this);
    }
}
