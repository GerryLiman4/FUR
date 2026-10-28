using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        gm.LoadLevel();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
