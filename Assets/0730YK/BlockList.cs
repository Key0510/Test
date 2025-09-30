using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockList : MonoBehaviour
{
    public List<GameObject> blockPrefabs = new List<GameObject>();

    // Event to notify when the block list changes
    public event Action OnBlockListChanged;

    public bool AddBlock(GameObject block)
    {
        if (block == null)
        {
            Debug.LogWarning("[BlockList] Attempted to add null block.");
            return false;
        }

        blockPrefabs.Add(block);
        OnBlockListChanged?.Invoke(); // Notify listeners of change
        Debug.Log($"[BlockList] Added block: {block.name}");
        return true;
    }

    // Optional: Method to update block at specific index (used by BlockButtonSpawner)
    public void UpdateBlock(int index, GameObject newBlock)
    {
        if (index >= 0 && index < blockPrefabs.Count)
        {
            blockPrefabs[index] = newBlock;
            OnBlockListChanged?.Invoke(); // Notify listeners of change
            Debug.Log($"[BlockList] Updated block at index {index} to {newBlock.name}");
        }
        else
        {
            Debug.LogWarning($"[BlockList] Invalid index {index} for UpdateBlock.");
        }
    }
}