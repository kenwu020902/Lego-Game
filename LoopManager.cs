using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LoopManager : MonoBehaviour
{
    [System.Serializable]
    public class LoopSlot
    {
        public int slotNumber;
        public bool isOccupied = false;
        public ShapeType storedShape;
        public Image slotIcon;
        public GameObject occupiedEffect;
        public GameObject emptyEffect;
        
        public bool AddShape(ShapeType shape)
        {
            if (!isOccupied)
            {
                isOccupied = true;
                storedShape = shape;
                UpdateSlotDisplay();
                return true;
            }
            return false;
        }
        
        public ShapeType RemoveShape()
        {
            if (isOccupied)
            {
                isOccupied = false;
                ShapeType returnedShape = storedShape;
                storedShape = ShapeType.Triangle; // Default
                UpdateSlotDisplay();
                return returnedShape;
            }
            return ShapeType.Triangle; // Default if empty
        }
        
        void UpdateSlotDisplay()
        {
            if (slotIcon != null)
            {
                slotIcon.gameObject.SetActive(isOccupied);
                // Set sprite based on storedShape
            }
            
            if (occupiedEffect != null)
                occupiedEffect.SetActive(isOccupied);
                
            if (emptyEffect != null)
                emptyEffect.SetActive(!isOccupied);
        }
    }
    
    [Header("Loop Slots")]
    public LoopSlot[] loopSlots = new LoopSlot[5]; // 5 slots
    
    [Header("References")]
    public BinManager binManager;
    public GameObject autoCollectEffect;
    
    void Start()
    {
        // Initialize all slots
        foreach (LoopSlot slot in loopSlots)
        {
            slot.isOccupied = false;
        }
    }
    
    public bool AddToLoop(Shape shape)
    {
        // Find first empty slot
        foreach (LoopSlot slot in loopSlots)
        {
            if (!slot.isOccupied)
            {
                slot.AddShape(shape.shapeType);
                Debug.Log("Shape " + shape.shapeType + " added to loop slot " + slot.slotNumber);
                
                // Check if this new stored shape matches any bin immediately
                CheckSingleLoopAgainstBins(slot);
                
                return true;
            }
        }
        
        Debug.Log("No empty loop slots!");
        return false;
    }
    
    void CheckSingleLoopAgainstBins(LoopSlot slot)
    {
        if (slot.isOccupied)
        {
            // Check if stored shape matches either bin
            if (slot.storedShape == binManager.GetBin1Shape() ||
                slot.storedShape == binManager.GetBin2Shape())
            {
                AutoCollectFromLoop(slot);
            }
        }
    }
    
    public void CheckAllLoopsAgainstBins(ShapeType bin1Shape, ShapeType bin2Shape)
    {
        foreach (LoopSlot slot in loopSlots)
        {
            if (slot.isOccupied)
            {
                if (slot.storedShape == bin1Shape || slot.storedShape == bin2Shape)
                {
                    AutoCollectFromLoop(slot);
                }
            }
        }
    }
    
    void AutoCollectFromLoop(LoopSlot slot)
    {
        Debug.Log("Auto-collecting " + slot.storedShape + " from loop!");
        
        // Play auto-collect effect
        if (autoCollectEffect != null)
        {
            GameObject effect = Instantiate(autoCollectEffect, slot.slotIcon.transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
        
        // Remove from loop and add to bin
        ShapeType collectedShape = slot.RemoveShape();
        
        // Here you would add to appropriate bin
        // binManager.AddToBin(collectedShape);
        
        // Update game manager
        // GameManager.Instance.ShapeCollected(null); // Pass shape reference if needed
    }
    
    public bool CheckLoopsForMatch(ShapeType shapeType)
    {
        // Check if this shape type is in any loop
        foreach (LoopSlot slot in loopSlots)
        {
            if (slot.isOccupied && slot.storedShape == shapeType)
            {
                AutoCollectFromLoop(slot);
                return true;
            }
        }
        return false;
    }
    
    public int GetAvailableLoopCount()
    {
        int count = 0;
        foreach (LoopSlot slot in loopSlots)
        {
            if (!slot.isOccupied)
                count++;
        }
        return count;
    }
}
