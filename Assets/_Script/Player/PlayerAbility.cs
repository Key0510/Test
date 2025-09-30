using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerAbility : MonoBehaviour
{
    [Header("체력 관련")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("공격 관련")]
    public int attackPower = 10; // 플레이어 공격력
    public float critChance = 0.1f; // 치명타 확률 (0.0 ~ 1.0)
    private BGMController bgmController;

    [Header("보호막")]
    public int shield = 0;

    [SerializeField] public bool enableDamageReduction = false; // 피해 감소 능력 활성화
    [SerializeField, Range(0f, 100f)] public float damageReductionPercentage = 0f; // 피해 감소 비율 (%)

    [Header("UI")]
    public TextMeshProUGUI playerHpText;
    public Image playerHpBar;
    public TextMeshProUGUI playerShieldText;
    public TextMeshProUGUI playerAttackPowerText;
    public TextMeshProUGUI playerCriticalRatioText;
    public GameObject GameOverPanel;

    [Header("사운드")]
    [SerializeField] private AudioClip damageSound; // 피해 시 재생할 사운드
    [SerializeField] private AudioClip deathSound; // 사망 시 재생할 사운드
    private AudioSource audioSource; // 오디오 소스 컴포넌트

    private Animator playerAnimator;
    public GameObject HealFX;
    public GameObject ShieldFX;
    public GameObject BuffFX;
    public GameObject CriticalFX;


    void Start()
    {
        bgmController = FindObjectOfType<BGMController>();
        playerAnimator = GetComponent<Animator>();
        // AudioSource 컴포넌트 가져오기 (없으면 추가)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; // 시작 시 자동 재생 방지
        audioSource.loop = false; // 사운드 루프 비활성화

        // 게임 시작 시 현재 체력을 최대 체력으로 초기화
        currentHealth = maxHealth;
        UpdateShieldUI();
        UpdateHealthUI();
        UpdateAttackPowerUI();
        UpdateCritChanceUI();

        // 사운드 클립 확인
        if (damageSound == null)
        {
            Debug.LogWarning($"플레이어 {gameObject.name}에 피해 사운드가 설정되지 않았습니다.");
        }
        if (deathSound == null)
        {
            Debug.LogWarning($"플레이어 {gameObject.name}에 사망 사운드가 설정되지 않았습니다.");
        }
    }

    /// <summary>
    /// 피해를 입을 때 호출되는 메서드
    /// </summary>
    public void TakeDamage(int damage)
    {
        // 피해 감소 적용
        int finalDamage = damage;
        if (enableDamageReduction)
        {
            finalDamage = Mathf.FloorToInt(damage * (1f - damageReductionPercentage / 100f));
            Debug.Log($"Damage reduced by {damageReductionPercentage}%: {damage} -> {finalDamage}");
        }

        // 피해 사운드 재생
        if (damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
        else
        {
            Debug.LogWarning("피해 사운드가 설정되지 않았습니다.");
        }

        playerAnimator.SetTrigger("isDamaged");
        if (shield > 0)
        {
            int remainingDamage = finalDamage - shield;
            shield = Mathf.Max(shield - finalDamage, 0);
            if (remainingDamage > 0)
            {
                currentHealth = Mathf.Max(currentHealth - remainingDamage, 0);
            }
        }
        else
        {
            currentHealth = Mathf.Max(currentHealth - finalDamage, 0);
        }

        UpdateShieldUI();
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            PlayerDie();
        }

        Debug.Log($"체력: {currentHealth}, 보호막: {shield}");
    }

    /// <summary>
    /// 체력을 회복할 때 호출되는 메서드
    /// </summary>
    public void Heal(int amount)
    {
        HealFX.SetActive(false);
        HealFX.SetActive(true);
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
        Debug.Log($"[회복] 체력: {currentHealth}");
    }

    /// <summary>
    /// 보호막을 추가할 때 호출되는 메서드
    /// </summary>
    public void AddShield(int amount)
    {
        ShieldFX.SetActive(false);
        ShieldFX.SetActive(true);
        shield += amount;
        UpdateShieldUI();
        Debug.Log($"[보호막 추가] 보호막: {shield}");
    }

    /// <summary>
    /// 공격력 증가 호출되는 메서드
    /// </summary>
    public void AddAttackPower(int amount)
    {
        BuffFX.SetActive(false);
        BuffFX.SetActive(true);
        attackPower = Mathf.Max(0, attackPower + amount);
        UpdateAttackPowerUI();
        Debug.Log($"[공격력 추가] 공격력: {attackPower}");
    }

    public void AddCritChance(float amount)
    {
        CriticalFX.SetActive(false);
        CriticalFX.SetActive(true);
        critChance = Mathf.Clamp(critChance + amount, 0f, 1f);
        UpdateCritChanceUI();
        Debug.Log($"[치명타확률 추가] 치명타확률 : {critChance}");
    }



    public void UpdateHealthUI()
    {
        if (playerHpText != null)
        {
            playerHpText.text = $"{currentHealth} / {maxHealth}";
        }

        if (playerHpBar != null)
        {
            playerHpBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void UpdateShieldUI()
    {
        if (playerShieldText != null)
        {
            playerShieldText.text = $"{shield}";
        }
    }

    public void UpdateAttackPowerUI()
    {
        if (playerAttackPowerText != null)
        {
            playerAttackPowerText.text = $"{attackPower}";
        }
    }

    public void UpdateCritChanceUI()
    {
        if (playerCriticalRatioText != null)
        {
            // 정수 부분만 표시
            int critPercentage = Mathf.FloorToInt(critChance * 100);
            playerCriticalRatioText.text = $"{critPercentage}%";
        }
    }

    public void PlayerDie()
    {
        // 사망 사운드 재생
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        else
        {
            Debug.LogWarning("사망 사운드가 설정되지 않았습니다.");
        }

        Debug.Log("플레이어 사망!");

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isDead", true); // Animator에 isDead 트리거 필요
        }
        GameOverPanel.SetActive(true);
        bgmController.PlayBGM(5);
    }
    public void RestartGame()
    {
        // 현재 씬 이름 가져오기
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

    }
}