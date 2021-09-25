using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    WeaponSlotManager weaponSlotManager;

    public WeaponItemSO leftWeapon;
    public WeaponItemSO rightWeapon;

    private void Awake()
    {
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    private void Start()
    {
        weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
    }
}
