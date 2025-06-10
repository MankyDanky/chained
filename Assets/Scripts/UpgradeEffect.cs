using UnityEngine;

public abstract class UpgradeEffect : ScriptableObject
{
    public abstract void ApplyEffect(UpgradeStation station);
}
