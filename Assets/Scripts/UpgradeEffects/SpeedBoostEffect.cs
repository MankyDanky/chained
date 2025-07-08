using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoostEffect", menuName = "Scriptable Objects/Upgrade Effects/Speed Boost")]
public class SpeedBoostEffect : UpgradeEffect
{
    public float speedIncrease;

    public override void ApplyEffect(UpgradeStation station)
    {
        FirstPersonController player = FirstPersonController.Instance.GetComponent<FirstPersonController>();
        player.speed += speedIncrease;
    }
}
