using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenLevel : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    [SerializeField] private Text text;

    public void SetMaxOxygen(int oxygen)
    {
        slider.maxValue = oxygen;
        slider.value = oxygen;
        text.text = oxygen + " / " + oxygen + "";
    }

    public void SetOxygen(int oxygen)
    {
        slider.value = oxygen;
        text.text = oxygen + " / " + slider.maxValue + "";
    }
}
