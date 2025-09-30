

using UnityEngine;

public class RandomPoisonOrWeak : BlockAbility
{
    [Header("���� ȿ�� ����")]
    public float chance = 0.5f;               // 50% Ȯ��
    public int poisonStackAmount = 2;         // ������ �� ���� ��
    public int weaknessStackAmount = 2;       // ������ ��ȭ ���� ��

    private void Start()
    {
        descriptionTemplate = "Increases the nearest enemy’s Weakness stacks by {0}, or increases their Poison stacks by {1}.";
        // �ʱ�ȭ �۾� �ʿ� �� ���⿡ �߰�
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();

        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                bool applyPoison = Random.value < chance;

                if (applyPoison)
                {
                    enemy.poison += poisonStackAmount;
                    enemy.UpdatePoisonUI();
                    Debug.Log($"������ �� ���� {poisonStackAmount}��ŭ �ο���.");
                }
                else
                {
                    enemy.weakness += weaknessStackAmount;
                    enemy.UpdateWeaknessUI();
                    Debug.Log($"������ ��ȭ ���� {weaknessStackAmount}��ŭ �ο���.");
                }
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
        Description = $"50% Ȯ���� ������ �� {poisonStackAmount} ���� �Ǵ� ��ȭ {weaknessStackAmount} ������ �ο��մϴ�.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, weaknessStackAmount, poisonStackAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Increases the nearest enemy’s Weakness stacks by {0}, or increases their Poison stacks by {1}.";
        UpdateDescription();
    }
}
