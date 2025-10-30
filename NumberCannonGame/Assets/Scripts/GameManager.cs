using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton

    [Header("Game State")]
    public int currentLevel = 1;
    public int score = 0;
    public float timeLeft;
    public Dictionary<int, int> ammo = new Dictionary<int, int>();

    [Header("Level Definitions")]
    public List<LevelDefinition> levelDefinitions; // ScriptableObjects defining levels

    [Header("Level State")]
    private int totalCratesForLevel;
    private int cratesDestroyedThisLevel;
    private bool levelOver = false;

    [Header("UI Elements")]
    public TMP_Text levelText;
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text ammo1Text;
    public TMP_Text ammo2Text;
    public TMP_Text ammo3Text;
    public TMP_Text cratesText;
    public GameObject pauseMenuUI;

    [Header("Game Objects")]
    public CrateSpawner spawner;

    [Header("Bonus: Effects & Audio")]
    public AudioSource sfxSource; // For one-shot effects
    public AudioSource musicSource; // For background music
    public AudioClip successClip;
    public AudioClip failClip;
    public AudioClip levelUpClip;
    public AudioClip levelDownClip;
    public GameObject successEffectPrefab;
    public GameObject failEffectPrefab;

    // --- Unity Lifecycle ---

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Load persistent data
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        score = PlayerPrefs.GetInt("CurrentScore", 0);

        // Load or initialize ammo
        // if (currentLevel == 1)
        // {
            ammo[1] = 50;
            ammo[2] = 50;
            ammo[3] = 50;
        // }
        // else
        // {
        //     ammo[1] = PlayerPrefs.GetInt("Ammo1", 50);
        //     ammo[2] = PlayerPrefs.GetInt("Ammo2", 50);
        //     ammo[3] = PlayerPrefs.GetInt("Ammo3", 50);
        // }

        // Reset pause state
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);

        // Load level data
        LoadLevelData(currentLevel);
        UpdateUI();
    }

    // --- Level Loading ---

    void LoadLevelData(int level)
    {
        cratesDestroyedThisLevel = 0;
        levelOver = false;

        int levelIndex = level - 1;
        if (levelDefinitions == null || levelDefinitions.Count == 0)
        {
            Debug.LogError("No LevelDefinitions assigned to GameManager!");
            EndLevel(false);
            return;
        }

        if (levelIndex < 0 || levelIndex >= levelDefinitions.Count)
        {
            Debug.LogWarning($"Level {level} out of range. Using last available definition.");
            levelIndex = levelDefinitions.Count - 1;
        }

        LevelDefinition currentDef = levelDefinitions[levelIndex];

        timeLeft = currentDef.timeLimit;
        totalCratesForLevel = currentDef.totalCratesToSpawn;

        spawner.InitializeLevel(currentDef);
        spawner.BeginSpawning();
    }

    // --- Update Loop ---

    void Update()
    {
        if (levelOver) return;

        // Pause input
        if (Keyboard.current.escapeKey.wasPressedThisFrame|| Keyboard.current.pKey.wasPressedThisFrame)
            TogglePause();

        if (Time.timeScale == 0) return; // Paused

        // Timer
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            timeLeft = 0;
            EndLevel(false);
        }

        UpdateUI();
    }

    // --- UI Updates ---

    void UpdateUI()
    {
        levelText.text = $"Level: {currentLevel}";
        scoreText.text = $"Score: {score}";
        timeText.text = $"Time: {Mathf.FloorToInt(timeLeft)}";
        ammo1Text.text = $"S (1): {ammo[1]}";
        ammo2Text.text = $"D (2): {ammo[2]}";
        ammo3Text.text = $"F (3): {ammo[3]}";
        cratesText.text = $"Crates: {totalCratesForLevel - cratesDestroyedThisLevel}/{totalCratesForLevel}";
    }

    // --- Gameplay Logic ---

    public bool HasAmmo(int type) => ammo.ContainsKey(type) && ammo[type] > 0;

    public void UseBullet(int type)
    {
        if (HasAmmo(type))
        {
            ammo[type]--;
            UpdateUI();
        }
    }

    public void HandleCrateDestruction(bool success, Vector3 position)
    {
        if (levelOver) return;

        cratesDestroyedThisLevel++;

        if (success)
        {
            score += 10;
            sfxSource.PlayOneShot(successClip);
            Instantiate(successEffectPrefab, position, Quaternion.identity);
        }
        else
        {
            score -= 5;
            sfxSource.PlayOneShot(failClip);
            Instantiate(failEffectPrefab, position, Quaternion.identity);
        }

        if (cratesDestroyedThisLevel >= totalCratesForLevel)
            EndLevel(true);
    }

    // --- Level End / Save / Progression ---

    // Called by Spawner
    public void RegisterCrateSpawn()
    {
        // This function is mostly to track "active crates" if needed,
        // but our main logic is just "total destroyed"
        // We don't do anything here for now.
    }

    void EndLevel(bool success)
    {
        if (levelOver) return;
        levelOver = true;
        Time.timeScale = 0f;

        // Save state
        PlayerPrefs.SetInt("CurrentScore", score);
        PlayerPrefs.SetInt("Ammo1", ammo[1]);
        PlayerPrefs.SetInt("Ammo2", ammo[2]);
        PlayerPrefs.SetInt("Ammo3", ammo[3]);

        if (success)
        {
            if (currentLevel < levelDefinitions.Count)
            {
                PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);
                sfxSource.PlayOneShot(levelUpClip);
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                Debug.Log("All levels complete!");
                GoToEndScene();
            }
        }
        else
        {
            PlayerPrefs.SetInt("CurrentLevel", Mathf.Max(1, currentLevel - 1));
            sfxSource.PlayOneShot(levelDownClip);
            GoToEndScene();
        }
    }

    private void GoToEndScene()
    {
        PlayerPrefs.SetInt("FinalScore", score);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
            PlayerPrefs.SetInt("HighScore", score);

        SceneManager.LoadScene("EndScene");
    }

    // --- Pause & Audio Controls ---

    public void TogglePause()
    {
        bool paused = Time.timeScale == 0;
        Time.timeScale = paused ? 1f : 0f;
        pauseMenuUI.SetActive(!paused);
    }

    public void ToggleAudio()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1f : 0f;
    }

    public void QuitToMenu()
    {
        EndLevel(false);
    }
}
