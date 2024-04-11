using UnityEngine;
using UnityEngine.UI;

public class UIMovement : MonoBehaviour
{
    public Image image;
    public float moveSpeed = 100f;
    public float distance = 100f;
    private RectTransform rectTransform;
    private Vector2 startPos;

    enum Direction { Down, Right, Up, Left };
    private Direction currentDirection = Direction.Down;
    private bool movingUp = false;

    private void Start()
    {
        rectTransform = image.GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        // Calculate the distance moved this frame based on moveSpeed
        float distanceThisFrame = moveSpeed * Time.deltaTime;

        // Update the position based on the current direction
        switch (currentDirection)
        {
            case Direction.Down:
                rectTransform.anchoredPosition -= new Vector2(0f, distanceThisFrame);
                if (rectTransform.anchoredPosition.y <= startPos.y - distance)
                {
                    movingUp = false;
                    rectTransform.anchoredPosition = new Vector2(startPos.x, startPos.y - distance);
                    currentDirection = Direction.Right;
                }
                break;
            case Direction.Right:
                rectTransform.anchoredPosition += new Vector2(distanceThisFrame, 0f);
                if (rectTransform.anchoredPosition.x >= startPos.x + distance)
                {
                    rectTransform.anchoredPosition = new Vector2(startPos.x + distance, startPos.y);
                    currentDirection = Direction.Up;
                }
                break;
            case Direction.Up:
                if (!movingUp)
                {
                    rectTransform.anchoredPosition += new Vector2(0f, distanceThisFrame);
                    if (rectTransform.anchoredPosition.y >= startPos.y + distance)
                    {
                        rectTransform.anchoredPosition = new Vector2(startPos.x + distance, startPos.y + distance);
                        currentDirection = Direction.Left;
                        movingUp = true;
                    }
                }
                else
                {
                    if (rectTransform.anchoredPosition.y >= startPos.y)
                    {
                        rectTransform.anchoredPosition = new Vector2(startPos.x + distance, startPos.y);
                        currentDirection = Direction.Left;
                        movingUp = false;
                    }
                }
                break;
            case Direction.Left:
                rectTransform.anchoredPosition -= new Vector2(distanceThisFrame, 0f);
                if (rectTransform.anchoredPosition.x <= startPos.x)
                {
                    rectTransform.anchoredPosition = startPos;
                    currentDirection = Direction.Down;
                }
                break;
        }
    }
}