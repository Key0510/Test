using UnityEngine;
using System.Collections;

public class AddBall : MonoBehaviour
{
    public float blinkInterval = 0.3f;
    private SpriteRenderer spriteRenderer;
    private BallManager ballmanager;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ballmanager = FindObjectOfType<BallManager>();
    }

    [System.Obsolete]
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            BallManager ballManager = FindObjectOfType<BallManager>();
            if (ballManager != null)
            {
                ballManager.addcount++;
            }
            Destroy(gameObject);
        }
        ballmanager.UpdateBallCountUI();

    }
    void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}
