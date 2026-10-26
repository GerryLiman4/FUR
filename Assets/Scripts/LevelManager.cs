using System;
using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] public GameStateId currentState;

    [Header("HUD")]
    [SerializeField] public NotificationPanel notificationUI;

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
                
                StartCoroutine(notificationUI.ShowNotification("Standby Phase"));
                Action WaitStandbyPhase = null;
                WaitStandbyPhase = () => { 
                    notificationUI.notificationFinished -= WaitStandbyPhase;
                    SwitchState(GameStateId.Deploy);
                };
                notificationUI.notificationFinished += WaitStandbyPhase;
                break;
            case GameStateId.Deploy:
                StartCoroutine(notificationUI.ShowNotification("Deploy Phase"));
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
        SwitchState(GameStateId.Standby);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
