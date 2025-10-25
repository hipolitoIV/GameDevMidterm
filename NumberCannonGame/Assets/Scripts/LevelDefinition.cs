using UnityEngine;

// This attribute allows you to create instances of this class
// from the Unity "Assets/Create" menu.
[CreateAssetMenu(fileName = "LevelDefinition", menuName = "Game/Level Definition", order = 1)]
public class LevelDefinition : ScriptableObject
{
    [Header("Level Setup")]
    public float timeLimit = 180f; // in seconds
    public int totalCratesToSpawn = 20;
    public float spawnInterval = 4.0f;

    [Header("Crate Properties")]
    public int minCrateValue = 3;
    public int maxCrateValue = 10;
    public float minFallSpeed = 1f;
    public float maxFallSpeed = 2f;
}