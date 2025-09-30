using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    public int Duration = 1;
    public float mul = 1f;
    private Floor floor;
    public TextMeshProUGUI DurationText;
    private BlockManager blockManager;


    void Start()
    {
        GameObject target = GameObject.Find("Floor");
        floor = target.GetComponent<Floor>();
        Duration = Mathf.CeilToInt(floor.Turn * mul);
        DurationText.text = $"{Duration}";

        blockManager = FindObjectOfType<BlockManager>();
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Duration--;
            DurationText.text = $"{Duration}";
            if (Duration == 0)
            {
                blockManager.DisplayBrokenBlock(gameObject);
                if (floor != null && floor.playerAnimator != null)
                {
                    floor.playerAnimator.SetBool("ready", true);
                    Debug.Log("Player animator 'ready' set to true when block is broken");
                }
                gameObject.SetActive(false);
            }
        }
    }
}