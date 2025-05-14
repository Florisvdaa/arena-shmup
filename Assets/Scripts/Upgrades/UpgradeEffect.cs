using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeEffectType { HealthPercentage, SpeedFlat, FireRateDecrease, ExpMultiplier}
[System.Serializable]
public class UpgradeEffect
{
    public UpgradeEffectType effectType;
    public float value;
}
