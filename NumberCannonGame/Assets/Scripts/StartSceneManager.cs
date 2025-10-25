using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    // Assign this to your "Start Game" button's OnClick() event
    public void StartGame()
    {
        // Reset score and level for a new game
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("CurrentScore", 0);
        
        // Also reset ammo for a fresh start
        PlayerPrefs.SetInt("Ammo1", 50);
        PlayerPrefs.SetInt("Ammo2", 50);
        PlayerPrefs.SetInt("Ammo3", 50);

        SceneManager.LoadScene("GameScene");
    }
}