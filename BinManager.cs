using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BinManager : MonoBehaviour
{
    [System.Serializable]
    public class Bin
    {
        public string binName;
        public ShapeType currentShape;
        public int currentCount = 0;
        public int maxCount = 3;
        public Image binIcon;
        public TextMeshProUGUI countText;
        public GameObject fillEffect;
        
        // The sequence of shapes this bin will cycle through
        public List<ShapeType> shapeSequence;
        private int sequenceIndex = 0;
        
        public void Initialize()
        {
            if (shapeSequence.Count > 0)
            {
                currentShape = shapeSequence[0];
                UpdateBinDisplay();
            }
        }
        
        public bool AddShape(Shape shape)
        {
            if (shape.shapeType == currentShape)
            {
                currentCount++;
                UpdateBinDisplay();
                
                // Play fill effect
                if (fillEffect != null)
                    GameObject.Instantiate(fillEffect, binIcon.transform.position, Quaternion.identity);
                
                // Check if bin is full
                if (currentCount >= maxCount)
                {
                    RotateToNextShape();
                }
                
                return true;
            }
            return false;
        }
        
        void RotateToNextShape()
        {
            sequenceIndex++;
            if (sequenceIndex >= shapeSequence.Count)
                sequenceIndex = 0; // Loop back to start
                
            currentShape = shapeSequence[sequenceIndex];
            currentCount = 0;
            UpdateBinDisplay();
            
            Debug.Log(binName + " now collecting: " + currentShape);
        }
        
        void UpdateBinDisplay()
        {
            if (binIcon != null)
            {
                // Update icon based on shape (you'd need shape sprites)
                // binIcon.sprite = GetShapeSprite(currentShape);
            }
            
            if (countText != null)
            {
                countText.text = currentCount + "/" + maxCount;
            }
        }
        
        // Get the sprite for shape (implement this with your assets)
        Sprite GetShapeSprite(ShapeType shape)
        {
            // Return appropriate sprite based on shape
            return null;
        }
    }
    
    [Header("Bins")]
    public Bin bin1;
    public Bin bin2;
    public LoopManager loopManager;
    
    void Start()
    {
        // Initialize bins
        bin1.Initialize();
        bin2.Initialize();
    }
    
    public bool TryCollectShape(Shape shape)
    {
        bool collected = false;
        
        // Try bin1 first
        if (shape.shapeType == bin1.currentShape && bin1.currentCount < bin1.maxCount)
        {
            collected = bin1.AddShape(shape);
        }
        // Then try bin2
        else if (shape.shapeType == bin2.currentShape && bin2.currentCount < bin2.maxCount)
        {
            collected = bin2.AddShape(shape);
        }
        // Check if shape is in loops
        else if (loopManager != null)
        {
            collected = loopManager.CheckLoopsForMatch(shape.shapeType);
        }
        
        if (collected)
        {
            // Check if any loops auto-collect after bin change
            CheckLoopsAfterBinChange();
        }
        
        return collected;
    }
    
    void CheckLoopsAfterBinChange()
    {
        // After bins change, check if any stored shapes match
        if (loopManager != null)
        {
            loopManager.CheckAllLoopsAgainstBins(bin1.currentShape, bin2.currentShape);
        }
    }
    
    // Public method to get current bin shapes
    public ShapeType GetBin1Shape()
    {
        return bin1.currentShape;
    }
    
    public ShapeType GetBin2Shape()
    {
        return bin2.currentShape;
    }
}
