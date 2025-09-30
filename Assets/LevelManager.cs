using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; } // Singleton pattern for easy access
    public GameObject panel1;
    public GameObject panel2;

    [Header("Level Settings")]
    public int currentLevel = 1; // Starting level, can be set in Inspector or via code
    public int unlockedLevel = 1; // Tracks the highest unlocked level, up to 5

    [Header("UI")]
    public TextMeshProUGUI levelText; // 레벨 텍스트 UI 컴포넌트 - 인스펙터에서 연결

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Load unlockedLevel from PlayerPrefs
        unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); // Default to 1 if not found
        Debug.Log($"[LevelManager] Loaded UnlockedLevel: {unlockedLevel} from PlayerPrefs");
    }

    void Start()
    {
        UpdateLevelUI(); // 초기 레벨 UI 업데이트
    }

    // Returns the level balance multiplier based on current level
    public float GetLevelBalance()
    {
        // Example: Balance increases by 0.05 per level beyond 1 (e.g., Level 1: 1.0, Level 2: 1.05, Level 3: 1.1, etc.)
        return 1.0f + (currentLevel - 1) * 0.05f;
    }

    // Called when the current level is completed to unlock only the next level
    public void UnlockNextLevel()
    {
        Debug.Log($"[LevelManager] UnlockNextLevel called. CurrentLevel: {currentLevel}, UnlockedLevel: {unlockedLevel}");

        if (currentLevel == unlockedLevel && unlockedLevel < 5)
        {
            unlockedLevel++;
            PlayerPrefs.SetInt("UnlockedLevel", unlockedLevel); // Save to PlayerPrefs
            PlayerPrefs.Save(); // Ensure the data is written to disk
            Debug.Log($"[LevelManager] Successfully unlocked level {unlockedLevel} after completing level {currentLevel}. Saved to PlayerPrefs.");
        }
        else if (currentLevel < unlockedLevel)
        {
            Debug.Log($"[LevelManager] Level {currentLevel} is already cleared. No new level unlocked.");
        }
        else if (unlockedLevel >= 5)
        {
            Debug.Log($"[LevelManager] Max level (5) already unlocked. No further unlocks possible.");
        }
        else
        {
            Debug.LogWarning($"[LevelManager] Unlock failed. CurrentLevel {currentLevel} is higher than UnlockedLevel {unlockedLevel}.");
        }
    }

    // Navigate to the next level (for Next button)
    public void NextLevel()
    {
        currentLevel++;
        if (currentLevel > unlockedLevel)
        {
            currentLevel = 1; // Cycle back to level 1 if beyond unlocked level
        }
        UpdateLevelUI();
        Debug.Log($"[LevelManager] Selected level {currentLevel}. Balance multiplier: {GetLevelBalance()}");
    }

    // Navigate to the previous level (for Previous button)
    public void PreviousLevel()
    {
        currentLevel--;
        if (currentLevel < 1)
        {
            currentLevel = unlockedLevel; // Go to highest unlocked level if below 1
        }
        UpdateLevelUI();
        Debug.Log($"[LevelManager] Selected level {currentLevel}. Balance multiplier: {GetLevelBalance()}");
    }

    // 레벨 UI를 업데이트하는 메소드
    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = $"{currentLevel}";
            Debug.Log($"[LevelManager] Updated level text to: {levelText.text}");
        }
        else
        {
            Debug.LogWarning("[LevelManager] levelText is not assigned in Inspector!");
        }
    }

    public void Animation1()
    {
        if (panel1 == null)
        {
            Debug.LogError("[LevelManager] panel1 is not assigned in Inspector!");
            return;
        }

        Animator animator1 = panel1.GetComponent<Animator>();
        if (animator1 == null)
        {
            Debug.LogError("[LevelManager] Animator component not found on panel1!");
            return;
        }

        bool currentState = animator1.GetBool("isEn");
        animator1.SetBool("isEn", !currentState);
        Debug.Log($"[LevelManager] Animation1 toggled. Current state: {!currentState}");
    }

    public void Animation2()
    {
        if (panel2 == null)
        {
            Debug.LogError("[LevelManager] panel2 is not assigned in Inspector!");
            return;
        }

        Animator animator2 = panel2.GetComponent<Animator>();
        if (animator2 == null)
        {
            Debug.LogError("[LevelManager] Animator component not found on panel2!");
            return;
        }

        bool currentState = animator2.GetBool("isEn");
        animator2.SetBool("isEn", !currentState);
        Debug.Log($"[LevelManager] Animation2 toggled. Current state: {!currentState}");
    }
}