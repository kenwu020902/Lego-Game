using UnityEngine;
using System.Collections;

public class Shape : MonoBehaviour
{
    [Header("Shape Properties")]
    public ShapeType shapeType;
    public int layerNumber = 1; // 1 = outer, 2 = middle, 3 = inner
    public GameObject innerShape; // The shape underneath (assign in inspector)
    
    [Header("Materials")]
    public Material triangleMaterial;
    public Material squareMaterial;
    public Material rectangleMaterial;
    public Material roundMaterial;
    public Material starMaterial;
    public Material hexagonMaterial;
    
    [Header("Peek Settings")]
    public float longPressDuration = 1.0f;
    private bool isLongPress = false;
    private bool isPeeking = false;
    private Material originalMaterial;
    private Renderer shapeRenderer;

    void Start()
    {
        shapeRenderer = GetComponent<Renderer>();
        if (shapeRenderer != null)
        {
            originalMaterial = shapeRenderer.material;
            ApplyShapeMaterial();
        }
    }

    void ApplyShapeMaterial()
    {
        // Apply different colors/materials based on shape type
        switch (shapeType)
        {
            case ShapeType.Triangle:
                shapeRenderer.material = triangleMaterial;
                break;
            case ShapeType.Square:
                shapeRenderer.material = squareMaterial;
                break;
            case ShapeType.Rectangle:
                shapeRenderer.material = rectangleMaterial;
                break;
            case ShapeType.Round:
                shapeRenderer.material = roundMaterial;
                break;
            case ShapeType.Star:
                shapeRenderer.material = starMaterial;
                break;
            case ShapeType.Hexagon:
                shapeRenderer.material = hexagonMaterial;
                break;
        }
    }

    void OnMouseDown()
    {
        // Start long press timer
        StartCoroutine(LongPressCheck());
    }

    void OnMouseUp()
    {
        // If it wasn't a long press, treat as normal click
        if (!isLongPress && !isPeeking)
        {
            StopAllCoroutines();
            TryCollect();
        }
        else
        {
            StopAllCoroutines();
            isLongPress = false;
            isPeeking = false;
            // Restore original material
            if (shapeRenderer != null)
                ApplyShapeMaterial();
        }
    }

    IEnumerator LongPressCheck()
    {
        yield return new WaitForSeconds(longPressDuration);
        
        // Long press detected - peek at inner shape
        isLongPress = true;
        PeekAtInnerShape();
    }

    void PeekAtInnerShape()
    {
        if (innerShape != null)
        {
            isPeeking = true;
            
            // Make this shape semi-transparent to peek inside
            Color semiTransparent = shapeRenderer.material.color;
            semiTransparent.a = 0.3f;
            shapeRenderer.material.color = semiTransparent;
            
            // Get the inner shape's component
            Shape innerShapeComponent = innerShape.GetComponent<Shape>();
            if (innerShapeComponent != null)
            {
                Debug.Log("Peeking at: " + innerShapeComponent.shapeType);
                // Here you would show the inner shape (maybe highlight it)
            }
        }
    }

    void TryCollect()
    {
        // Check if game is active
        if (!GameManager.Instance.IsGameActive())
            return;

        // Check if this shape matches current bins
        if (binManager != null)
        {
            bool collected = binManager.TryCollectShape(this);
            
            if (collected)
            {
                // Play collection animation
                StartCoroutine(CollectAnimation());
            }
            else
            {
                // No match - suggest using extra loop
                Debug.Log("No matching bin! Consider using extra loop");
                // Flash red to indicate no match
                StartCoroutine(FlashColor(Color.red));
            }
        }
    }

    IEnumerator CollectAnimation()
    {
        // Shrink and fly to bin animation
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 0.5f;
        
        float duration = 0.3f;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Notify GameManager
        GameManager.Instance.ShapeCollected(this);
        
        // Destroy this shape
        Destroy(gameObject);
    }

    IEnumerator FlashColor(Color color)
    {
        shapeRenderer.material.color = color;
        yield return new WaitForSeconds(0.2f);
        ApplyShapeMaterial();
    }

    // Public method to use extra loop on this shape
    public void UseExtraLoop()
    {
        if (loopManager != null)
        {
            bool loopUsed = loopManager.AddToLoop(this);
            
            if (loopUsed)
            {
                // Move shape to loop (don't destroy yet)
                gameObject.SetActive(false); // Hide the shape
                // Reveal inner shape
                if (innerShape != null)
                    innerShape.SetActive(true);
            }
        }
    }
}
