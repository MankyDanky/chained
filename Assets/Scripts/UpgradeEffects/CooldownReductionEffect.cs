using UnityEngine;

[CreateAssetMenu(fileName = "CooldownReductionEffect", menuName = "Scriptable Objects/Upgrade Effects/Cooldown Reduction")]
public class CooldownReductionEffect : UpgradeEffect
{
    public float multiplier = 0.9f;
    public Pistol.AttackType attackType = Pistol.AttackType.Bullet;

    public override void ApplyEffect(UpgradeStation station)
    {
        Pistol pistol = FindAnyObjectByType<Pistol>();
        switch (attackType)
        {
            case Pistol.AttackType.Bullet:
                pistol.fireCooldown *= multiplier;
                break;
            case Pistol.AttackType.Zag:
                pistol.secondaryFireCooldown *= multiplier;
                break;
            case Pistol.AttackType.Grenade:
                pistol.grenadeCooldown *= multiplier;
                break;
            default:
                break;
        }
    }
}
