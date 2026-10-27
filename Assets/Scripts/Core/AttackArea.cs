using System;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [SerializeField] public int damage;
    [SerializeField] public FactionId factionId;

    public event Action Attack;

    public void InitializeAttackArea(FactionId designatedFactionId, int designatedDamage)
    {
        factionId = designatedFactionId;
        damage = designatedDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthController healthController = collision.GetComponent<HealthController>();

        if (healthController.GetFactionId() == factionId) return;

        healthController.GetDamaged(transform.parent.gameObject , damage);
        Attack?.Invoke();

        Destroy(gameObject);
    }
}
