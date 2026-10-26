using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists in the scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Kill duplicates
            return;
        }

        Instance = this;

        // Optional: Keep this manager alive when loading new scenes
        DontDestroyOnLoad(gameObject);
    }
}
