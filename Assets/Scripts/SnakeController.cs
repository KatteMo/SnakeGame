using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [Header("Grid & Speed")]
    public float moveStepTime = 0.18f;         // период "тика"
    public Vector2Int gridMin = new(-8, -5);   // границы поля
    public Vector2Int gridMax = new( 8,  5);

    [Header("Snake")]
    public int startSize = 3;
    public Transform segmentPrefab;

    [Header("Refs")]
    public FoodSpawner foodSpawner;

    private float timer;
    private Vector2Int dir = Vector2Int.right;
    private readonly List<Transform> segments = new();

    void Start()
    {
        segments.Clear();
        segments.Add(transform);

        for (int i = 1; i < startSize; i++)
            Grow();

        // первая еда
        RespawnFoodSafe();
    }

    void Update()
    {
        // управление (без разворота на 180)
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))    && dir != Vector2Int.down)  dir = Vector2Int.up;
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))  && dir != Vector2Int.up)    dir = Vector2Int.down;
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))  && dir != Vector2Int.right) dir = Vector2Int.left;
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && dir != Vector2Int.left)  dir = Vector2Int.right;

        timer += Time.deltaTime;
        if (timer >= moveStepTime)
        {
            timer = 0f;
            Step();
        }
    }

    void Step()
    {
        // сдвинуть тело
        for (int i = segments.Count - 1; i > 0; i--)
            segments[i].position = segments[i - 1].position;

        // новая позиция головы (в клетках)
        Vector3 head = segments[0].position;
        head.x += dir.x;
        head.y += dir.y;
        segments[0].position = head;

        // выход за границы = Game Over
        Vector2Int h = Vector2Int.RoundToInt(segments[0].position);
        if (h.x < gridMin.x || h.x > gridMax.x || h.y < gridMin.y || h.y > gridMax.y)
        {
            GameManager.Instance.GameOver();
            return;
        }

        // самоукус
        for (int i = 1; i < segments.Count; i++)
        {
            if (Vector2Int.RoundToInt(segments[i].position) == h)
            {
                GameManager.Instance.GameOver();
                return;
            }
        }

        // съели еду?
        if (foodSpawner != null && foodSpawner.HasFood &&
            Vector2Int.RoundToInt(foodSpawner.CurrentFoodCell) == h)
        {
            Grow();
            GameManager.Instance.AddScore(1);
            RespawnFoodSafe();
        }
    }

    public void Grow()
    {
        Transform tail = segments[segments.Count - 1];
        Transform seg = Instantiate(segmentPrefab, tail.position, Quaternion.identity);
        segments.Add(seg);
    }

    void RespawnFoodSafe()
    {
        // подготовим занятые клетки змейкой
        HashSet<Vector2Int> occupied = new();
        foreach (var t in segments)
            occupied.Add(Vector2Int.RoundToInt(t.position));

        foodSpawner.SpawnNewFood(gridMin, gridMax, occupied);
    }

    // утилита для спавнера, если потребуется
    public IEnumerable<Vector2Int> OccupiedCells()
    {
        foreach (var t in segments)
            yield return Vector2Int.RoundToInt(t.position);
    }
}