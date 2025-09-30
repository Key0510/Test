// 랜덤5
using UnityEngine;

public class RandomWeakOrBuffBlockAbility : BlockAbility
{
    [Header("확률 및 효과 설정")]
    public float chance = 0.5f;            // 50% 확률
    public int weaknessAmount = 2;         // 적에게 줄 약화 수치
    public int attackBuffAmount = 3;       // 플레이어가 받을 공격력 증가 수치

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "Increases the nearest enemy’s Weakness stacks by {0}, or increases the player’s Attack Power by {1}.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();

        // 50% 확률로 효과 결정
        bool applyWeakness = Random.value < chance;

        if (applyWeakness)
        {
            if (closestEnemy != null)
            {
                Enemy enemy = closestEnemy.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.weakness += weaknessAmount;
                    enemy.UpdateWeaknessUI();
                    Debug.Log($"적에게 {weaknessAmount}만큼 약화를 부여했습니다.");
                }
            }
        }
        else
        {
            if (playerAbility != null)
            {
                playerAbility.AddAttackPower(attackBuffAmount);
                Debug.Log($"플레이어의 공격력이 {attackBuffAmount}만큼 증가했습니다.");
            }
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
        Description = $"50% 확률로 적에게 {weaknessAmount} 약화를 부여하거나, 플레이어 공격력을 {attackBuffAmount} 증가시킵니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, weaknessAmount, attackBuffAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Increases the nearest enemy’s Weakness stacks by {0}, or increases the player’s Attack Power by {1}.";
        UpdateDescription();
    }
}
