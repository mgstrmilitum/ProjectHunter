using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;

[CreateAssetMenu(fileName ="NewGunData",menuName ="Gun/GunData")]
public class WeaponData : ScriptableObject
{
    public string gunName;

    public LayerMask targetLayerMask;

    [Header("-----Shoot Info-----")]
    public float shootingRange;
    public float fireRate;

    [Header("-----Reload Info-----")]
    public int magazineSize;
    public float timeToReload;

}
