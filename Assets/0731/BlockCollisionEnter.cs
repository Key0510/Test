
using UnityEngine;

public class BlockCollisionEnter : MonoBehaviour
{
    [SerializeField] private int damage = 10; // 인스펙터에서 설정 가능한 데미지 값
    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
        {
            Debug.LogError("[BlockCollisionHandler] PlayerAbility not found!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Block"))
        {
            Debug.Log($"[BlockCollisionHandler] Block 충돌 감지: {other.gameObject.name}");
            
            // 블록 삭제
            Destroy(other.gameObject);
            Debug.Log($"[BlockCollisionHandler] 블록 삭제됨: {other.gameObject.name}");

            // 플레이어 데미지 적용
            if (playerAbility != null)
            {
                playerAbility.TakeDamage(damage);
                Debug.Log($"[BlockCollisionHandler] 플레이어에게 {damage} 데미지 적용: {other.gameObject.name}");
            }
            else
            {
                Debug.LogWarning("[BlockCollisionHandler] PlayerAbility가 null입니다, 데미지 적용 실패");
            }
        }
    }

    // Update는 현재 불필요하지만, 추가 로직 필요 시 사용 가능
    void Update()
    {
        // 필요 시 추가 로직 (예: 지연 처리, 상태 체크)
        // 현재는 OnTriggerEnter2D로 충분
    }
}
