using UnityEngine;

public class ShieldBlockAbility : BlockAbility
{
    private PlayerAbility player;

    private void Start()
    {
        player = FindObjectOfType<PlayerAbility>();
        if (player == null)
        {
            Debug.LogError("PlayerAbility ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    public override void Execute()
    {
        if (player != null)
        {
            player.AddShield(defense);  // BlockAbility�� defense ���� ��ȣ������ �߰�
            Debug.Log($"[��ȣ�� ���] �÷��̾�� {defense} ��ȣ�� �߰�");
        }
    }

    public override void Upgrade()
    {
        defense += 5; // ����: ���׷��̵�� ��ȣ�� ��ġ ����
        Description = $"Provides {defense} shield to the player.";
        UpgradeDescription = $"Upgrade: Increases shield by 5.";
    }
}