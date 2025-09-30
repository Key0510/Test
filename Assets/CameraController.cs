using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private Vector3 startPosition = new Vector3(-4.75f, 3.5f, -10f); // 초기 위치
    private Vector3 targetPosition = new Vector3(0f, 0f, -10f); // 목표 위치
    private float startFov; // 초기 FOV
    private float targetFov = 53f; // 목표 FOV
    private float moveDuration = 1f; // 이동 시간 (초)
    private bool isMoving = false;
    private float moveTimer = 0f;

    [SerializeField] private Button playButton; // Play 버튼 참조

    private Camera mainCamera;

    void Start()
    {
        // 메인 카메라 참조
        mainCamera = Camera.main;

        // 카메라 초기 설정
        transform.position = startPosition;
        startFov = mainCamera.fieldOfView; // 초기 FOV 저장

        // 카메라가 Perspective 모드인지 확인
        if (mainCamera.orthographic)
        {
            Debug.LogWarning("카메라가 Orthographic 모드입니다. FOV를 변경하려면 Perspective 모드로 설정하세요.");
        }

        // Play 버튼에 클릭 이벤트 추가
        if (playButton != null)
        {
            playButton.onClick.AddListener(StartCameraMove);
        }
        else
        {
            Debug.LogWarning("Play 버튼이 할당되지 않았습니다. 인스펙터에서 버튼을 지정하세요.");
        }
    }

    void Update()
    {
        if (isMoving)
        {
            moveTimer += Time.deltaTime;
            float t = moveTimer / moveDuration;

            // 부드럽게 위치와 FOV를 보간
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCamera.fieldOfView = Mathf.Lerp(startFov, targetFov, t);

            // 이동 완료 시
            if (t >= 1f)
            {
                isMoving = false;
                transform.position = targetPosition;
                mainCamera.fieldOfView = targetFov;
            }
        }
    }

    // Play 버튼 클릭 시 호출
    void StartCameraMove()
    {
        if (!isMoving)
        {
            isMoving = true;
            moveTimer = 0f;
        }
    }
}