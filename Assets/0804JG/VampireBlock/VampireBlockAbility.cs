using UnityEngine;

public class VampireBlockAbility : BlockAbility
{
    [SerializeField, Range(0f, 1f)] private float lifestealRatio = 0.3f; // Lifesteal ratio (e.g., 0.3 = 30%)
    private PlayerAbility player;

    private void Start()
    {
        player = FindObjectOfType<PlayerAbility>();
        if (player == null)
        {
            Debug.LogError("PlayerAbility component not found.");
        }
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead())
            {
                // Calculate if critical hit occurs based on PlayerAbility's critChance
                bool isCriticalHit = Random.value < player.critChance;
                int damageToDeal = isCriticalHit ? CriticalHitDamage + player.attackPower : AttackDamage + player.attackPower;

                enemy.TakeDamage(damageToDeal);

                // Heal based on damage dealt
                int healAmount = Mathf.CeilToInt(damageToDeal * lifestealRatio);
                if (player != null)
                {
                    player.Heal(healAmount);
                    Debug.Log($"Healed: {healAmount} (Critical: {isCriticalHit})");
                }
            }
        }
    }

    public override void Upgrade()
    {
        Level++;
        lifestealRatio = Mathf.Min(lifestealRatio + 0.1f, 1f); // Increase lifesteal, cap at 100%
        AttackDamage += 1; // Increase attack damage
        CriticalHitDamage += 2; // Increase critical hit damage
        Description = string.Format("Deals {0} damage ({1} on crit) and heals {2}% of damage dealt.", AttackDamage, CriticalHitDamage, lifestealRatio * 100);
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minX = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float xPos = enemy.transform.position.x;
            if (xPos < minX)
            {
                minX = xPos;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }
}