using UnityEngine;

public class ShieldBlockAbility : BlockAbility
{
    private PlayerAbility player;

    private void Start()
    {
        player = FindObjectOfType<PlayerAbility>();
        if (player == null)
        {
            Debug.LogError("PlayerAbility 컴포넌트를 찾을 수 없습니다.");
        }
    }

    public override void Execute()
    {
        if (player != null)
        {
            player.AddShield(defense);  // BlockAbility의 defense 값을 보호막으로 추가
            Debug.Log($"[보호막 블록] 플레이어에게 {defense} 보호막 추가");
        }
    }

    public override void Upgrade()
    {
        defense += 5; // 예시: 업그레이드시 보호막 수치 증가
        Description = $"Provides {defense} shield to the player.";
        UpgradeDescription = $"Upgrade: Increases shield by 5.";
    }
}