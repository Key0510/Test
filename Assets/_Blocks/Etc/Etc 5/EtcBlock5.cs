using UnityEngine;

public class EtcBlock5 : BlockAbility
{
    [SerializeField] private float executeHealthRatio = 0.1f; // 처형 조건 체력 비율 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to the nearest enemy, and instantly executes if their health is {2} or below.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 치명타 확률에 따라 치명타 여부 결정
                bool isCriticalHit = Random.value < playerAbility.critChance;
                // 데미지 계산: 블록 데미지 + 플레이어 공격력
                int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

                // 사전 계산: 데미지 적용 후 잔여 체력 예측
                int remainingShield = (int)Mathf.Max(0, enemy.currentShield - damageToDeal);
                int overflowDamage = (int)Mathf.Max(0, damageToDeal - enemy.currentShield);
                int remainingHealth = (int)Mathf.Max(0, enemy.currentHealth - overflowDamage);

                // 공격
                enemy.TakeDamage(damageToDeal);

                // 처형 조건: 잔여 체력이 설정된 비율 이하
                bool willExecute = remainingHealth <= (int)(enemy.maxHealth * executeHealthRatio);

                if (willExecute && !enemy.IsDead())
                {
                    enemy.currentHealth = 0; // 처형
                    enemy.UpdateHealthUI(); // 체력 UI 갱신
                    enemy.Die(); // 사망 처리
                    Debug.Log($"처형! {closestEnemy.name}에게 {damageToDeal} 데미지를 주고 체력 {executeHealthRatio:P0} 이하로 즉시 사망. (잔여 체력: {remainingHealth})");
                }
                else
                {
                    // 디버그 로그 (처형 안 됨)
                    if (isCriticalHit)
                    {
                        Debug.Log($"치명타! {closestEnemy.name}에게 {damageToDeal} 데미지를 줌. (잔여 체력: {remainingHealth})");
                    }
                    else
                    {
                        Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 줌. (잔여 체력: {remainingHealth})");
                    }
                }
            }
        }
        else
        {
            Debug.Log("적을 찾을 수 없어 공격하지 못함.");
        }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minX = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                Enemy enemyComp = enemy.GetComponent<Enemy>();
                if (enemyComp != null && !enemyComp.IsDead())
                {
                    float xPos = enemy.transform.position.x;
                    if (xPos < minX)
                    {
                        minX = xPos;
                        closestEnemy = enemy;
                    }
                }
            }
        }
        return closestEnemy;
    }

    public override void Upgrade()
    {
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주고, 체력이 {2:P0} 이하라면 즉시 처형하며, {3} 방어력을 얻음.", AttackDamage, CriticalHitDamage, executeHealthRatio, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage,criticalHitDamage, executeHealthRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to the nearest enemy, and instantly executes if their health is {2} or below.";
        UpdateDescription();
    }
}