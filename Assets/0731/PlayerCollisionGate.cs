using UnityEngine;

public class PlayerCollisionGate : MonoBehaviour
{
    [SerializeField] private Canvas mainCanvas; // 인스펙터에서 할당 권장
    [SerializeField] private Vector2 teleportPosition = new Vector2(-6.5f, 3f); // 플레이어 이동 위치
    [SerializeField] private int bgmIndexOnEnter = 0; // BGM 인덱스
    private GameObject player;
    private StageManager stageManager;
    private BlockSpawner blockSpawner;
    private Floor floor;
    private GateSpawner gateSpawner;
    private BGMController bgmController;

    [System.Obsolete]
    void Start()
    {
        gateSpawner = FindObjectOfType<GateSpawner>();
        player = GameObject.FindWithTag("Player");
        stageManager = GameObject.Find("StageManager")?.GetComponent<StageManager>();
        blockSpawner = FindObjectOfType<BlockSpawner>();
        floor = FindObjectOfType<Floor>();
        bgmController = FindObjectOfType<BGMController>();

        // 캔버스 초기화
        if (mainCanvas == null)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>(true);
            foreach (Canvas canvas in canvases)
            {
                if (canvas.gameObject.name == "MainCanvas")
                {
                    mainCanvas = canvas;
                    Debug.Log("[PlayerCollisionGate] MainCanvas found: " + canvas.gameObject.name);
                    break;
                }
            }
        }

        // null 체크
        if (gateSpawner == null) Debug.LogError("[PlayerCollisionGate] GateSpawner not found!");
        if (player == null) Debug.LogError("[PlayerCollisionGate] Player not found!");
        if (stageManager == null) Debug.LogError("[PlayerCollisionGate] StageManager not found!");
        if (blockSpawner == null) Debug.LogError("[PlayerCollisionGate] BlockSpawner not found!");
        if (floor == null) Debug.LogError("[PlayerCollisionGate] Floor not found!");
        if (bgmController == null) Debug.LogError("[PlayerCollisionGate] BGMController not found!");
        if (mainCanvas == null) Debug.LogWarning("[PlayerCollisionGate] MainCanvas not assigned!");
    }

    [System.Obsolete]
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log($"[PlayerCollisionGate] Player 충돌 감지: {other.gameObject.name}");
            Enter();
        }
    }

    [System.Obsolete]
 public void Enter()
    {
        gateSpawner.isRoundEnd = false;
        gateSpawner.flag = true;
        bgmController.PlayBGM(bgmIndexOnEnter);
        floor.CanStart = true;
        SpriteRenderer playerSR = player.GetComponent<SpriteRenderer>();
        if (playerSR != null)
        {
            playerSR.flipX = false;
        }

        // y=-3.78에서 블록 확인
        if (floor != null && floor.CheckBlockAtY(-3.78f))
        {
            // 블록이 없으면 블록 내리고 새 블록 스폰
            if (blockSpawner != null)
            {
                blockSpawner.MoveAllBlocksDown();
                blockSpawner.SpawnBlocks();
                Debug.Log("[BattleGate] Blocks moved down and new blocks spawned.");
            }
            floor.Turn++;
            floor.CanStart = true;
            Debug.Log($"[BattleGate] Turn {floor.Turn} started, CanStart=true");
        }
        else
        {
            // 블록이 있으면 턴 감소 및 데미지 적용
            if (floor != null)
            {
                floor.Turn--;
                PlayerAbility playerAbility = FindObjectOfType<PlayerAbility>();
                if (playerAbility != null)
                {
                    playerAbility.TakeDamage(floor.damageWhenBlockTouchFloor);
                    Debug.Log($"[BattleGate] Block at y=-3.78, applied {floor.damageWhenBlockTouchFloor} damage to player, Turn decremented to {floor.Turn}");
                }
                else
                {
                    Debug.LogWarning("[BattleGate] PlayerAbility not found, damage not applied.");
                }
            }
            else
            {
                Debug.LogWarning("[BattleGate] Floor not found, cannot process block check.");
            }
        }

        // 캔버스가 없으면 다시 찾기 시도
        if (mainCanvas == null)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>(true);
            foreach (Canvas canvas in canvases)
            {
                if (canvas.gameObject.name == "MainCanvas")
                {
                    mainCanvas = canvas;
                    break;
                }
            }
        }

        RevealCanvas();

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Gate");
        foreach (GameObject obj in objs)
        {
            Destroy(obj);
        }

        if (player != null)
        {
            player.transform.position = teleportPosition;
        }
        TeleportCamera();

        if (stageManager != null)
        {
            stageManager.currentStage++;
            // Deactivate all background objects first
            if (stageManager.backgroundManager != null && stageManager.backgroundManager.stageBackgrounds != null)
            {
                foreach (GameObject bg in stageManager.backgroundManager.stageBackgrounds)
                {
                    if (bg != null)
                    {
                        bg.SetActive(false);
                    }
                }

                // Activate the background corresponding to currentStage
                int bgIndex = stageManager.currentStage % stageManager.backgroundManager.stageBackgrounds.Count;
                if (stageManager.backgroundManager.stageBackgrounds[bgIndex] != null)
                {
                    stageManager.backgroundManager.stageBackgrounds[bgIndex].SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"[BattleGate] Background at index {bgIndex} is null.");
                }
            }
            else
            {
                Debug.LogWarning("[BattleGate] BackgroundManager or stageBackgrounds is null.");
            }

            stageManager.SpawnForCurrentStage();
        }
    }

    private void TeleportCamera()
    {
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(0f, 0f, -10f);
            Camera.main.fieldOfView = 53;
            Debug.Log("[PlayerCollisionGate] 카메라 이동: (0, 0, -10), FOV: 53");
        }
        else
        {
            Debug.LogWarning("[PlayerCollisionGate] Main camera not found!");
        }
    }

    private void RevealCanvas()
    {
        if (mainCanvas != null)
        {
            mainCanvas.gameObject.SetActive(true);
            if (mainCanvas.transform.parent != null)
            {
                mainCanvas.transform.parent.gameObject.SetActive(true);
            }
            Debug.Log("[PlayerCollisionGate] MainCanvas 활성화");
        }
        else
        {
            Debug.LogWarning("[PlayerCollisionGate] MainCanvas is null, cannot activate.");
        }
    }
}
