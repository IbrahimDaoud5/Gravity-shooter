using UnityEngine;

public class PlayerAreaDetection1 : MonoBehaviour
{
    public BoxCollider2D playerCollider;
    public AreaEffector2D areaEffector;
    public RectTransform windArrowRectTransform;
    public float movementSpeed = 5f;

    private bool isInsideArea = false;

    void Update()
    {
        // Check if the player is inside the AreaEffector2D
        bool wasInsideArea = isInsideArea;
        isInsideArea = IsPlayerInArea();

        if (isInsideArea)
        {
            // If the player just entered the area, move the WindArrow
            if (!wasInsideArea)
            {
                MoveWindArrow();
            }
        }
    }

    bool IsPlayerInArea()
    {
        // Check if the player's collider is within the manually calculated bounds of the area effector
        Bounds effectorBounds = CalculateEffectorBounds();
        Debug.Log(effectorBounds);
        return effectorBounds.Intersects(playerCollider.bounds);
    }

    void MoveWindArrow()
    {
        // Convert the area effector's position to screen space
        Vector3 effectorScreenPos = Camera.main.WorldToScreenPoint(areaEffector.transform.position);

        // Set the position of the WindArrow to the screen position of the area effector
        windArrowRectTransform.position = effectorScreenPos;
    }

    Bounds CalculateEffectorBounds()
    {
        Vector2 effectorSize = areaEffector.forceMagnitude * new Vector2(Mathf.Abs(Mathf.Cos(areaEffector.forceAngle)), Mathf.Abs(Mathf.Sin(areaEffector.forceAngle)));
        return new Bounds(areaEffector.transform.position, effectorSize);
    }
}
