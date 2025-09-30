using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가

public class BallManager : MonoBehaviour
{
    public GameObject ballPrefab;       
    public GameObject launchIndicatorPrefab;    
    public float ballSpeed = 10f;
    public float launchInterval = 0.1f;  
    public int playerBallCount = 1;         
    public int addcount = 0;
    public GamePause gamePause;

    [SerializeField] private TMP_Text ballCountText; // 공 개수를 표시할 TMP 텍스트

    private List<GameObject> activeBalls = new List<GameObject>();      
    private Vector3 launchPosition = new Vector3(0, -4, 0);
    private Vector3 lastBallPosition; 
    private GameObject launchIndicator;
    private bool isLaunching = false;
    private Floor floor;

    [System.Obsolete]
    void Start()
    {
        floor = FindObjectOfType<Floor>();
        launchIndicator = Instantiate(launchIndicatorPrefab, launchPosition, Quaternion.identity);

        // ballCountText null 체크
        if (ballCountText == null)
        {
            Debug.LogWarning("[BallManager] ballCountText is not assigned!");
        }
        UpdateBallCountUI(); // 초기 UI 업데이트

        gamePause = FindObjectOfType<GamePause>();
    }

    public Vector3 GetLaunchPosition()
    {
        return launchPosition;
    }

    public bool CanLaunch()
    {
        return !isLaunching && activeBalls.Count  == 0  && !gamePause.isPaused;                   
    }

    public void LaunchBalls(Vector2 direction)
    {
        if (CanLaunch() && floor.CanStart == true)
        {
            isLaunching = true;  

            if (launchIndicator != null)
            {
                Destroy(launchIndicator);
            }

            StartCoroutine(LaunchBallSequence(direction));
        }
    }

    private IEnumerator LaunchBallSequence(Vector2 direction)
    {
        for (int i = 0; i < playerBallCount; i++)
        {
            GameObject newBall = Instantiate(ballPrefab, launchPosition + new Vector3(0, 0, 0), Quaternion.identity);
            activeBalls.Add(newBall);
            Rigidbody2D rb = newBall.GetComponent<Rigidbody2D>();
            rb.linearVelocity = direction.normalized * ballSpeed;
            yield return new WaitForSeconds(launchInterval);
        }
    }

    public void IncreaseBallCount()
    {
        playerBallCount++;
        UpdateBallCountUI(); // 공 개수 증가 시 UI 업데이트
    }

    public void SetAddCount(int value)
    {
        addcount = value;
        UpdateBallCountUI(); // addcount 변경 시 UI 업데이트
    }

    public void BallStopped(GameObject ball)
    {
        lastBallPosition = ball.transform.position;   
        activeBalls.Remove(ball);

        if (activeBalls.Count == 0)
        {
            EndTurn();                            
        }
    }

    private void EndTurn()
    {          
        launchPosition = new Vector3(lastBallPosition.x, lastBallPosition.y + 0.1f, lastBallPosition.z);
        isLaunching = false;   
                                                            
        if (launchIndicatorPrefab != null)
        {
            launchIndicator = Instantiate(launchIndicatorPrefab, launchPosition, Quaternion.identity);
        }
    }

    public void UpdateBallCountUI()
    {
        if (ballCountText != null)
        {
            ballCountText.text = $"{playerBallCount + addcount}";
            Debug.Log($"[BallManager] Updated ball count UI: {playerBallCount + addcount}");
        }
    }
}