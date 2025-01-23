using CatDogEnums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterTooltip : UIPopUp
{

    [SerializeField] TextMeshProUGUI tierText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] TextMeshProUGUI atkText;
    [SerializeField] TextMeshProUGUI rangeText;
    [SerializeField] TextMeshProUGUI classText;

    [SerializeField] Image tooltipCardProfileImg;
    [SerializeField] Image tooltipCardImg;

    private Sprite dogSprite;
    private Sprite catSprite;

    private void OnEnable()
    {
        catSprite = ResourceManager.Instance.Load<Sprite>("Images/CatCard");
        dogSprite = ResourceManager.Instance.Load<Sprite>("Images/DogCard");
    }

    public void SetupTooltip(AssetIdType assetIdType, Image characterCardImg)
    {
        List<UnitData> unitDatas = DataManager.Instance.LoadUnitData();
        foreach (var item in unitDatas)
        {
            if (item.id == (int)assetIdType)
            {
                tierText.text = item.tier.ToString();
                nameText.text = item.DisplayName;
                speedText.text = item.spd.ToString();
                atkText.text = item.atk.ToString();
                rangeText.text = item.atkRange.ToString();
                classText.text = item.@class.ToString();

                Debug.Log("item.species" + item.species);

                if (item.species == "cat")
                {
                    tooltipCardImg.sprite = catSprite;
                }
                else
                {
                    tooltipCardImg.sprite = dogSprite;
                }

                    break;
            }
        }

        tooltipCardProfileImg.sprite = characterCardImg.sprite;
       
    }
}
