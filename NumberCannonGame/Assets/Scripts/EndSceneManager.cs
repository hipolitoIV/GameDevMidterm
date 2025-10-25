using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndSceneManager : MonoBehaviour
{
    public TMP_Text finalScoreText;
    public TMP_Text highScoreText;

    void Start()
    {
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        finalScoreText.text = "Your Score: " + finalScore;
        highScoreText.text = "High Score: " + highScore;
    }

    // Assign this to your "Restart Game" button's OnClick() event
    public void RestartGame()
    {
        SceneManager.LoadScene("StartScene");
    }
}