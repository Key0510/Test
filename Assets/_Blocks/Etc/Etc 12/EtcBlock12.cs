using UnityEngine;

public class EtcBlock12 : BlockAbility
{
    [SerializeField] private float turnDamageRatio = 0.5f; // 턴 수에 곱해지는 추가 피해 비율 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private Floor floor;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        floor = FindObjectOfType<Floor>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to the nearest enemy, plus an additional {2} of the turn count.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }
        if (floor == null)
        {
            Debug.LogWarning("Floor가 없습니다.");
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
                // 기본 데미지 계산: 블록 데미지 + 플레이어 공격력
                int baseDamage = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;
                // 추가 데미지: 현재 턴 * 비율
                int turnDamage = Mathf.FloorToInt((floor.Turn - 1) * turnDamageRatio);
                // 총 데미지
                int totalDamage = baseDamage + turnDamage;

                enemy.TakeDamage(totalDamage);

                // 디버그 로그
                if (isCriticalHit)
                {
                    Debug.Log($"치명타! {closestEnemy.name}에게 {totalDamage} 데미지(기본: {baseDamage}, 턴 추가: {turnDamage})를 입힘. (현재 턴: {floor.Turn})");
                }
                else
                {
                    Debug.Log($"{closestEnemy.name}에게 {totalDamage} 데미지(기본: {baseDamage}, 턴 추가: {turnDamage})를 입힘. (현재 턴: {floor.Turn})");
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
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)에 턴 수의 {2:P0}를 추가로 주고, {3} 방어력을 얻음.", AttackDamage, CriticalHitDamage, turnDamageRatio, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, turnDamageRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to the nearest enemy, plus an additional {2} of the turn count.";
        UpdateDescription();
    }
}