using UnityEngine;

[CreateAssetMenu(fileName = "HealthBoostEffect", menuName = "Scriptable Objects/Upgrade Effects/Health Boost")]
public class HealthBoostEffect : UpgradeEffect
{
    public float healthIncrease;

    public override void ApplyEffect(UpgradeStation station)
    {
        FirstPersonController player = FirstPersonController.Instance.GetComponent<FirstPersonController>();
        player.maxHealth += healthIncrease;
    }
}
