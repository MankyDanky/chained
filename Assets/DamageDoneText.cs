using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageDoneText : MonoBehaviour
{
    enum DamageType
    {
        Pistol,
        Zag,
        Grenade
    }
    
    TMP_Text tmpText;
    [SerializeField] DamageType damageType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        if (GameManager.Instance == null) return;
        switch (damageType)
        {
            case DamageType.Pistol:
                tmpText.text = $"PISTOL DAMAGE DONE: {GameManager.Instance.bulletDamageDone}";
                break;
            case DamageType.Zag:
                tmpText.text = $"ZAG DAMAGE DONE: {GameManager.Instance.zagDamageDone}";
                break;
            case DamageType.Grenade:
                tmpText.text = $"GRENADE DAMAGE DONE: {GameManager.Instance.grenadeDamageDone}";
                break;
        }
    }
}
