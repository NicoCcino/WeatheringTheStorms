using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Makes a UI element follow the mouse cursor with an offset to avoid overlap, 
/// prioritizing directions that keep the element fully on screen.
/// </summary>
public class UIMouseFollower : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How fast the UI element follows the cursor. Higher value = snappier.")]
    [Range(1f, 20f)]
    public float followSpeed = 10f;

    [Tooltip("The padding distance between the cursor and the UI element.")]
    public float cursorPadding = 10f;

    [Header("References")]
    [Tooltip("The Canvas this UI element belongs to. Drag the parent Canvas here.")]
    public Canvas parentCanvas;

    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private Vector2 targetPosition;
    private Camera worldCamera;

    // UI element half-dimensions
    private float halfWidth;
    private float halfHeight;

    // Canvas half-dimensions
    private float canvasHalfWidth;
    private float canvasHalfHeight;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRect = parentCanvas.transform as RectTransform;

        // Initialize camera reference
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            worldCamera = parentCanvas.worldCamera;
        }

        // Calculate constant UI and Canvas half-sizes
        halfWidth = rectTransform.rect.width * 0.5f;
        halfHeight = rectTransform.rect.height * 0.5f;
        canvasHalfWidth = canvasRect.rect.width * 0.5f;
        canvasHalfHeight = canvasRect.rect.height * 0.5f;
    }

    void Update()
    {
        // 1. Get Mouse Position in Local Canvas Space
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector2 mouseLocalPosition;

        bool converted = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mouseScreenPosition,
            worldCamera,
            out mouseLocalPosition
        );

        if (converted)
        {
            // 2. Determine Best Offset Position
            Vector2 desiredPosition = FindBestOffset(mouseLocalPosition);

            // 3. Clamp Position to Stay On Screen
            targetPosition = ClampToScreenBorders(desiredPosition);

            // 4. Smoothly Move the UI Element
            rectTransform.localPosition = Vector3.Lerp(
                rectTransform.localPosition,
                targetPosition,
                Time.deltaTime * followSpeed
            );
        }
    }

    /// <summary>
    /// Calculates the optimal offset position (quadrant) around the cursor.
    /// Prioritizes directions that are furthest from the screen center.
    /// </summary>
    private Vector2 FindBestOffset(Vector2 mousePos)
    {
        // Calculate screen quadrant based on mouse position relative to canvas center (0, 0)
        // If mouse is on the right half of the screen, offset to the left, etc.

        // --- Calculate Offset ---

        // 1. Horizontal Direction: If mouse is in the right half, offset left (Negative X).
        float offsetX = (mousePos.x > 0) ?
            -(halfWidth + cursorPadding) :
            (halfWidth + cursorPadding);

        // 2. Vertical Direction: If mouse is in the upper half, offset down (Negative Y).
        float offsetY = (mousePos.y > 0) ?
            -(halfHeight + cursorPadding) :
            (halfHeight + cursorPadding);

        // --- Check for Off-Screen Conflicts (Refinement) ---

        // Check if an *immediate* switch to the opposite direction is better (e.g., in a corner).

        Vector2 positionCandidate = mousePos + new Vector2(offsetX, offsetY);

        // Get the boundary-safe position for the candidate.
        Vector2 clampedCandidate = ClampToScreenBorders(positionCandidate);

        // If the clamped position is NOT the same as the candidate, it means the candidate 
        // position went off-screen and had to be adjusted.
        if (clampedCandidate != positionCandidate)
        {
            // Try the opposite horizontal direction if the original caused clamping
            if (clampedCandidate.x != positionCandidate.x)
            {
                offsetX *= -1; // Flip X offset
            }

            // Try the opposite vertical direction if the original caused clamping
            if (clampedCandidate.y != positionCandidate.y)
            {
                offsetY *= -1; // Flip Y offset
            }
        }

        // The final position is the mouse position plus the refined offset
        return mousePos + new Vector2(offsetX, offsetY);
    }

    /// <summary>
    /// Adjusts the target position to ensure the RectTransform stays within the Canvas boundaries.
    /// </summary>
    private Vector2 ClampToScreenBorders(Vector2 position)
    {
        // Calculate the maximum allowed boundary limits for the UI element's center (pivot)

        // Horizontal Clamping
        float minX = -canvasHalfWidth + halfWidth;
        float maxX = canvasHalfWidth - halfWidth;

        // Vertical Clamping
        float minY = -canvasHalfHeight + halfHeight;
        float maxY = canvasHalfHeight - halfHeight;

        // Clamp the position to these boundaries
        float clampedX = Mathf.Clamp(position.x, minX, maxX);
        float clampedY = Mathf.Clamp(position.y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }
}