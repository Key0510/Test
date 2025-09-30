using UnityEngine;

public class PlayerMoving : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool canControl = true; // 플레이어 조작 가능 여부

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public bool isOnGate = false;
    private Gate currentGate; // 현재 충돌 중인 Gate를 저장
    private GateSpawner gateSpawner;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gateSpawner = FindObjectOfType<GateSpawner>();
    }

    void Update()
    {
        if (!canControl)
        {
            // 조작 비활성화 시 이동과 애니메이션 멈춤
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("isRunning", false);
            return;
        }

        float moveInput = 0f;

        if (Input.GetKey(KeyCode.A) && gateSpawner.isRoundEnd == true)
            moveInput = -1f;
        else if (Input.GetKey(KeyCode.D) && gateSpawner.isRoundEnd == true)
            moveInput = 1f;

        if (moveInput < 0)
            spriteRenderer.flipX = true;
        else if (moveInput > 0)
            spriteRenderer.flipX = false;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        bool isRunning = moveInput != 0;
        animator.SetBool("isRunning", isRunning);

        if (isOnGate && Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Gate에 있습니다. W키를 눌렀습니다.");
            currentGate?.Enter();  // 널 확인 후 실행
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Gate"))
        {
            currentGate = other.GetComponent<Gate>();
            isOnGate = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Gate"))
        {
            isOnGate = false;
            currentGate = null;
        }
    }

    public void EnableControl()
    {
        canControl = true;
        Debug.Log("[PlayerMoving] Player control enabled (canControl = true).");

        // 이동 상태 초기화 (필요 시)
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            Debug.Log("[PlayerMoving] Rigidbody2D velocity reset to zero.");
        }

        // 애니메이션 상태 초기화 (필요 시)
        if (animator != null)
        {
            animator.SetBool("isRunning", false);
            Debug.Log("[PlayerMoving] Animator 'isRunning' set to false.");
        }
    }
}