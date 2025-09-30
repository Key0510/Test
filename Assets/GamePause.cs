using UnityEngine;
using UnityEngine.UI;

public class GamePause : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel; // 설정 패널 GameObject
    [SerializeField] private GameObject settingPanel; // 설정 패널 GameObject
    [SerializeField] private AudioSource bgmAudioSource; // BGM용 AudioSource
    [SerializeField] private Slider bgmSlider; // BGM 음량 슬라이더
    [SerializeField] private Slider masterSlider; // 전체 소리 슬라이더
    [SerializeField] private Button speed1xButton; // 1배속 버튼
    [SerializeField] private Button speed2xButton; // 2배속 버튼
    [SerializeField] private Button speed3xButton; // 3배속 버튼
    public bool isPaused = false; // 일시 정지 상태
    private float currentSpeed = 1f; // 현재 게임 속도

    void Start()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("SettingsPanel is not assigned in GameManager!");
        }
        if (bgmAudioSource == null)
        {
            Debug.LogWarning("BgmAudioSource is not assigned in GameManager!");
        }
        if (bgmSlider == null || masterSlider == null)
        {
            Debug.LogWarning("One or more sliders are not assigned in GameManager!");
        }
        if (speed1xButton == null || speed2xButton == null || speed3xButton == null)
        {
            Debug.LogWarning("One or more speed buttons are not assigned in GameManager!");
        }

        // 초기 값 설정
        if (bgmSlider != null)
        {
            bgmSlider.value = bgmAudioSource != null ? bgmAudioSource.volume : 1f; // 기본 BGM 음량
            bgmSlider.onValueChanged.AddListener(delegate { SetBGMVolume(bgmSlider.value); }); // 이벤트 연결
        }
        if (masterSlider != null)
        {
            masterSlider.value = AudioListener.volume; // 기본 전체 음량
            masterSlider.onValueChanged.AddListener(delegate { SetMasterVolume(masterSlider.value); }); // 이벤트 연결
        }
        if (speed1xButton != null) speed1xButton.onClick.AddListener(SetSpeed1x);
        if (speed2xButton != null) speed2xButton.onClick.AddListener(SetSpeed2x);
        if (speed3xButton != null) speed3xButton.onClick.AddListener(SetSpeed3x);
        SetGameSpeed(currentSpeed); // 기본 1배속
        ResumeGame(); // 초기 상태는 재개
    }

    void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // 일시 정지 중 UI 업데이트
        if (isPaused)
        {
            UpdateGameUI();
        }
    }

    // 게임 일시 정지
    public void PauseGame()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true); // 설정 패널 활성화
        }
        Time.timeScale = 0; // 게임 시간 정지
        isPaused = true;
        Debug.Log("Game Paused");
    }

    // 게임 재개
    public void ResumeGame()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false); // 설정 패널 비활성화
            settingPanel.SetActive(false);
        }
        Time.timeScale = currentSpeed; // 현재 속도로 복원
        isPaused = false;
        Debug.Log("Game Resumed");
    }

    // UI 업데이트 (시간 스케일에 상관없이 실행)
    private void UpdateGameUI()
    {
        PlayerAbility playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility != null)
        {
            playerAbility.UpdateHealthUI();
            playerAbility.UpdateShieldUI();
            playerAbility.UpdateAttackPowerUI();
            playerAbility.UpdateCritChanceUI();
        }
    }

    // BGM 음량 조절
    public void SetBGMVolume(float volume)
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = volume;
        }
        else
        {
            Debug.LogWarning("BgmAudioSource is null, cannot set volume");
        }
    }

    // 전체 소리 음량 조절 (AudioListener 사용)
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    // 게임 속도 설정
    public void SetGameSpeed(float speed)
    {
        currentSpeed = speed;
        Time.timeScale = speed;
        if (bgmAudioSource != null)
        {
            bgmAudioSource.pitch = 1f; // BGM 속도 고정
        }
        Debug.Log($"Game Speed set to {speed}x, BGM pitch fixed at 1");
    }

    // 1배속 버튼 클릭
    public void SetSpeed1x()
    {
        SetGameSpeed(1f);
    }

    // 2배속 버튼 클릭
    public void SetSpeed2x()
    {
        SetGameSpeed(2f);
    }

    // 3배속 버튼 클릭
    public void SetSpeed3x()
    {
        SetGameSpeed(3f);
    }
}