using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton instance so other scripts can access GameManager easily
    public static GameManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI starText;
    public GameObject levelCompletePanel;
    
    [Header("Bin References")]
    public BinManager binManager;
    public LoopManager loopManager;
    
    [Header("Game Settings")]
    public float levelTime = 0f;
    private bool isGameActive = true;
    private int starsEarned = 0;
    private int totalShapes = 0;
    private int shapesCollected = 0;

    void Awake()
    {
        // Set up singleton
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        // Count all shapes in the scene at start
        totalShapes = FindObjectsOfType<Shape>().Length;
        Debug.Log("Total shapes in level: " + totalShapes);
        
        // Start timer
        InvokeRepeating("UpdateTimer", 1f, 1f);
    }

    void UpdateTimer()
    {
        if (!isGameActive) return;
        
        levelTime += 1f;
        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        // Format time as MM:SS
        int minutes = Mathf.FloorToInt(levelTime / 60);
        int seconds = Mathf.FloorToInt(levelTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        // Update stars based on time
        UpdateStarsBasedOnTime();
    }

    void UpdateStarsBasedOnTime()
    {
        // Star rating based on time
        if (levelTime < 180) // Under 3 minutes
            starsEarned = 5;
        else if (levelTime < 240) // 3-4 minutes
            starsEarned = 4;
        else if (levelTime < 300) // 4-5 minutes
            starsEarned = 3;
        else if (levelTime < 360) // 5-6 minutes
            starsEarned = 2;
        else // Over 6 minutes
            starsEarned = 1;
            
        // Update star display
        string stars = "";
        for (int i = 0; i < starsEarned; i++)
            stars += "⭐";
        for (int i = starsEarned; i < 5; i++)
            stars += "☆";
            
        starText.text = stars;
    }

    public void ShapeCollected(Shape shape)
    {
        shapesCollected++;
        Debug.Log("Collected: " + shapesCollected + "/" + totalShapes);
        
        // Check if level is complete
        if (shapesCollected >= totalShapes)
        {
            LevelComplete();
        }
    }

    void LevelComplete()
    {
        isGameActive = false;
        levelCompletePanel.SetActive(true);
        Debug.Log("Level Complete! Stars: " + starsEarned);
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }
}
