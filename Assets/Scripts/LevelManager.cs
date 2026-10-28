using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] public GameStateId currentState;

    [SerializeField] public int maxRound= 3;
    [SerializeField] public int currentGold;
    [SerializeField] public int currentRound = 1;

    [Header("Round Management")]
    [SerializeField] public GameObject spawnRoot;
    [SerializeField] public int creepQuantity = 5;
    [SerializeField] public float creepMultiplier = 2f;

    [Header("HUD")]
    [SerializeField] public NotificationPanel notificationUI;
    [SerializeField] public CardListUI cardListUI;
    [SerializeField] public GameObject deployUI;
    [SerializeField] public TextMeshProUGUI goldText;
    [SerializeField] public TextMeshProUGUI roundText;
    [SerializeField] public GameObject gameOverUI;

    public bool isGameOver = false;
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
                currentRound += 1;
                roundText.text = currentRound.ToString();
                StartCoroutine(notificationUI.ShowNotification("Wave Phase"));
                StartCoroutine(DoRoundWave());
                break;
            case GameStateId.Finish:
                // check gameover condition
                if (currentRound >= maxRound || isGameOver)
                {
                    gameOverUI.SetActive(true);
                    return;
                }
                notificationUI.gameObject.SetActive(true);
                StartCoroutine(notificationUI.ShowNotification("Finish"));

                Action WaitPhase = null;

                WaitPhase = () => {
                    notificationUI.notificationFinished -= WaitPhase;
                    SwitchState(GameStateId.Standby, true);
                };

                notificationUI.notificationFinished += WaitPhase;
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
                foreach (CreepController creep in creepControllerList)
                {
                    if(creep != null ) Destroy(creep.gameObject);
                    
                }
                creepControllerList.Clear();
                break;
            case GameStateId.Finish:
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goldText.text = currentGold.ToString();
        roundText.text = currentRound.ToString();

        SignalManager.OnPlayerBaseDestroyed += OnPlayerBaseDestroyed;

        SwitchState(GameStateId.Standby);
    }

    private void OnPlayerBaseDestroyed()
    {
        isGameOver = true;
        SwitchState(GameStateId.Finish, true);
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

    public List<CreepController> creepControllerList = new List<CreepController>();
    public GameObject[] enemyPath;
    public void StartWave()
    {
        creepControllerList.Clear();
        cardListUI.ResetSelected();
        SwitchState(GameStateId.Wave);
    }

    public IEnumerator DoRoundWave()
    {
        int maxCreep = (int)((float)creepQuantity * currentRound);
        while (creepControllerList.Count < maxCreep)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 2f));
            int randomId = UnityEngine.Random.Range((int)UnitId.Pawn, (int)UnitId.Monk + 1);

            CreepController enemyPrefab = Resources.Load<CreepController>("Enemy/" + (UnitId)randomId);
            CreepController enemy = Instantiate<CreepController>(enemyPrefab, spawnRoot.transform.position, Quaternion.identity);

            enemy.path = enemyPath;
            enemy.hpController.Died += OnEnemyKilled;

            creepControllerList.Add(enemy);
        }
    }

    private void OnEnemyKilled()
    {
        currentGold += UnityEngine.Random.Range(20, 26);
        goldText.text = currentGold.ToString();

        bool isRoundOver = true;
        foreach(CreepController creep in creepControllerList)
        {
            //print(creep != null && creep.hpController.currentHp > 0);
            //print(creep.hpController.currentHp);
            if(creep != null && creep.hpController.currentHp > 0)
            {
                isRoundOver = false;
            }
        }

        if (isRoundOver && !isGameOver) SwitchState(GameStateId.Finish);
    }

    private void OnDestroy()
    {
        SignalManager.OnPlayerBaseDestroyed -= OnPlayerBaseDestroyed;
    }

    public void Retry()
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        gm.LoadLevel();
    }
    public void GoToMainMenu()
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        gm.LoadMainMenu();
    }
}
