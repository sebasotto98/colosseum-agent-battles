using UnityEngine;

public enum WeaponMovementType
{
    SWING,
    POKE
}

[CreateAssetMenu(fileName = "newWeapon", menuName = "New Weapon Type")]
public class WeaponType : ScriptableObject
{
    public GameObject weaponPrefab;
    public Vector3 positionOffset;
    public Vector3 scaleOffset;

    public WeaponMovementType weaponAnimationType;
    public Vector2 hitBoxOffset;
    public Vector2 hitBoxSize;

    public float weaponRange = 1;
}
