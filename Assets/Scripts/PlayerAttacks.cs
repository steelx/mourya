using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    AnimatorHandler animatorHandler;

    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }

    public void HandleLightAttack(WeaponItemSO weaponItem)
    {
        animatorHandler.PlayTargetAnimation(weaponItem.OH_Light_Attack_1, true);
    }

    public void HandleHeavyAttack(WeaponItemSO weaponItem)
    {
        animatorHandler.PlayTargetAnimation(weaponItem.OH_Heavy_Attack_1, true);
    }
}
