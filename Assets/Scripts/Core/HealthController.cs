using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour, IDamagable
{
    [SerializeField] public int currentHp;
    [SerializeField] public int maxHp = 5;
    [SerializeField] public Collider2D hitbox;
    [SerializeField] public Image healthBar;

    public FactionId factionId;

    public event Action Damaged;
    public event Action Died;

    public void Initialize(FactionId designatedFactionId)
    {
        factionId = designatedFactionId;
        currentHp = maxHp;
        hitbox.enabled = true;
        healthBar.fillAmount = ((float)currentHp / (float)maxHp);
    }

    public void GetDamaged(GameObject damager, int damage)
    {
        currentHp = (int)Mathf.Max((currentHp - damage) , 0f);
        healthBar.fillAmount = ((float)currentHp / (float)maxHp);
        if (currentHp <= 0f)
        {
            hitbox.enabled = false;
            Died?.Invoke();
            return;
        }
        Damaged?.Invoke();
    }

    private void OnDestroy()
    {
        Damaged = null;
        Died = null;
    }

    public FactionId GetFactionId()
    {
        return factionId;
    }

    public bool CanBeTargeted()
    {
        return currentHp > 0 && hitbox.enabled;
    }
}
