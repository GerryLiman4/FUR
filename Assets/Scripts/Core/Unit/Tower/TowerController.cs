using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] public bool isAggro;
    [SerializeField] public SpriteRenderer attackRangeIndicator;
    [SerializeField] public FactionId factionId = FactionId.Player;

    // tower info
    [SerializeField] public float attackRange = 2.0f;
    [SerializeField] public LayerMask hitboxLayer;
    public int damage = 5;
    public float attackCooldown = 1.5f;

    public float attackTimer;
    public HealthController currentAttackTarget;

    protected virtual void Start()
    {
        attackTimer = attackCooldown;
        attackRangeIndicator.transform.localScale = new Vector2(attackRange, attackRange);
    }
    protected virtual void FixedUpdate()
    {
        if (!isAggro) return;

        // try to attack
        if (attackTimer <= 0f)
        {
            // attack same target
            if (currentAttackTarget != null && currentAttackTarget.CanBeTargeted() && factionId != currentAttackTarget.factionId)
            {
                Collider2D[] hitList = GetTargetList();
                foreach (Collider2D hit in hitList)
                {
                    HealthController targetHpController = hit.GetComponent<HealthController>();
                    if (targetHpController != null && targetHpController != currentAttackTarget)
                    {
                        currentAttackTarget = targetHpController;
                        Attack();
                        return;
                    }

                }
            }

            currentAttackTarget = null;
            Collider2D[] hits = GetTargetList();

            foreach (Collider2D hit in hits)
            {
                HealthController targetHpController = hit.GetComponent<HealthController>();
                if (targetHpController != null 
                    && factionId != targetHpController.factionId)
                {
                    currentAttackTarget = targetHpController;
                    break;
                }

            }

            if (currentAttackTarget != null && currentAttackTarget.CanBeTargeted()) Attack();
            return;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    protected virtual Collider2D[] GetTargetList()
    {
        // check enemy
        float radius = attackRange / 2f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            radius,
            hitboxLayer
        );

        return hits;
    }

    protected virtual void Attack()
    {
        currentAttackTarget.GetDamaged(gameObject, damage);
        attackTimer = attackCooldown;
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange / 2f);
    }

    private void OnMouseEnter()
    {
        attackRangeIndicator.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        attackRangeIndicator.gameObject.SetActive(false);
    }
}
