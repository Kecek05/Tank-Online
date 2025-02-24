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

            string fixednewNameInputField = newNameInputField.text.Replace(" ", "_"); // cant have spaces

            await AuthenticationWrapper.RenamePlayerName(fixednewNameInputField);

            ClientSingleton.Instance.GameManager.UpdateUserDataName();

            OnNameChanged?.Invoke(fixednewNameInputField);

            Hide();
        });

        cancelBtn.onClick.AddListener(() =>
        {
            Hide();
        });

    }

    public void Hide()
    {
        background.SetActive(false);
        newNameInputField.text = "";
    }

    public void Show()
    {
        background.SetActive(true);
    }

}
