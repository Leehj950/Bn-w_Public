using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    private Slider currentHpSlider;
    private Slider backGroundSlider;
    public Slider CurrentHpSlider { get { return currentHpSlider; } private set { } }
    public Slider BackGroundSlider { get { return backGroundSlider; } private set { } }

    private void Awake()
    {
        var sliders = GetComponentsInChildren<Slider>();

        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i].name == "HpSlider")
            {
                currentHpSlider = sliders[i];
            }
            else
            {
                backGroundSlider = sliders[i];
            }
        }
    }
}

