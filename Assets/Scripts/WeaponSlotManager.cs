using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    WeaponHolderSlot leftHandSlot;
    WeaponHolderSlot rightHandSlot;
    private void Awake()
    {
        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach(WeaponHolderSlot slot in weaponHolderSlots)
        {
            if (slot.isLeftHandSlot)
            {
                leftHandSlot = slot;
            }
            else if (slot.isRightHandSlot)
            {
                rightHandSlot = slot;
            }
        }
    }

    public void LoadWeaponOnSlot(WeaponItemSO weapon, bool isLeft)
    {
        if (isLeft)
        {
            leftHandSlot.LoadWeaponModel(weapon);
        }
        else
        {
            rightHandSlot.LoadWeaponModel(weapon);
        }
    }
}
