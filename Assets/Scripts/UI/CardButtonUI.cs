using System;
using UnityEngine;
using UnityEngine.UI;

public class CardButtonUI : MonoBehaviour
{
    [SerializeField] public Button button;
    [SerializeField] public int id;
    [SerializeField] public int price = 100;

    public event Action<int> ButtonPressed;

    public void Initialize(int designatedId)
    {
        id = designatedId;
    }
    public void OnButtonPressed()
    {
        ButtonPressed?.Invoke(id);
    }
}
