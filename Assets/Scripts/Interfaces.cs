using UnityEngine.InputSystem;

public interface IEquippable
{
    void Equip();
    void Unequip();
    void HandleInput(InputAction.CallbackContext context);
}