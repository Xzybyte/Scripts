using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    [SerializeField] private Text text;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        text.text = health + " / " + health + " HP";
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        text.text = health + " / " + slider.maxValue + " HP";
    }
}
