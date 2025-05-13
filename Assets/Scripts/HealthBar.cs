using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{




    public Slider slider;


    public void SetMaxHP(float max)
    {
        slider.maxValue = max;
        slider.value = max;
    }
   public void SetHealth(float health)
    {
        slider.value = health;
    }
}
