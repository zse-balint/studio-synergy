using UnityEngine;

public class DisplayFullScreenCentered : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Camera mainCamera; // Assign your main camera in the Inspector

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (spriteRenderer != null && mainCamera != null)
        {
            PositionSpriteFullScreenCentered();
        }
        else
        {
            Debug.LogError("SpriteRenderer or Main Camera not found.");
        }
    }

    public void PositionSpriteFullScreenCentered()
    {
        // Get screen height in world units based on orthographicSize
        float screenHeightWorldUnits = mainCamera.orthographicSize * 2f;
        float screenWidthWorldUnits = screenHeightWorldUnits * mainCamera.aspect;

        // Get sprite dimensions in world units
        float spriteWidth = spriteRenderer.bounds.size.x;
        float spriteHeight = spriteRenderer.bounds.size.y;

        // Calculate center position of the screen
        Vector3 screenCenter = mainCamera.transform.position;
        screenCenter.z = 0f;

        // Calculate final position (pivot is assumed to be center)
        transform.position = screenCenter;

        // Scale sprite to fill screen
        ScaleSpriteToFullScreen(screenWidthWorldUnits, screenHeightWorldUnits, spriteWidth, spriteHeight);
    }

    void ScaleSpriteToFullScreen(float screenWidth, float screenHeight, float spriteWidth, float spriteHeight)
    {
        if (spriteWidth > 0 && spriteHeight > 0)
        {
            float scaleX = screenWidth / spriteWidth;
            float scaleY = screenHeight / spriteHeight;
            float finalScale = Mathf.Max(scaleX, scaleY);
            transform.localScale = new Vector3(finalScale, finalScale, 1f);
        }
        else
        {
            Debug.LogError("Sprite dimensions are zero.");
        }
    }

    // Call this if the screen resolution changes
    void OnRectTransformDimensionsChange()
    {
        if (spriteRenderer != null && mainCamera != null)
        {
            PositionSpriteFullScreenCentered();
        }
    }
}