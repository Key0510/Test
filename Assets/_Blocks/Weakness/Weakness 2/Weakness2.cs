using UnityEngine;

public class Weakness2 : BlockAbility
{
    [SerializeField] private int baseWeaknessAmount = 3; // 기본 약화 수치 (인스펙터에서 설정)
    [SerializeField] private int bonusWeaknessAmount = 5; // 약화 스택 1 이상 시 추가 약화 수치 (인스펙터에서 설정)

    private BlockManager blockManager;

    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Applies {0} Weakness stacks to the nearest enemy. If already weakened, applies {1} instead.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                int initialWeakness = (int)enemy.weakness; // 초기 약화 스택 저장
                enemy.weakness += baseWeaknessAmount; // 기본 약화 적용

                // 약화 스택이 1 이상이었으면 추가 약화 적용
                if (initialWeakness >= 1)
                {
                    enemy.weakness += bonusWeaknessAmount;
                    enemy.UpdateWeaknessUI(); // 약화 UI 업데이트
                    Debug.Log($"{closestEnemy.name}에게 {baseWeaknessAmount} 약화 스택 적용, 초기 약화 {initialWeakness}로 {bonusWeaknessAmount} 추가 약화 적용. 총 약화 스택: {enemy.weakness}");
                }
                else
                {
                    enemy.UpdateWeaknessUI(); // 약화 UI 업데이트
                    Debug.Log($"{closestEnemy.name}에게 {baseWeaknessAmount} 약화 스택 적용. 총 약화 스택: {enemy.weakness}");
                }
            }
        }
        else
        {
            Debug.Log("적을 찾을 수 없어 약화를 적용하지 못함.");
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
        Description = string.Format("가장 가까운 적에게 {0} 약화 스택을 적용하고, 이미 약화 스택이 1 이상이면 추가로 {1} 약화 스택을 적용하며, {2} 방어력을 얻음.", baseWeaknessAmount, bonusWeaknessAmount, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, baseWeaknessAmount, bonusWeaknessAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate =  "Applies {0} Weakness stacks to the nearest enemy. If already weakened, applies {1} instead.";
        UpdateDescription();
    }
}