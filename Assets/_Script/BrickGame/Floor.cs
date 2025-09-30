using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour
{
    private BallManager ballManager;
    private BlockSpawner blockSpawner;
    private BlockManager blockManager;
    private GateSpawner gateSpawner;
    public Animator playerAnimator;
    public bool CanStart = false;
    public int CollisionCount = 0;
    public int Turn = 0;
    public EnemyManager enemyManager;
    public int countJackpot = 0;
    public int damageWhenBlockTouchFloor = 10;
    private PlayerAbility playerAbility;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip blockAbilitySound;
    [SerializeField] public GameObject Block;

    [SerializeField] public bool enableMoneyOnBreak = false;      // 벽돌 깨질 때마다 돈 1원 추가
    [SerializeField] public int BreakMoneyAmount = 0;
    [SerializeField] public bool enableMaxHpOnBreak = false;      // 벽돌 깨질 때마다 최대 체력 1 증가
    [SerializeField] public int BreakMaxHealthAmount = 0;
    [SerializeField] public bool enableAttackOnBreak = false;     // 벽돌 깨질 때마다 공격력 1 증가
    [SerializeField] public int BreakAttackAmount = 0;
    [SerializeField] public bool enableDefenseOnBreak = false;    // 벽돌 깨질 때마다 방어도 1 증가
    [SerializeField] public int BreakShieldAmount = 0;
    [SerializeField] public bool enableHpRecoveryOnBreak = false; // 벽돌 깨질 때마다 체력 3 회복
    [SerializeField] public int BreakHealAmount = 0;
    [SerializeField] public bool enableCritOnBreak = false; // 벽돌 깨질 때마다 체력 3 회복
    [SerializeField] public float BreakCritAmount = 0.00f;
    [SerializeField] public bool enableShieldRetentionOnTurnEnd = false; // 턴 종료 시 방어도 유지
    [SerializeField] public float shieldRetentionPercentage = 0f;      // 유지할 방어도 비율 (%)

    [System.Obsolete]
    void Start()
    {
        ballManager = FindObjectOfType<BallManager>();
        blockSpawner = FindObjectOfType<BlockSpawner>();
        blockManager = FindObjectOfType<BlockManager>();
        enemyManager = FindObjectOfType<EnemyManager>();
        gateSpawner = FindObjectOfType<GateSpawner>();
        playerAbility = FindObjectOfType<PlayerAbility>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>();
        }
        if (gateSpawner == null) Debug.LogError("GateSpawner not found!");
        if (blockManager == null) Debug.LogError("BlockManager not found!");
        if (blockSpawner == null) Debug.LogError("BlockSpawner not found!");
        if (player == null) Debug.LogError("Player not found!");
        if (playerAnimator == null) Debug.LogError("Player Animator not found!");
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) Debug.LogError("AudioSource not found on Floor!");
        if (blockAbilitySound == null) Debug.LogWarning("BlockAbilitySound not assigned!");
        if (Block == null) Debug.LogError("Block GameObject not assigned!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gateSpawner != null && gateSpawner.isRoundEnd)
        {
            Debug.Log("Round ended, ignoring ball collision");
            return;
        }

        if (other.gameObject.CompareTag("Ball"))
        {
            CollisionCount++;
            Debug.Log($"Ball collision detected, count: {CollisionCount}");
        }

        if (ballManager != null && ballManager.playerBallCount == CollisionCount)
        {
            CanStart = false;
            Turn++;
            Debug.Log($"Turn {Turn} started");
            StartCoroutine(EndPlayerTurn());
        }
    }

    private IEnumerator EndPlayerTurn()
    {
        CollisionCount = 0;

        if (CheckBlockAtY(-3.78f))
        {
            if (ballManager != null)
            {
                ballManager.playerBallCount += ballManager.addcount;
                ballManager.addcount = 0;
            }
            if (blockSpawner != null)
            {
                blockSpawner.MoveAllBlocksDown();
                blockSpawner.SpawnBlocks();
                Debug.Log("Blocks moved down and new blocks spawned");
            }
        }
        else
        {
            Turn--;
            playerAbility.TakeDamage(damageWhenBlockTouchFloor);
            Debug.Log("Block at y=-3.78, no block movement or spawning, Turn decremented");
        }

        if (blockManager != null && blockManager.brokenBlocks != null && blockManager.brokenBlocks.Count > 0)
        {
            Debug.Log($"Executing abilities for {blockManager.brokenBlocks.Count} broken blocks");
            yield return ExecuteBrokenBlockAbilities();
        }

        if (enemyManager != null)
        {
            enemyManager.StartEnemyTurn();
            yield return enemyManager.EnemyTurnRoutine();
        }

        if (gateSpawner != null && gateSpawner.isRoundEnd)
        {
            CanStart = false;
            Debug.Log("Round ended, stopping turn progression, CanStart=false");
        }
        else
        {
            CanStart = true;
            if (enableShieldRetentionOnTurnEnd)
            {
                int retainedShield = Mathf.FloorToInt(playerAbility.shield * (shieldRetentionPercentage / 100f));
                playerAbility.shield = retainedShield;
            }
            else
            {
                playerAbility.shield = 0;
            }
            playerAbility.UpdateShieldUI();
        }
    }
    private void ApplySpecialAbilitiesOnBreak()
    {
        if (playerAbility == null)
        {
            return;
        }

        if (enableMoneyOnBreak)
        {
            MoneyManager.Instance.AddMoney(BreakMoneyAmount);  // 돈 1원 추가 (money 변수 가정)

        }

        if (enableMaxHpOnBreak)
        {
            playerAbility.maxHealth += BreakMaxHealthAmount;  // 최대 체력 1 증가 (maxHp 변수 가정)
            playerAbility.UpdateHealthUI();
        }

        if (enableAttackOnBreak)
        {
            playerAbility.attackPower += BreakAttackAmount;  // 공격력 1 증가 (attack 변수 가정)
            playerAbility.UpdateAttackPowerUI();
        }

        if (enableDefenseOnBreak)
        {
            playerAbility.shield += BreakShieldAmount;  // 방어도 1 증가 (defense 변수 가정)
            playerAbility.UpdateShieldUI();
        }

        if (enableHpRecoveryOnBreak)
        {
            playerAbility.Heal(BreakHealAmount);
            playerAbility.UpdateHealthUI();
        }
        if (enableCritOnBreak)
        {
            playerAbility.AddCritChance(BreakCritAmount);
            playerAbility.UpdateCritChanceUI();
        }
    }

    private IEnumerator ExecuteBrokenBlockAbilities()
    {
        if (blockManager == null || blockManager.brokenBlocks == null)
        {
            Debug.LogError("BlockManager or brokenBlocks is null, cannot execute abilities");
            yield break;
        }

        foreach (GameObject brokenBlock in blockManager.brokenBlocks)
        {
            if (!gateSpawner.isRoundEnd)
            {
                if (brokenBlock != null)
                {
                    BlockAbility ability = brokenBlock.GetComponent<BlockAbility>();
                    if (ability != null)
                    {
                        if (playerAnimator != null)
                        {
                            playerAnimator.SetTrigger("isAttacking");
                        }
                        if (brokenBlock.GetComponent<JackpotBlockAbility>() != null)
                        {
                            countJackpot++;
                            Debug.Log($"Jackpot block detected, count: {countJackpot}");
                        }
                        else
                        {
                            countJackpot = 0;
                        }

                        // Block 스프라이트를 brokenBlock 스프라이트로 설정
                        if (Block != null && brokenBlock != null)
                        {
                            SpriteRenderer blockRenderer = Block.GetComponent<SpriteRenderer>();
                            SpriteRenderer brokenBlockRenderer = brokenBlock.GetComponent<SpriteRenderer>();
                            if (blockRenderer != null && brokenBlockRenderer != null && brokenBlockRenderer.sprite != null)
                            {
                                Block.SetActive(false);
                                blockRenderer.sprite = brokenBlockRenderer.sprite;
                                Debug.Log($"Set Block sprite to {brokenBlockRenderer.sprite.name} for {brokenBlock.name}");
                                Block.SetActive(true);
                            }
                            else
                            {
                                Debug.LogWarning($"SpriteRenderer missing or no sprite on Block or {brokenBlock.name}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Block or brokenBlock is null");
                        }

                        ability.Execute();
                        playerAnimator.SetTrigger("isAttacking");
                        if (audioSource != null && blockAbilitySound != null)
                        {
                            audioSource.PlayOneShot(blockAbilitySound);
                            Debug.Log($"Playing sound for block {brokenBlock.name}");
                        }
                        if (blockManager != null)
                        {
                            blockManager.RemoveBrokenBlockUI(brokenBlock);
                        }
                        ApplySpecialAbilitiesOnBreak();
                        yield return new WaitForSeconds(0.5f);
                    }
                    else
                    {
                        Debug.LogWarning($"No BlockAbility found on {brokenBlock.name}");
                    }
                }
                else
                {
                    Debug.LogWarning("Broken block is null");
                }
            }
        }

        if (blockManager != null && blockManager.brokenBlocks != null)
        {
            Debug.Log($"Clearing {blockManager.brokenBlocks.Count} broken blocks");
            blockManager.brokenBlocks.Clear();
            blockManager.ClearBrokenBlocksUI();
            countJackpot = 0;
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("ready", false);
                Debug.Log("Player animator 'ready' set to false after clearing broken blocks");
            }
            Debug.Log("Broken blocks list and UI cleared at turn end");
        }
    }

    public bool CheckBlockAtY(float targetY)
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        float tolerance = 0.1f;
        bool blockAtTarget = false;

        foreach (GameObject block in blocks)
        {
            if (block != null)
            {
                float blockY = block.transform.position.y;
                if (Mathf.Abs(blockY - targetY) < tolerance)
                {
                    Debug.Log($"Block {block.name} found at y={blockY} (target={targetY}), blocking turn progression");
                    blockAtTarget = true;
                }
                else if (blockY <= -4f)
                {
                    Debug.Log($"Block {block.name} at y={blockY} is below -4, deactivating");
                    block.SetActive(false);
                }
            }
        }

        if (!blockAtTarget)
        {
            Debug.Log("No blocks found at y=-3.78");
        }
        return !blockAtTarget;
    }
}