using System;
using UnityEngine;

public class CardListUI : MonoBehaviour
{
    [SerializeField] public CardButtonUI[] cardList;

    public event Action<int> SelectTower;
    public int selectedCardTowerId = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (CardButtonUI cardButtonUI in cardList)
        {
            cardButtonUI.ButtonPressed += OnCardSelected;
        }
    }

    private void OnCardSelected(int designatedId)
    {
        selectedCardTowerId = designatedId;
        SelectTower?.Invoke(selectedCardTowerId);
    }

    public void ResetSelected()
    {
        selectedCardTowerId = -1;
    }

    public UnitId GetSelectedUnitId()
    {
        if (selectedCardTowerId <= 0) return UnitId.None;

        return (UnitId)selectedCardTowerId;
    }

    public int GetUnitCost()
    {
        if (selectedCardTowerId <= 0) return 0;

        foreach (CardButtonUI cardButtonUI in cardList) 
        {
            if(cardButtonUI.id == selectedCardTowerId) return cardButtonUI.price;
        }

        return 0;
    }
}
