using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RenameUI : MonoBehaviour
{
    public event Action<string> OnNameChanged;


    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private TMP_InputField newNameInputField;
    [SerializeField] GameObject background;

    private void Awake()
    {
        Hide();

        confirmBtn.onClick.AddListener(async () =>
        {
            if (string.IsNullOrEmpty(newNameInputField.text))
            {
                Debug.LogWarning("Name cannot be empty.");
                return;
            }

            newNameInputField.text.Replace(" ", "_"); // cant have spaces

            await AuthenticationWrapper.RenamePlayerName(newNameInputField.text);

            OnNameChanged?.Invoke(newNameInputField.text);

            Hide();
        });

        cancelBtn.onClick.AddListener(() =>
        {
            Hide();
            newNameInputField.text = "";
        });

    }

    private void Hide()
    {
        background.SetActive(false);
    }

    public void Show()
    {
        background.SetActive(true);
    }
}
