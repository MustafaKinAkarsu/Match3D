using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject quad; // Reference to your quad
    public int rows = 5;
    public int columns = 5;
    private Vector3 startPosition;
    [SerializeField] GameObject[] gemPrefabs;
    int previousRandom;

    void Start()
    {
        startPosition = quad.GetComponent<Renderer>().bounds.min;
        CreateGrid();
    }

    void CreateGrid()
    {
        
        Vector3 quadSize = quad.GetComponent<Renderer>().bounds.size;
        Vector3 cellSize = new Vector3(quadSize.x / columns, quadSize.y / rows, 1f);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {

                 int random;
                 do
                 {
                     random = Random.Range(0, 4);
                 }
                 while (random == previousRandom);
                previousRandom = random;
                float xPos = (col * cellSize.x + cellSize.x / 2) + startPosition.x;
                float yPos = (row * cellSize.y + cellSize.y / 2) + startPosition.y;
                Vector3 position = new Vector3(xPos, yPos, 0f);
                GameObject cell = Instantiate(gemPrefabs[random], position, Quaternion.identity);
                cell.transform.parent = transform;
            }
        }
    }

}