using UnityEngine;

public class ShopGate : MonoBehaviour, Gate
{
    public Vector2 teleportPosition = new Vector2(-5f, 22f);
    public GameObject prefabToSpawn;
    private GameObject player;
    private Canvas mainCanvas;
    private Canvas ShopCanvas;
    private BGMController bgmController;
    [SerializeField] private int bgmIndexOnEnter = 0;

    void Awake()
    {
        bgmController = FindObjectOfType<BGMController>();
        bgmController.PlayBGM(4);
        player = GameObject.FindWithTag("Player");
        mainCanvas = GameObject.Find("MainCanvas")?.GetComponent<Canvas>();
        if (ShopCanvas == null)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>(true);
            foreach (Canvas canvas in canvases)
            {
                if (canvas.gameObject.name == "ShopCanvas")
                {
                    ShopCanvas = canvas;
                    break;
                }
            }
        }
    }

    public void Enter()
    {
        bgmController.PlayBGM(bgmIndexOnEnter);
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Gate");
        foreach (GameObject obj in objs)
        {
            Destroy(obj);
        }

        if (player != null)
        {
            player.transform.position = teleportPosition;
            PlayerMoving playerMoving = player.GetComponent<PlayerMoving>();
            if (playerMoving != null)
            {
                // 조작 비활성화
                playerMoving.canControl = false;
                Debug.Log("[ShopGate] Player control disabled (canControl = false).");

                // 물리적 이동 멈추기 (Rigidbody2D 속도 초기화)
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    Debug.Log("[ShopGate] Player Rigidbody2D velocity reset to zero.");
                }

                // 애니메이션 상태 초기화
                Animator animator = player.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("isRunning", false);
                    Debug.Log("[ShopGate] Player animator 'isRunning' set to false.");
                }
            }
            else
            {
                Debug.LogWarning("[ShopGate] PlayerMoving component not found on Player.");
            }
        }
        else
        {
            Debug.LogWarning("[ShopGate] Player not found.");
        }

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, new Vector3(5f, 22f, 0f), Quaternion.identity);
        }
        TeleportCamera();
        HideCanvas();
    }

    public void TeleportCamera()
    {
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(0f, 25f, -10f);
            Camera.main.fieldOfView = 25f;
        }
    }

    private void HideCanvas()
    {
        if (mainCanvas != null)
        {
            mainCanvas.gameObject.SetActive(false);
        }
        ShopCanvas.gameObject.SetActive(true);
    }
}