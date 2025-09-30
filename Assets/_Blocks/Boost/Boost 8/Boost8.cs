// 버프8
using UnityEngine;

public class Boost8 : BlockAbility
{
    [Header("증가 설정")]
    [Tooltip("적의 약화(weakness) 1스택당 증가할 플레이어 공격력")]
    public int attackPerWeaknessStack = 1;  // 인스펙터에서 조절

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
            Debug.LogWarning("[WeaknessStacksToAttackBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        descriptionTemplate = "The player gains {0} Attack Power for each Weakness stack on the nearest enemy.";
    }

    public override void Execute()
    {
        if (playerAbility == null) return;

        // 가장 가까운 적 찾기 (x 좌표가 가장 작은 적)
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy == null) return;

        Enemy enemy = closestEnemy.GetComponent<Enemy>();
        if (enemy == null) return;

        // 적의 약화 스택 확인
        int stacks = (int)Mathf.Max(0, enemy.weakness);
        int gain = stacks * attackPerWeaknessStack;

        // 플레이어 공격력 증가
        int before = playerAbility.attackPower;
        playerAbility.AddAttackPower(gain);

        Debug.Log($"[WeaknessStacksToAttack] 약화 스택 {stacks} → 공격력 +{gain} ({before} ▶ {playerAbility.attackPower})");
    }

    // 예시와 동일: x가 가장 작은 적을 찾는 로직
    public GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
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
                        closest = enemy;
                    }
                }
            }
        }
        return closest;
    }

    public override void Upgrade()
    {
        // Description만 변경 (다른 값은 변경하지 않음)
        Description = $"가장 가까운 적의 약화 스택 × {attackPerWeaknessStack} 만큼 플레이어 공격력을 증가시킵니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackPerWeaknessStack);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player gains {0} Attack Power for each Weakness stack on the nearest enemy.";
        UpdateDescription();
    }
}
