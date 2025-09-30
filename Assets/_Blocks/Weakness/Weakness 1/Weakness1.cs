using UnityEngine;

public class Weakness1 : BlockAbility
{
    [SerializeField] private int weaknessAmount = 5; // 약화 수치 (인스펙터에서 설정)

    private BlockManager blockManager;

    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Apply weakness Stacks {0} to nearest enemy";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.weakness += weaknessAmount; // 약화 적용
                enemy.UpdateWeaknessUI(); // 약화 UI 업데이트
                Debug.Log($"{closestEnemy.name}에게 {weaknessAmount} 약화 스택을 적용함. 총 약화 스택: {enemy.weakness}");
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
        Description = string.Format("가장 가까운 적에게 {0} 약화 스택을 적용하고, {1} 방어력을 얻음.", weaknessAmount, Defense); // 설명 업데이트
    }
    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, weaknessAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Apply weakness Stacks {0} to nearest enemy";
        UpdateDescription();
    }
}