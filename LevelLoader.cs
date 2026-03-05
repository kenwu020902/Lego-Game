using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public int levelNumber;
        public string levelName;
        public GameObject levelPrefab; // The 3D object for this level
        public ShapeType startingBin1Shape;
        public ShapeType startingBin2Shape;
        public float parTime = 180f; // 3 minutes
        public int totalShapes;
    }
    
    [Header("Levels")]
    public LevelData[] levels;
    public int currentLevel = 1;
    
    [Header("UI References")]
    public GameObject levelSelectPanel;
    public Transform levelButtonContainer;
    public GameObject levelButtonPrefab;
    
    void Start()
    {
        // Load the first level
        LoadLevel(currentLevel);
    }
    
    public void LoadLevel(int levelNumber)
    {
        // Find level data
        LevelData level = System.Array.Find(levels, l => l.levelNumber == levelNumber);
        
        if (level != null)
        {
            // Clear current level object if exists
            GameObject currentObject = GameObject.FindGameObjectWithTag("LevelObject");
            if (currentObject != null)
                Destroy(currentObject);
            
            // Instantiate new level
            if (level.levelPrefab != null)
            {
                GameObject newLevel = Instantiate(level.levelPrefab, Vector3.zero, Quaternion.identity);
                newLevel.tag = "LevelObject";
                
                // Add rotator component if not present
                if (newLevel.GetComponent<ObjectRotator>() == null)
                    newLevel.AddComponent<ObjectRotator>();
                
                currentLevel = levelNumber;
                Debug.Log("Loaded Level " + levelNumber + ": " + level.levelName);
            }
        }
    }
    
    public void NextLevel()
    {
        if (currentLevel < levels.Length)
        {
            LoadLevel(currentLevel + 1);
        }
        else
        {
            Debug.Log("Congratulations! All levels completed!");
            // Show game complete screen
        }
    }
    
    public void RestartLevel()
    {
        LoadLevel(currentLevel);
    }
    
    // For UI buttons
    public void ShowLevelSelect()
    {
        levelSelectPanel.SetActive(true);
        PopulateLevelButtons();
    }
    
    void PopulateLevelButtons()
    {
        // Clear existing buttons
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create buttons for each level
        foreach (LevelData level in levels)
        {
            GameObject button = Instantiate(levelButtonPrefab, levelButtonContainer);
            // Set button text and functionality
            // button.GetComponentInChildren<Text>().text = "Level " + level.levelNumber;
            // button.GetComponent<Button>().onClick.AddListener(() => LoadLevel(level.levelNumber));
        }
    }
}
