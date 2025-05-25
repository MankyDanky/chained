using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    FirstPersonController player;
    [SerializeField] Material healthBarFillMaterial;
    [SerializeField] TextMeshProUGUI healthBarText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthBarFillMaterial = transform.Find("Health")?.GetComponent<Image>().material;
        healthBarText = transform.Find("HealthText")?.GetComponent<TextMeshProUGUI>();
        player = FirstPersonController.Instance.GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBarText.text = $"{player.health}/{player.maxHealth}";
        healthBarFillMaterial.SetFloat("_Health", player.health / player.maxHealth);
    }
}
