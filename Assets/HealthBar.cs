using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    FirstPersonController player;
    [SerializeField] Material healthBarFillMaterial;
    [SerializeField] Material healthBarIndicatorMaterial;
    [SerializeField] TextMeshProUGUI healthBarText;
    float timer = 0f;
    float indicatorStart = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthBarFillMaterial = transform.Find("Health")?.GetComponent<Image>().material;
        healthBarIndicatorMaterial = transform.Find("Indicator")?.GetComponent<Image>().material;
        healthBarFillMaterial.SetFloat("_Health", 1f);
        healthBarIndicatorMaterial.SetFloat("_Health", 1f);
        healthBarText = transform.Find("HealthText")?.GetComponent<TextMeshProUGUI>();
        player = FirstPersonController.Instance.GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 1f)
        {
            timer += Time.deltaTime;
            healthBarIndicatorMaterial.SetFloat("_Health", Mathf.Lerp(indicatorStart, player.health/player.maxHealth, timer));
        }
    }

    public void UpdateHealthBar()
    {
        healthBarText.text = $"{player.health}/{player.maxHealth}";
        healthBarFillMaterial.SetFloat("_Health", player.health / player.maxHealth);
        indicatorStart = healthBarIndicatorMaterial.GetFloat("_Health");
        timer = 0f;
    }
}
