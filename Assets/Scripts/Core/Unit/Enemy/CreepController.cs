using System;
using System.Collections;
using UnityEngine;

public class CreepController : MonoBehaviour
{
    [SerializeField] public FactionId factionId;
    [SerializeField] public HealthController hpController;
    [SerializeField] public AttackArea attackArea;
    [SerializeField] public UnitStateId currentState;
    [SerializeField] public Animator modelAnimator;
    [SerializeField] public GameObject[] path;

    public int currentPathIndex = -1;
    public int nextDestinationIndex = -1;

    // unit info
    public float moveSpeed;
    public int damage = 1;

    public void SwitchState(UnitStateId designatedStateId, bool canOverride = false)
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
            case UnitStateId.None:
                break;
            case UnitStateId.Idle:
                break;
            case UnitStateId.Walk:
                break;
            case UnitStateId.Die:
                StartCoroutine(Die());
                break;
            case UnitStateId.Custom:
                break;
        }
    }

    public void OnStateExited()
    {
        switch (currentState)
        {
            case UnitStateId.None:
                break;
            case UnitStateId.Idle:
                break;
            case UnitStateId.Walk:
                break;
            case UnitStateId.Die:
                break;
            case UnitStateId.Custom:
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackArea.Attack += OnAttack;
        hpController.Initialize(factionId);
        attackArea.InitializeAttackArea(factionId, damage);
        //
        SetNextDestination(0);
        SwitchState(UnitStateId.Walk);
    }

    private void OnDestroy()
    {
        attackArea.Attack -= OnAttack;
    }

    private void OnAttack()
    {
        SwitchState(UnitStateId.Die);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case UnitStateId.None:
                break;
            case UnitStateId.Idle:

                break;
            case UnitStateId.Walk:
                MoveToNextDestination();
               
                if(Vector2.Distance(transform.position, path[nextDestinationIndex].transform.position) <= 0.01f )
                {
                    currentPathIndex = nextDestinationIndex;
                    if (nextDestinationIndex + 1 >= path.Length)
                    {
                        SwitchState(UnitStateId.Die);
                        return;
                    }
                    SetNextDestination(nextDestinationIndex + 1);
                }
                break;
            case UnitStateId.Die:
                hpController.gameObject.SetActive(false);
                break;
            case UnitStateId.Custom:
                break;
        }
    }

    public void SetNextDestination(int index)
    {
        nextDestinationIndex = index;
    }
    public void MoveToNextDestination()
    {
        transform.position = Vector2.MoveTowards(transform.position, path[nextDestinationIndex].transform.position, moveSpeed * Time.deltaTime);
    }

    private IEnumerator Die()
    {
        modelAnimator.PlayInFixedTime("Die");
        // wait for animation finished
        float normalizedTime = 0f;
        while (normalizedTime < 1f)
        {
            yield return new WaitForEndOfFrame();
            AnimatorStateInfo stateInfo = modelAnimator.GetCurrentAnimatorStateInfo(0);
            normalizedTime = stateInfo.normalizedTime;
        }

        Destroy(gameObject);
    }


}
