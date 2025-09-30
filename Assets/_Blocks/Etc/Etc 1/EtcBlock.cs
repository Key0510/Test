using UnityEngine;

public class EtcBlock1 : BlockAbility
{
    [SerializeField] private float shieldReductionRatio = 0.5f; // 방어구 감소 비율 (인스펙터에서 설정)

    private BlockManager blockManager;

    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Reduces the armor of the nearest enemy by {0}%.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (enemy.currentShield > 0)
                {
                    int shieldToReduce = Mathf.FloorToInt(enemy.currentShield * shieldReductionRatio); // 감소할 방어구 계산
                    enemy.currentShield = Mathf.Max(0, enemy.currentShield - shieldToReduce); // 방어구 감소
                    enemy.UpdateShieldUI(); // 방어구 UI 갱신
                    Debug.Log($"{closestEnemy.name}의 방어구를 {shieldToReduce}({shieldReductionRatio:P0}) 감소함. 남은 방어구: {enemy.currentShield}");
                }
                else
                {
                    Debug.Log($"{closestEnemy.name}의 방어구가 0이라 감소하지 않음.");
                }
            }
        }
        else
        {
            Debug.Log("적을 찾을 수 없어 방어구를 감소시키지 못함.");
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
        Description = string.Format("가장 가까운 적의 방어구의 {0}%를 감소시킵니다. {1} 방어력을 얻음.", shieldReductionRatio, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, shieldReductionRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Reduces the armor of the nearest enemy by {0}%.";
        UpdateDescription();
    }
}