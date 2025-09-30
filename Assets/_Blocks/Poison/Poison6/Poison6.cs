using UnityEngine;

public class Poison6 : BlockAbility
{
    private PlayerAbility playerAbility;

    public float multiplier;
    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "Applies Poison stacks equal to {0} × Player’s Attack Power to the farthest enemy.";
    }

    public override void Execute()
    {
        GameObject farthestEnemy = FindFarthestEnemy();
        if (farthestEnemy != null)
        {
            Enemy enemy = farthestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.poison += (int)(playerAbility.attackPower * multiplier); // Increase poison stacks by player's attackPower
                Debug.Log($"Added {playerAbility.attackPower} poison stacks to {farthestEnemy.name}. Total poison: {enemy.poison}");
            }
        }
    }

    public GameObject FindFarthestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject farthestEnemy = null;
        float maxX = -Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            float xPos = enemy.transform.position.x;
            if (xPos > maxX)
            {
                maxX = xPos;
                farthestEnemy = enemy;
            }
        }
        return farthestEnemy;
    }

    public override void Upgrade()
    {
        Description = string.Format("Increases poison stacks by {0} on the farthest enemy and gains {1} defense.", playerAbility.attackPower, Defense); // Update description
    }
                    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, multiplier);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Applies Poison stacks equal to {0} × Player’s Attack Power to the farthest enemy.";
        UpdateDescription();
    }
}