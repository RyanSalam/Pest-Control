using UnityEngine.InputSystem;
using UnityEngine;

public interface IEquippable
{
    void Equip();
    void Unequip();
    void HandleInput(InputAction.CallbackContext context);
    Animator GetAnimator();
}