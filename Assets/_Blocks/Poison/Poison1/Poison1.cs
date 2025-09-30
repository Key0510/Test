using UnityEngine;

public class Poison1 : BlockAbility
{
    //가장 가까운 적에게 독스택을 5줌

    [SerializeField] protected int poisonAmount = 5;

    void Start()
    {
        descriptionTemplate = "Applies {0} Poison stacks to the nearest enemy.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();

        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.poison += poisonAmount;
            }
        }
    }

    private GameObject FindClosestEnemy()
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
        Description = string.Format("Adds {0} poison stacks to the closest enemy and gains {1} defense.", poisonAmount, Defense); // Description 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, poisonAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Applies {0} Poison stacks to the nearest enemy.";
        UpdateDescription();
    }
}