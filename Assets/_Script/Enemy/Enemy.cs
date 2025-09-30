// Now, the modified Enemy.cs script
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100;
    public float currentHealth;
    public float currentShield = 0;
    public float attackPower = 10;
    public float moneyReward = 10;
    public float _poison = 0;
    public float weakness = 0;

    [Header("Enemy Pattern")]
    public float healUp = 0;
    public float shieldUp = 0;
    public float boostUp = 0;

    [Header("Pattern Ratio(ex. 50, 60, 90 == 50%, 10% (60-50), 30% (90-60), 10% (100-90)")]
    public int firstRatio = 50;
    public int secondRatio = 60;
    public int thirdRatio = 90;

    [Header("UI")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI shieldText;
    public TextMeshProUGUI weaknessText;
    [SerializeField] private TextMeshProUGUI poisonText;
    [SerializeField] private TextMeshProUGUI shieldDamageText;
    [SerializeField] private TextMeshProUGUI actualDamageText;
    public TextMeshProUGUI attackPowerText;
    public Image hpBar;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip damageSound;

    public StageManager stageManager;
    public GateSpawner gateSpawner;
    public PlayerAbility playerAbility;
    private Floor floor;
    private Animator enemyAnimator;
    private MoneyManager moneyManager;
    private BGMController bGMController;
    public GameObject PoisonFX;
    public GameObject WeaknessFX;
    public GameObject ShieldFX;
    public GameObject HealFX;
    public GameObject BuffFX;

    [Header("Boss??")]
    public bool isBoss = false;

    private LevelManager levelManager; // Reference to LevelManager

    public int poison
    {
        get => (int)_poison;
        set
        {
            _poison = Mathf.Max(0, value);
            UpdatePoisonUI();
        }
    }

    [System.Obsolete]
    void Start()
    {
        gateSpawner = FindObjectOfType<GateSpawner>();
        playerAbility = FindObjectOfType<PlayerAbility>();
        floor = FindObjectOfType<Floor>();
        enemyAnimator = GetComponent<Animator>();
        moneyManager = FindObjectOfType<MoneyManager>();
        stageManager = FindObjectOfType<StageManager>();
        bGMController = FindObjectOfType<BGMController>();
        levelManager = LevelManager.Instance; // Access the singleton instance
        if (levelManager == null)
        {
            Debug.LogError("[Enemy] LevelManager not found! Make sure to add it to the scene.");
        }

        if (isBoss) bGMController.PlayBGM(6);

        if (enemyAnimator == null)
        {
            Debug.LogError($"[Enemy] Animator not found on {gameObject.name}");
        }

        float levelBalance = levelManager != null ? levelManager.GetLevelBalance() : 1.0f; // Default to 1.0 if not found

        if (!isBoss)
        {
            // 모든 적은 "Enemy" 태그 사용, 스탯은 인스펙터에서 설정
            maxHealth = (int)(33 * stageManager.currentStage * 0.7f * levelBalance);
            attackPower = (int)(3 * stageManager.currentStage * 0.5f * levelBalance);
            moneyReward = (int)(10 * stageManager.currentStage * 0.7f * levelBalance);
            healUp = (int)(10 * stageManager.currentStage * 0.7f * levelBalance);
            shieldUp = (int)(3 * stageManager.currentStage * 0.7f * levelBalance);
            boostUp = (int)(3 * stageManager.currentStage * 0.7f * levelBalance);
            firstRatio = 70;
            secondRatio = 80;
            thirdRatio = 90;

            currentHealth = maxHealth;
            UpdateHealthUI();
            UpdateShieldUI();
            UpdateWeaknessUI();
            UpdatePoisonUI();
            UpdateAttackPowerUI();
            PoisonFX.SetActive(false);
            ShieldFX.SetActive(false);
            WeaknessFX.SetActive(false);
            if (shieldDamageText != null)
            {
                shieldDamageText.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("shieldDamageText not assigned!");
            }
            if (actualDamageText != null)
            {
                actualDamageText.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("actualDamageText not assigned!");
            }
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            if (audioSource == null)
            {
                Debug.LogError("AudioSource not found on Enemy!");
            }
            if (damageSound == null)
            {
                Debug.LogWarning("damageSound not assigned!");
            }
        }
        else if (isBoss)
        {
            // 모든 적은 "Enemy" 태그 사용, 스탯은 인스펙터에서 설정
            maxHealth = 100 * stageManager.currentStage * levelBalance;
            attackPower = 3 * stageManager.currentStage * levelBalance;
            moneyReward = 10 * stageManager.currentStage * levelBalance;
            healUp = 10 * stageManager.currentStage * levelBalance;
            shieldUp = 3 * stageManager.currentStage * levelBalance;
            boostUp = 3 * stageManager.currentStage * levelBalance;
            firstRatio = 70;
            secondRatio = 80;
            thirdRatio = 90;

            currentHealth = maxHealth;
            UpdateHealthUI();
            UpdateShieldUI();
            UpdateWeaknessUI();
            UpdatePoisonUI();
            UpdateAttackPowerUI();
            PoisonFX.SetActive(false);
            ShieldFX.SetActive(false);
            WeaknessFX.SetActive(false);
            if (shieldDamageText != null)
            {
                shieldDamageText.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("shieldDamageText not assigned!");
            }
            if (actualDamageText != null)
            {
                actualDamageText.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("actualDamageText not assigned!");
            }
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            if (audioSource == null)
            {
                Debug.LogError("AudioSource not found on Enemy!");
            }
            if (damageSound == null)
            {
                Debug.LogWarning("damageSound not assigned!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"[Enemy] TakeDamage called for {gameObject.name} with damage: {damage}");
        enemyAnimator.SetTrigger("isDamaged");
        int shieldDamage = 0;
        int actualDamage = 0;

        if (currentShield > 0)
        {
            shieldDamage = (int)Mathf.Min(damage, currentShield);
            actualDamage = (int)Mathf.Max(0, damage - currentShield);
            currentShield = Mathf.Max(currentShield - damage, 0);
            currentHealth = Mathf.Max(currentHealth - actualDamage, 0);
        }
        else
        {
            actualDamage = damage;
            currentHealth = Mathf.Max(currentHealth - damage, 0);
        }

        if (audioSource != null && damageSound != null && (shieldDamage > 0 || actualDamage > 0))
        {
            audioSource.PlayOneShot(damageSound);
            Debug.Log($"Playing damage sound for {gameObject.name}, shieldDamage={shieldDamage}, actualDamage={actualDamage}");
        }

        if (shieldDamageText != null && shieldDamage > 0)
        {
            shieldDamageText.text = "-" + shieldDamage.ToString();
            shieldDamageText.gameObject.SetActive(true);
            Debug.Log($"Shield damage text displayed: -{shieldDamage}");
            StartCoroutine(HideDamageText(shieldDamageText));
        }

        if (actualDamageText != null && actualDamage > 0)
        {
            actualDamageText.text = "-" + actualDamage.ToString();
            actualDamageText.gameObject.SetActive(true);
            Debug.Log($"Actual damage text displayed: -{actualDamage}");
            StartCoroutine(HideDamageText(actualDamageText));
        }

        UpdateShieldUI();
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HideDamageText(TextMeshProUGUI damageText)
    {
        yield return new WaitForSeconds(1f);
        if (damageText != null)
        {
            damageText.gameObject.SetActive(false);
            Debug.Log($"Damage text hidden: {damageText.text}");
        }
    }

    public void UpdateHealthUI()
    {
        if (hpText != null)
        {
            hpText.text = $"{currentHealth} / {maxHealth}";
        }

        if (hpBar != null)
        {
            hpBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void UpdateShieldUI()
    {
        if (shieldText != null)
        {
            shieldText.text = $"{currentShield}";
        }
    }

    public void UpdateWeaknessUI()
    {
        WeaknessFX.SetActive(false);
        WeaknessFX.SetActive(true);
        if (weaknessText != null)
        {
            weaknessText.text = $"{weakness}";
        }
    }

    public void UpdatePoisonUI()
    {
        PoisonFX.SetActive(false);
        PoisonFX.SetActive(true);
        if (poisonText != null)
        {
            poisonText.text = $"{_poison}";
        }
    }

    public void UpdateAttackPowerUI()
    {
        if (attackPowerText != null)
        {
            attackPowerText.text = $"{attackPower}";
        }
    }

    public void Die()
    {
        Debug.Log($"[Enemy] Die called for {gameObject.name}");
        moneyManager.AddMoney((int)moneyReward);
        enemyAnimator.SetBool("isDead", true);
        StartCoroutine(DestroyAfterAnimation());
        CheckAllEnemiesDead();
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void CheckAllEnemiesDead()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");
        if (objs.Length == 1 && gateSpawner != null && !gateSpawner.isRoundEnd)
        {
            Debug.Log($"[Enemy] 모든 적 사망, 라운드 종료 및 게이트 스폰: {gameObject.name}");
            gateSpawner.isRoundEnd = true;
            gateSpawner.SpawnTwoRandomGates();
            if (floor != null)
            {
                floor.CanStart = false;
            }
        }
    }

    public IEnumerator Attack()
    {
        Debug.Log($"[Enemy] Attack called for {gameObject.name}");
        int roll = UnityEngine.Random.Range(0, 100);
        Debug.Log($"[Enemy] Roll: {roll}, firstRatio: {firstRatio}, secondRatio: {secondRatio}, thirdRatio: {thirdRatio}");

        if (roll < firstRatio)
        {
            enemyAnimator.SetTrigger("isAttacking");
            Debug.Log($"[Enemy] Triggered isAttacking for {gameObject.name}");
        }
        else if (roll < secondRatio)
        {
            enemyAnimator.SetTrigger("isHealing");
            Debug.Log($"[Enemy] Triggered isHealing for {gameObject.name}");
        }
        else if (roll < thirdRatio)
        {
            enemyAnimator.SetTrigger("isShielding");
            Debug.Log($"[Enemy] Triggered isShielding for {gameObject.name}");
        }
        else
        {
            enemyAnimator.SetTrigger("isBoosting");
            Debug.Log($"[Enemy] Triggered isBoosting for {gameObject.name}");
        }

        if (_poison > 0)
        {
            poison = (int)Mathf.Max(_poison - 1, 0);
            TakeDamage((int)_poison);
            Debug.Log($"[Enemy] Applied poison damage: {_poison} to {gameObject.name}");
        }

        yield return new WaitForSeconds(0.75f);

        if (weakness > 0)
        {
            weakness = Mathf.Max(weakness - 1, 0);
            UpdateWeaknessUI();
            Debug.Log($"[Enemy] Weakness reduced to {weakness} for {gameObject.name}");
        }
    }

    public bool IsDead()
    {
        bool isDead = currentHealth <= 0;
        Debug.Log($"[Enemy] IsDead check for {gameObject.name}: {isDead}, currentHealth: {currentHealth}");
        return isDead;
    }

    public void DealDamageToPlayer()
    {
        int DealtoDamage = (int)(attackPower - weakness);
        if (DealtoDamage <= 0)
        {
            playerAbility.TakeDamage(0);
        }
        else
        {
            playerAbility.TakeDamage((int)(attackPower - weakness));
        }
        Debug.Log($"[Enemy] 플레이어에게 데미지: {DealtoDamage} by {gameObject.name}");
    }

    public void Heal()
    {
        HealFX.SetActive(false);
        HealFX.SetActive(true);
        currentHealth = Mathf.Min(currentHealth + healUp, maxHealth);
        UpdateHealthUI();
        Debug.Log($"[Enemy] 힐: {healUp} to {gameObject.name}");
    }

    public void Shield()
    {
        ShieldFX.SetActive(false);
        ShieldFX.SetActive(true);
        currentShield += shieldUp;
        UpdateShieldUI();
        Debug.Log($"[Enemy] 쉴드 추가: {shieldUp} to {gameObject.name}");
    }

    public void Boost()
    {
        BuffFX.SetActive(false);
        BuffFX.SetActive(true);
        attackPower = Mathf.Max(0, attackPower + boostUp);
        UpdateAttackPowerUI();
        Debug.Log($"[Enemy] 공격력 버프: {boostUp} to {gameObject.name}");
    }
}