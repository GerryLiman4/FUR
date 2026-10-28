using UnityEngine;

public class BaseController : MonoBehaviour
{
    [SerializeField] public HealthController hpController;
    [SerializeField] public FactionId factionId;

    private void Start()
    {
        hpController.Damaged += OnDamaged;
        hpController.Died += OnDied;
        hpController.Initialize(factionId);
    }

    private void OnDied()
    {
        SignalManager.PlayerBaseDestroyed();
        Destroy(gameObject);
    }

    private void OnDamaged()
    {
        
    }

    private void OnDestroy()
    {
        hpController.Damaged -= OnDamaged;
        hpController.Died -= OnDied;
    }
}
