using UnityEngine;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    private int collisionCount = 0; // 충돌 횟수
    private int maxCollisionsBeforeAngleAdjust = 20; // 각도 조정 전 최대 충돌 횟수
    private int gravitytrigger = 0;
    private bool hasStopped;
    public float minSpeed = 10f; // 최소 속도
    private List<float> recentYPositions = new List<float>(); // Track recent Y positions
    private float yPositionThreshold = 0.2f; // Threshold for considering Y positions similar
    private float downwardForce = 0.5f; // Downward force to apply

    [SerializeField] private AudioClip blockHitSound; // 블록 충돌 시 사운드
    [SerializeField] private AudioClip blockDestroySound; // 블록 파괴 시 사운드
    private AudioSource hitAudioSource; // 충돌 사운드용 AudioSource
    private AudioSource destroyAudioSource; // 파괴 사운드용 AudioSource

    [System.Obsolete]
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        // 충돌 사운드용 AudioSource 설정
        hitAudioSource = gameObject.AddComponent<AudioSource>();
        hitAudioSource.playOnAwake = false;
        hitAudioSource.loop = false;

        // 파괴 사운드용 AudioSource 설정
        destroyAudioSource = gameObject.AddComponent<AudioSource>();
        destroyAudioSource.playOnAwake = false;
        destroyAudioSource.loop = false;

        // 사운드 클립 확인
        if (blockHitSound == null)
        {
            Debug.LogWarning($"공 {gameObject.name}에 블록 충돌 사운드가 설정되지 않았습니다.");
        }
        if (blockDestroySound == null)
        {
            Debug.LogWarning($"공 {gameObject.name}에 블록 파괴 사운드가 설정되지 않았습니다.");
        }
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude < minSpeed)
        {
            BoostSpeed();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            return;
        }

        if (collision.gameObject.CompareTag("Block"))
        {
            // 블록 컴포넌트 참조
            Block block = collision.gameObject.GetComponent<Block>();
            if (block != null)
            {
                // 충돌 사운드 재생
                if (blockHitSound != null)
                {
                    hitAudioSource.PlayOneShot(blockHitSound);
                }
                else
                {
                    Debug.LogWarning("블록 충돌 사운드가 설정되지 않았습니다.");
                }

                if (block.DurationText != null)
                {
                    block.DurationText.text = $"{block.Duration}";
                }

                // Duration이 0이 되면 파괴 사운드 재생 및 블록 처리
                if (block.Duration == 1)
                {
                    if (blockDestroySound != null)
                    {
                        destroyAudioSource.PlayOneShot(blockDestroySound);
                    }
                    else
                    {
                        Debug.LogWarning("블록 파괴 사운드가 설정되지 않았습니다.");
                    }

                    // BlockManager 참조하여 brokenBlocks에 추가
                    BlockManager blockManager = FindObjectOfType<BlockManager>();
                    if (blockManager != null)
                    {
                        blockManager.brokenBlocks.Add(collision.gameObject);
                    }
                    collision.gameObject.SetActive(false);
                }
            }

            rb.gravityScale = 0.001f;
            gravitytrigger = 0;
        }
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Block"))
        {
            gravitytrigger++;
            if (gravitytrigger > 30)
            {
                rb.gravityScale = 0.03f;
            }

            // Record current Y position
            float currentY = transform.position.y;
            recentYPositions.Add(currentY);

            // Keep only the last two Y positions
            if (recentYPositions.Count > 2)
            {
                recentYPositions.RemoveAt(0);
            }

            // Check if the last two Y positions are similar
            if (recentYPositions.Count == 2)
            {
                float previousY = recentYPositions[0];
                if (Mathf.Abs(currentY - previousY) < yPositionThreshold)
                {
                    // Apply downward force
                    rb.AddForce(Vector2.down * downwardForce, ForceMode2D.Impulse);
                    Debug.Log($"[Ball] Applied downward force to {gameObject.name} due to similar Y positions. CurrentY={currentY}, PreviousY={previousY}");
                }
                else
                {
                    Debug.Log($"[Ball] Y positions differ: CurrentY={currentY}, PreviousY={previousY}, Delta={Mathf.Abs(currentY - previousY)}");
                }
            }

            BoostSpeed();
        }
    }

    [System.Obsolete]
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            StopBall();
            FindObjectOfType<BallManager>().BallStopped(this.gameObject);
            Destroy(gameObject);
        }
    }

    [System.Obsolete]
    private void StopBall()
    {
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        hasStopped = true;
    }

    private void BoostSpeed()
    {
        rb.linearVelocity = rb.linearVelocity.normalized * minSpeed;
    }

    public static void SetMinSpeedTo20()
    {
        Ball[] balls = FindObjectsOfType<Ball>();
        if (balls.Length == 0)
        {
            Debug.LogWarning("[Ball] No Ball objects found in the scene.");
            return;
        }

        foreach (Ball ball in balls)
        {
            ball.minSpeed = 25f;
            if (ball.rb.linearVelocity.magnitude < ball.minSpeed)
            {
                ball.BoostSpeed();
            }
            Debug.Log($"[Ball] Set minSpeed to 30 for {ball.gameObject.name}.");
        }
    }
}