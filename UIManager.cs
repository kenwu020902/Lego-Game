using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Main Game UI")]
    public GameObject gameUIPanel;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI starText;
    public TextMeshProUGUI levelText;
    
    [Header("Pause Menu")]
    public GameObject pausePanel;
    public bool isPaused = false;
    
    [Header("Level Complete")]
    public GameObject levelCompletePanel;
    public TextMeshProUGUI completeTimeText;
    public TextMeshProUGUI completeStarsText;
    public Button nextLevelButton;
    public Button restartButton;
    
    [Header("Tutorial")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;
    public Button nextTutorialButton;
    
    private string[] tutorialMessages = new string[]
    {
        "Welcome! Click on shapes that match the bins below!",
        "Each bin needs 3 shapes to change to a new shape.",
        "Long press a shape to peek at what's underneath!",
        "No matches? Use the EXTRA LOOPS at the top!",
        "Shapes in loops auto-collect when bins match them!",
        "Complete all layers as fast as you can for 5 stars!"
    };
    
    private int tutorialIndex = 0;
    
    void Start()
    {
        // Initialize UI
        pausePanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        
        // Show tutorial on first level
        if (PlayerPrefs.GetInt("FirstTime", 1) == 1)
        {
            ShowTutorial();
            PlayerPrefs.SetInt("FirstTime", 0);
        }
    }
    
    void Update()
    {
        // Pause on Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    public void UpdateStars(int stars)
    {
        string starsString = "";
        for (int i = 0; i < stars; i++)
            starsString += "⭐";
        for (int i = stars; i < 5; i++)
            starsString += "☆";
            
        starText.text = starsString;
    }
    
    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        tutorialIndex = 0;
        UpdateTutorialText();
    }
    
    public void NextTutorial()
    {
        tutorialIndex++;
        if (tutorialIndex < tutorialMessages.Length)
        {
            UpdateTutorialText();
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }
    
    void UpdateTutorialText()
    {
        tutorialText.text = tutorialMessages[tutorialIndex];
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1; // Freeze game when paused
    }
    
    public void ShowLevelComplete(float time, int stars)
    {
        levelCompletePanel.SetActive(true);
        
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        completeTimeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        
        string starsString = "";
        for (int i = 0; i < stars; i++)
            starsString += "⭐";
        completeStarsText.text = starsString;
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
