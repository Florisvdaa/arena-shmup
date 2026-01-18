using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "SO's/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public GameObject bulletPrefab;
    public int overheatCap = 30;
    public int damage = 10;
    public float cooldownTime = 5f;
    public float fireRate = 5f; // shots per second
}
