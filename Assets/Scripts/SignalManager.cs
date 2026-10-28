using System;

public static class SignalManager 
{
    public static event Action OnPlayerBaseDestroyed;
    public static void PlayerBaseDestroyed()
    {
        OnPlayerBaseDestroyed?.Invoke();
    }

}
