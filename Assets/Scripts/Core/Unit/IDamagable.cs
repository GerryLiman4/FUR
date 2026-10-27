using UnityEngine;

public interface IDamagable
{
    public void GetDamaged(GameObject damager, int damage);
    public FactionId GetFactionId();
    public void Initialize(FactionId designatedFactionId);

    public bool CanBeTargeted();
}
