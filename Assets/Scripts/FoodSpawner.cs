using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab;

    private GameObject currentFood;
    public bool HasFood => currentFood != null;
    public Vector3 CurrentFoodCell => currentFood != null ? currentFood.transform.position : Vector3.zero;

    public void SpawnNewFood(Vector2Int min, Vector2Int max, HashSet<Vector2Int> occupied)
    {
        // если была еда — удалим
        if (currentFood != null) Destroy(currentFood);

        // до 200 попыток подобрать пустую клетку
        for (int tries = 0; tries < 200; tries++)
        {
            int x = Random.Range(min.x, max.x + 1);
            int y = Random.Range(min.y, max.y + 1);
            Vector2Int cell = new(x, y);

            if (occupied == null || !occupied.Contains(cell))
            {
                currentFood = Instantiate(foodPrefab, new Vector3(x, y, 0f), Quaternion.identity);
                return;
            }
        }

        Debug.LogWarning("FoodSpawner: не нашёл свободную клетку.");
    }
}