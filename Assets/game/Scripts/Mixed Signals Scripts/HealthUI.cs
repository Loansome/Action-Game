using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    private Image healthUI;

    void Awake()
    {
        healthUI = GameObject.FindWithTag("HealthUI").GetComponent<Image>();
    }

    public void DisplayHealth(float value)
    {
        value /= 1000f;
        if (value < 0f)
            value = 0f;

        healthUI.fillAmount = value;
    }
}
