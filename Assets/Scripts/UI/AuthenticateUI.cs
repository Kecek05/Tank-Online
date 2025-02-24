using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticateUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI authTxt;
    [SerializeField] private GameObject buttonsParent;

    [SerializeField] private Button authUnityBtn;
    [SerializeField] private Button authAnonymously;

    private void Awake()
    {
        ShowButtons();
        HideTxt();

        authUnityBtn.onClick.AddListener(async () =>
        {
            HideButtons();
            ShowTxt();
            await ClientSingleton.Instance.AuthClient();
        });
        authAnonymously.onClick.AddListener(async () =>
        {
            HideButtons();
            ShowTxt();
            await ClientSingleton.Instance.AuthClientAnonymously();
        });
    }

    private void Start()
    {
        AuthenticationWrapper.OnSignInFail += AuthenticationWrapper_OnSignInFail;
    }

    private void AuthenticationWrapper_OnSignInFail()
    {
        HideTxt();
        ShowButtons();
    }

    private void HideTxt()
    {
        authTxt.gameObject.SetActive(false);
    }

    private void ShowTxt()
    {
        authTxt.gameObject.SetActive(true);
    }

    private void HideButtons()
    {
        buttonsParent.SetActive(false);
    }

    private void ShowButtons()
    {
        buttonsParent.SetActive(true);
    }
}
