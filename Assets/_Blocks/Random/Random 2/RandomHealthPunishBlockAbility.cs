using UnityEngine;

public class RandomHealthPunishBlockAbility : BlockAbility
{
    [Header("Ȯ�� ����")]
    [Range(0f, 1f)]
    public float playerDamageChance = 50f;  // �÷��̾ ���� ���� Ȯ��
    public float mul = 0.5f;

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "Deals damage equal to {0}% of current HP to either yourself or the nearest enemy.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility�� ã�� �� �����ϴ�.");
            return;
        }

        bool damagePlayer = Random.value < playerDamageChance;

        if (damagePlayer)
        {
            int damage = Mathf.FloorToInt(playerAbility.currentHealth * (mul / 100));
            playerAbility.TakeDamage(damage);
            Debug.Log($"[���� ü�� �ս�] �÷��̾ �ڽ��� ü�� 50%({damage}) ���ظ� �Ծ����ϴ�.");
        }
        else
        {
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                Enemy enemy = closestEnemy.GetComponent<Enemy>();
                if (enemy != null && !enemy.IsDead())
                {
                    int damage = Mathf.FloorToInt(enemy.currentHealth * 0.5f);
                    enemy.TakeDamage(damage);
                    Debug.Log($"[���� ü�� �ս�] ���� ����� ���� �ڽ��� ü�� 50%({damage}) ���ظ� �Ծ����ϴ�.");
                }
            }
            else
            {
                Debug.Log("���� �������� �ʾ� �ƹ� �ϵ� �Ͼ�� �ʾҽ��ϴ�.");
            }
        }
    }

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
        Description = $"50% Ȯ���� �÷��̾ ���� ����� ���� �ڽ��� ü���� 50% ���ظ� �Խ��ϴ�.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, mul);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals damage equal to {0}% of current HP to either yourself or the nearest enemy.";
        UpdateDescription();
    }
}
