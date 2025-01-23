using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    private void Start()
    {
        inputReader.OnPrimaryFireEvent += InputReader_OnPrimaryFireEvent; ;
        inputReader.OnMoveEvent += InputReader_OnMoveEvent; ;
    }

    private void InputReader_OnMoveEvent(Vector2 obj)
    {
        Debug.Log(obj);
    }

    private void InputReader_OnPrimaryFireEvent(bool obj)
    {
        Debug.Log(obj);
    }
}
