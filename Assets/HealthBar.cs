using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    FirstPersonController player;
    [SerializeField] Image healthBarFill;
    [SerializeField] Image healthBarIndicator;
    [SerializeField] TextMeshProUGUI healthBarText;
    float timer = 0f;
    float indicatorStart = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthBarFill = transform.Find("Health")?.GetComponent<Image>();
        healthBarIndicator = transform.Find("Indicator")?.GetComponent<Image>();
        healthBarText = transform.Find("HealthText")?.GetComponent<TextMeshProUGUI>();
        player = FirstPersonController.Instance.GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 1f)
        {
            timer += Time.deltaTime;
            healthBarIndicator.fillAmount = Mathf.Lerp(indicatorStart, player.health / player.maxHealth, timer);
        }
    }

    public void UpdateHealthBar()
    {
        healthBarText.text = $"{player.health}/{player.maxHealth}";
        healthBarFill.fillAmount = player.health / player.maxHealth;
        indicatorStart = healthBarIndicator.fillAmount;
        timer = 0f;
    }
}
