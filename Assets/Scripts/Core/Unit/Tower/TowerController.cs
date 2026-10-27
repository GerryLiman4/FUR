using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] public bool isAggro;
    [SerializeField] public SpriteRenderer attackRangeIndicator;

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
        attackRangeIndicator.transform.localScale = new Vector2(attackRange,attackRange);
    }
    protected virtual void FixedUpdate()
    {
        if (!isAggro) return;

        // try to attack
        if (attackTimer <= 0f)
        {
            // attack same target
            if (currentAttackTarget != null && currentAttackTarget.CanBeTargeted())
            {
                Collider2D[] hitList = GetTargetList();
                foreach (Collider2D hit in hitList)
                {
                    if (hit.TryGetComponent(out HealthController targetHpController))
                    {
                        if (targetHpController != null && targetHpController == currentAttackTarget)
                        {
                            currentAttackTarget = targetHpController;
                            Attack();
                            return;
                        }
                    }
                }
            }

            currentAttackTarget = null;
            Collider2D[] hits = GetTargetList();

            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent(out HealthController targetHpController))
                {
                    if (targetHpController != null)
                    {
                        currentAttackTarget = targetHpController;
                        break;
                    }
                }
            }

            if(currentAttackTarget != null && currentAttackTarget.CanBeTargeted()) Attack();
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
        currentAttackTarget.GetDamaged(gameObject,damage);
        attackTimer = attackCooldown;
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange / 2f);
    }
}
