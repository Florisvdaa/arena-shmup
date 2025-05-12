using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * ScriptableObject to define temporary power-ups.
 */
public enum PowerUpType { Shotgun, ExplosiveRounds, SlowMotion, OverShield, Afterburners, RegenField, EMPBlast, Decoy }

[CreateAssetMenu(menuName = "Game/PowerUps/PowerUpDefinition")]
public class PowerUpDefinition : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public PowerUpType type;
    [Tooltip("Duration in seconds for this power-up")] 
    public float duration;
    [Tooltip("Generic value parameter (e.g. damage multiplier, radius, speed scale)")] 
    public float value;
    [Range(1, 3)]
    [Tooltip("Cost for the powerup")]
    public int cost;
}
