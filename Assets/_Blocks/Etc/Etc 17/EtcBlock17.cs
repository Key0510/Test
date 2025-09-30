using UnityEngine;

public class EtcBlock17 : BlockAbility
{
    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to the nearest enemy";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();

        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Calculate if critical hit occurs based on PlayerAbility's critChance
                bool isCriticalHit = Random.value < playerAbility.critChance;
                int damageToDeal = isCriticalHit ? CriticalHitDamage : AttackDamage;

                enemy.TakeDamage(damageToDeal + playerAbility.attackPower);

                // Optional: Log critical hit for debugging
                if (isCriticalHit)
                {
                    Debug.Log($"Critical Hit! Dealt {damageToDeal} damage.");
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
        Level++;
        AttackDamage += 1; // Increase attack damage
        Defense += 1; // Increase defense
        CriticalHitDamage += 2; // Increase critical hit damage
        Description = string.Format("Deals {0} damage ({1} on crit) and gains {2} defense.", AttackDamage, CriticalHitDamage, Defense); // Update description
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to the nearest enemy";
        UpdateDescription();
    }
}