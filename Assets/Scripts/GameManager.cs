using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
