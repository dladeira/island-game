using UnityEngine;

public class PlayerCurrentWeapon : MonoBehaviour
{
    [SerializeField] private PlayerHotbar hotbar;

    public void SwingWeapon()
    {
        hotbar.SwingWeapon();
    }
}
