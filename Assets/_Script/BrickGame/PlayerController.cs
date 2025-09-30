using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public BallManager ballManager;
    public GameObject rectanglePrefab;
    private GameObject trajectoryRectangle;
    private Floor floor;

    private Vector3 dragStartPosition;
    private Vector2 launchDirection;
    private bool isDragging = false;
    public GamePause gamePause;

    [System.Obsolete]
    void Start()
    {
        floor = FindObjectOfType<Floor>();
        trajectoryRectangle = Instantiate(rectanglePrefab, Vector3.zero, Quaternion.identity, transform);
        trajectoryRectangle.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gamePause.isPaused)
        {
            if (ballManager.CanLaunch() && floor.CanStart && !gamePause.isPaused)
            {
                dragStartPosition = GetMouseWorldPosition();
                trajectoryRectangle.SetActive(true);
                isDragging = true;
            }
            else if (Input.GetMouseButtonDown(0) && gamePause.isPaused)
            {
                trajectoryRectangle.SetActive(false);
                isDragging = false;
            }
        }

        if (Input.GetMouseButton(0) && isDragging && ballManager.CanLaunch() && floor.CanStart && !gamePause.isPaused)
        {
            floor.CollisionCount = 0;
            Vector3 currentMousePosition = GetMouseWorldPosition();

            launchDirection = (currentMousePosition - ballManager.GetLaunchPosition()).normalized;

            launchDirection = AdjustLaunchDirection(launchDirection);

            UpdateTrajectoryRectangle(ballManager.GetLaunchPosition(), currentMousePosition);
        }

        if (Input.GetMouseButtonUp(0) && isDragging && !gamePause.isPaused)
        {
            if (ballManager.CanLaunch() && !gamePause.isPaused)
            {
                ballManager.LaunchBalls(launchDirection);
            }
            else if (Input.GetMouseButtonDown(0) && gamePause.isPaused)
            {
                trajectoryRectangle.SetActive(false);
                isDragging = false;
            }
            HideTrajectory();
            isDragging = false;
        }
        else if (Input.GetMouseButtonDown(0) && gamePause.isPaused)
        {
            trajectoryRectangle.SetActive(false);
            isDragging = false;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;         
        float zDistance = Mathf.Abs(Camera.main.transform.position.z - ballManager.GetLaunchPosition().z);
        return Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zDistance));
    }

    private Vector2 AdjustLaunchDirection(Vector2 direction)
    {
        float minAngle = 5f;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle < minAngle && angle > -minAngle)
        {
            angle = minAngle;
        }
        else if (angle < -minAngle)
        {
            angle = minAngle;
        }

        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }

    private void UpdateTrajectoryRectangle(Vector3 startPosition, Vector3 currentMousePosition)
    {
        Vector2 start = new Vector2(startPosition.x, startPosition.y);
        Vector2 end = new Vector2(currentMousePosition.x, currentMousePosition.y);

        trajectoryRectangle.transform.position = (start + end) / 2;

        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
        trajectoryRectangle.transform.rotation = Quaternion.Euler(0, 0, angle);

        float distance = Vector2.Distance(start, end);
        trajectoryRectangle.transform.localScale = new Vector3(distance, trajectoryRectangle.transform.localScale.y, 1);
    }

    private void HideTrajectory()
    {
        trajectoryRectangle.SetActive(false);
    }
}