using System;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] public GameStateId currentState;

    [SerializeField] public int currentGold;

    [Header("HUD")]
    [SerializeField] public NotificationPanel notificationUI;
    [SerializeField] public CardListUI cardListUI;
    [SerializeField] public GameObject deployUI;
    [SerializeField] public TextMeshProUGUI goldText;

    public void SwitchState(GameStateId designatedStateId, bool canOverride = false)
    {
        if (currentState == designatedStateId && !canOverride) return;

        OnStateExited();
        currentState = designatedStateId;
        OnStateEntered();
    }

    public void OnStateEntered()
    {
        switch (currentState)
        {
            case GameStateId.None:
                break;
            case GameStateId.Standby:
                notificationUI.gameObject.SetActive(true);
                StartCoroutine(notificationUI.ShowNotification("Standby Phase"));
                Action WaitStandbyPhase = null;
               
                WaitStandbyPhase = () => { 
                    notificationUI.notificationFinished -= WaitStandbyPhase;
                    SwitchState(GameStateId.Deploy, true);
                };
                notificationUI.notificationFinished += WaitStandbyPhase;
                break;
            case GameStateId.Deploy:
                notificationUI.gameObject.SetActive(true);
                StartCoroutine(notificationUI.ShowNotification("Deploy Phase"));
                cardListUI.ResetSelected();
                deployUI.SetActive(true);
                break;
            case GameStateId.Wave:
                StartCoroutine(notificationUI.ShowNotification("Wave Phase"));
                break;
            case GameStateId.Finish:
                break;
        }
    }

    public void OnStateExited()
    {
        switch (currentState)
        {
            case GameStateId.None:
                break;
            case GameStateId.Standby:
                break;
            case GameStateId.Deploy:
                deployUI.SetActive(false);
                cardListUI.ResetSelected();
                break;
            case GameStateId.Wave:
                break;
            case GameStateId.Finish:
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentGold = 200;
        goldText.text = currentGold.ToString();
        SwitchState(GameStateId.Standby);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case GameStateId.None:
                break;
            case GameStateId.Standby:
                break;
            case GameStateId.Deploy:
                UnitId selectedUnitId = cardListUI.GetSelectedUnitId();

                if(selectedUnitId != UnitId.None)
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePosition,Vector2.zero,0f);

                    if (hit.collider != null)
                    {
                        if (hit.collider.TryGetComponent(out TilePiece targetTile))
                        {
                            if (targetTile != null)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    PlaceTower(targetTile,selectedUnitId);
                                }
                            }
                        }
                    }

                }
                break;
            case GameStateId.Wave:
                break;
            case GameStateId.Finish:
                break;
        }
    }

    private void PlaceTower(TilePiece targetTile , UnitId selectedUnitId)
    {
        if(targetTile.canBeOccupied && targetTile.isOccupied == false)
        {
            // check if gold enough 
            int cost = cardListUI.GetUnitCost();

            if (currentGold < cost) return;

            currentGold = Math.Max(currentGold - cost, 0);
            goldText.text = currentGold.ToString();

            GameObject towerPrefab = Resources.Load<GameObject>("Tower/" + selectedUnitId);
            GameObject tower = Instantiate(towerPrefab, Vector3.zero, Quaternion.identity);
            targetTile.OccupyTile(tower);
        }
    }
}
