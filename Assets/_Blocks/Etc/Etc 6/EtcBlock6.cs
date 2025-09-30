using UnityEngine;

public class EtcBlock6 : BlockAbility
{
    [SerializeField] private float durationMultiplier = 0.5f; // Duration 배율 (인스펙터에서 설정, 기본값 0.5)

    private Block block;
    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        block = GetComponent<Block>();
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Duration is set to {0} of base durability, and deals {1} + player attack damage (or {2} + player attack damage on critical hit) to the nearest enemy.";

        if (block != null)
        {
            block.Duration = Mathf.FloorToInt(block.Duration * durationMultiplier); // Duration을 절반으로 설정
            Debug.Log($"EtcBlock6 블록 Duration 설정: {block.Duration} (기본 Duration * {durationMultiplier})");
        }
        else
        {
            Debug.LogWarning("Block 컴포넌트를 찾을 수 없습니다.");
        }
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

                enemy.TakeDamage(damageToDeal);

                // 디버그 로그
                if (isCriticalHit)
                {
                    Debug.Log($"치명타! {closestEnemy.name}에게 {damageToDeal} 데미지를 줌. (현재 Duration: {(block != null ? block.Duration : 0)})");
                }
                else
                {
                    Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 줌. (현재 Duration: {(block != null ? block.Duration : 0)})");
                }
            }
        }
        else
        {
            Debug.Log($"적을 찾을 수 없어 공격하지 못함. (현재 Duration: {(block != null ? block.Duration : 0)})");
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
        Description = string.Format("Duration이 기본 내구도의 {0}로 설정되고, 가장 가까운 적에게 {1} + 플레이어 공격력 데미지(치명타 시 {2} + 플레이어 공격력)를 주며, {3} 방어력을 얻음.", durationMultiplier, AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, durationMultiplier, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Duration is set to {0} of base durability, and deals {1} + player attack damage (or {2} + player attack damage on critical hit) to the nearest enemy.";
        UpdateDescription();
    }
}