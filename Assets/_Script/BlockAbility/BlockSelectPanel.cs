using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BlockSelectPanel : MonoBehaviour
{
    private TextMeshProUGUI nameText; // UI Text field for block name
    private TextMeshProUGUI descriptionText; // UI Text field for block description
    private Button selectButton; // Button component on this GameObject
    [SerializeField] private GameObject blockPrefab; // Assigned block prefab
    [SerializeField] private BlockChoice blockChoice; // Reference to BlockChoice component

    private BlockAbility blockAbility;

    private void Awake()
    {
        // Get the Button component from this GameObject
        selectButton = GetComponent<Button>();
        if (selectButton == null)
        {
            Debug.LogError("Button component not found on " + gameObject.name, gameObject);
        }

        // Automatically find TextMeshProUGUI components in children
        Transform titleTransform = transform.Find("TitleText");
        if (titleTransform == null)
        {
            Debug.LogError("TitleText GameObject not found as a child of " + gameObject.name, gameObject);
        }
        else
        {
            nameText = titleTransform.GetComponent<TextMeshProUGUI>();
            if (nameText == null)
            {
                Debug.LogError("TitleText GameObject does not have a TextMeshProUGUI component", titleTransform.gameObject);
            }
        }

        Transform descriptionTransform = transform.Find("Discription Text");
        if (descriptionTransform == null)
        {
            Debug.LogError("DescriptionText GameObject not found as a child of " + gameObject.name, gameObject);
        }
        else
        {
            descriptionText = descriptionTransform.GetComponent<TextMeshProUGUI>();
            if (descriptionText == null)
            {
                Debug.LogError("DescriptionText GameObject does not have a TextMeshProUGUI component", descriptionTransform.gameObject);
            }
        }

        // Add listener to button to call AddBlock on BlockChoice and deactivate
        if (selectButton != null && blockChoice != null)
        {
            selectButton.onClick.AddListener(() =>
            {
                blockChoice.AddBlock(blockPrefab);
                gameObject.SetActive(false); // Deactivate the button (and its parent panel) after adding
            });
        }
        else
        {
            if (selectButton == null)
            {
                Debug.LogError("Cannot add button listener: selectButton is null", gameObject);
            }
            if (blockChoice == null)
            {
                Debug.LogError("Cannot add button listener: blockChoice is not assigned in the Inspector", gameObject);
            }
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    // Method to assign a new block prefab and update UI
    public void SetBlockPrefab(GameObject newBlockPrefab)
    {
        blockPrefab = newBlockPrefab;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (blockPrefab != null)
        {
            blockAbility = blockPrefab.GetComponent<BlockAbility>();
            if (blockAbility != null)
            {
                if (nameText != null)
                {
                    nameText.text = blockAbility.Name;
                }
                if (descriptionText != null)
                {
                    descriptionText.text = blockAbility.Description;
                }
            }
            else
            {
                if (nameText != null)
                {
                    nameText.text = "No Block Assigned";
                }
                if (descriptionText != null)
                {
                    descriptionText.text = "";
                }
            }
        }
        else
        {
            if (nameText != null)
            {
                nameText.text = "No Block Assigned";
            }
            if (descriptionText != null)
            {
                descriptionText.text = "";
            }
        }
    }

    private void OnDestroy()
    {
        // Clean up button listener
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
        }
    }
}