using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Canvas_NameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private Button button;

    public static string displayName { get; private set; }

    public void OnEnable()
    {
        // Lock the button on start (when the input field is empty)
        UpdateButton();
    }

    // <summary>When the user edits his display name update the button interactability</summary>
    public void UpdateButton()
    {
        button.interactable = !string.IsNullOrWhiteSpace(input.text);
    }

    // <summary>Set the display name and move to the main menu</summary>
    public void LockName()
    {
        displayName = input.text;
        CanvasController.Instance.SetMenu(CanvasController.MenuState.MainMenu);
    }
}
