using UnityEngine;

public class ShieldBlock11 : BlockAbility
{
    [Header("훔치기 설정")]
    [Tooltip("가장 가까운 적의 현재 실드에서 훔칠 비율(%)")]
    [Range(0f, 100f)]
    public float stealPercent = 30f;

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
            Debug.LogWarning("[StealShieldFromNearest] PlayerAbility를 찾을 수 없습니다.");
        descriptionTemplate = "The player steals {0}% of the nearest enemy’s Defense.";
    }

    public override void Execute()
    {
        if (playerAbility == null) return;

        // 1) 대상: 가장 가까운 적
        GameObject target = FindClosestEnemy();
        if (target == null) return;

        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy == null) return;

        // 2) 훔칠 양 계산 (적의 현재 실드 x 비율)
        int enemyShieldBefore = (int)enemy.currentShield;
        if (enemyShieldBefore <= 0)
        {
            Debug.Log("[StealShieldFromNearest] 대상 적의 실드가 0입니다. 훔칠 수 없습니다.");
            return;
        }

        int stealAmount = Mathf.FloorToInt(enemyShieldBefore * (stealPercent / 100f));
        if (stealAmount <= 0)
        {
            Debug.Log("[StealShieldFromNearest] 비율이 너무 낮거나 실드가 낮아 0으로 계산되었습니다.");
            return;
        }

        // 3) 적 실드 감소
        enemy.currentShield = Mathf.Max(0, enemy.currentShield - stealAmount);

        // Enemy.UpdateShieldUI()가 private이라 UI 갱신을 위해 0 피해 적용(내부적으로 UI 업데이트)
        enemy.TakeDamage(0);

        // 4) 플레이어에게 그만큼 실드 지급
        playerAbility.AddShield(stealAmount);

        Debug.Log($"[StealShieldFromNearest] 적 실드 {enemyShieldBefore} → {enemy.currentShield} (-{stealAmount}), 플레이어 실드 +{stealAmount}");
    }

    // 예시와 동일: x가 가장 작은 적을 찾는 유틸
    private GameObject FindClosestEnemy()
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
        // Description만 변경 (다른 수치 변경 X)
        Description = $"가장 가까운 적의 현재 실드의 {stealPercent}%를 훔쳐 플레이어 실드로 변환합니다.";
    }
            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, stealPercent);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player steals {0}% of the nearest enemy’s Defense.";
        UpdateDescription();
    }
}
