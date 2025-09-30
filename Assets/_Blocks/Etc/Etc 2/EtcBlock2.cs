using UnityEngine;

public class EtcBlock2 : BlockAbility
{
    private PlayerAbility playerAbility;
    private BlockManager blockManager;
    private Animator enemyAnimator;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Ignores all armor of the nearest enemy and deals {0} + player attack damage (or {1} + player attack damage on critical hit).";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        GameObject closestEnemy = FindClosestEnemy();
        enemyAnimator = closestEnemy.GetComponent<Animator>();

        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 치명타 확률에 따라 치명타 여부 결정
                bool isCriticalHit = Random.value < playerAbility.critChance;
                // 데미지 계산: 블록 데미지 + 플레이어 공격력
                int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

                // 방어구 무시하고 체력에 직접 데미지 적용
                enemy.currentHealth = Mathf.Max(0, enemy.currentHealth - damageToDeal);
                enemy.UpdateHealthUI(); // 체력 UI 갱신
                enemyAnimator.SetTrigger("isDamaged");

                if (enemy.currentHealth <= 0)
                {
                    enemy.Die(); // 체력 0 이하 시 사망 처리
                }

                // 디버그 로그
                if (isCriticalHit)
                {
                    Debug.Log($"치명타! {closestEnemy.name}의 방어구를 무시하고 {damageToDeal} 데미지를 입힘. (현재 체력: {enemy.currentHealth}, 방어구: {enemy.currentShield})");
                }
                else
                {
                    Debug.Log($"{closestEnemy.name}의 방어구를 무시하고 {damageToDeal} 데미지를 입힘. (현재 체력: {enemy.currentHealth}, 방어구: {enemy.currentShield})");
                }
            }
        }
        else
        {
            Debug.Log("적을 찾을 수 없어 데미지를 입히지 못함.");
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
        Description = string.Format("가장 가까운 적의 방어구를 모두 무시하고 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 입힙니다.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Ignores all armor of the nearest enemy and deals {0} + player attack damage (or {1} + player attack damage on critical hit).";
        UpdateDescription();
    }
}