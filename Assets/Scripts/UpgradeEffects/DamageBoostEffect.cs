using UnityEngine;

[CreateAssetMenu(fileName = "DamageBoostEffect", menuName = "Scriptable Objects/Upgrade Effects/Damage Boost")]
public class DamageBoostEffect : UpgradeEffect
{
    public float damageIncrease;

    public override void ApplyEffect(UpgradeStation station)
    {
        Pistol pistol = FindAnyObjectByType<Pistol>();
        pistol.damageBoost += damageIncrease;
    }
}
