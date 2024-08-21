using UnityEngine;

namespace Environments
{
    [System.Serializable]
    public class MapSectorSpawnMap
    {
        [SerializeField] private CellData[] cells;
        [SerializeField] private Bounds bounds = new(Vector3.zero, new Vector3(120, 5, 120));
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private int width;
        [SerializeField] private int depth;

        public Bounds Bounds => bounds;

        public float CellSize => cellSize;

        public int Length => width * depth;

        public CellData this[int index]
        {
            get
            {
                if (width == 0 || depth == 0 || cells == null || cells.Length == 0)
                {
                    Debug.LogAssertion("Map sector spawn map is not initialized.");
                    return default;
                }

                if (index < 0 || index >= Length)
                {
                    Debug.LogAssertion("Index out of bounds.");
                    return default;
                }
                
                return cells[index];
            }
            set
            {
                if (width == 0 || depth == 0 || cells == null || cells.Length == 0)
                {
                    Debug.LogAssertion("Map sector spawn map is not initialized.");
                    return;
                }

                if (index < 0 || index >= Length)
                {
                    Debug.LogAssertion("Index out of bounds.");
                    return;
                }
                
                cells[index] = value;
            }
        }

        public CellData this[int x, int z]
        {
            get 
            {
                if (width == 0 || depth == 0 || cells == null || cells.Length == 0)
                {
                    Debug.LogAssertion("Map sector spawn map is not initialized.");
                    return default;
                }
                
                if (x < 0 || x >= width || z < 0 || z >= depth)
                {
                    Debug.LogAssertion("Index out of bounds.");
                    return default;
                }
                
                return cells [z * width + x];
            }
            set
            {
                if (width == 0 || depth == 0 || cells == null || cells.Length == 0)
                {
                    Debug.LogAssertion("MapSectorSpawnMap is not initialized.");
                    return;
                }
                
                if (x < 0 || x >= width || z < 0 || z >= depth)
                {
                    Debug.LogAssertion("Index out of bounds.");
                    return;
                }
                
                cells[z * width + x] = value;
            }
        }

        public CellData this[float x, float z]
        {
            get
            {
                var xIndex = x + bounds.extents.x / cellSize;
                var zIndex = z + bounds.extents.z / cellSize;
                return this[Mathf.FloorToInt(xIndex), Mathf.FloorToInt(zIndex)];
            }
            set
            {
                var xIndex = x + bounds.extents.x / cellSize;
                var zIndex = z + bounds.extents.z / cellSize;
                this[Mathf.FloorToInt(xIndex), Mathf.FloorToInt(zIndex)] = value;
            }
        }
        
#if UNITY_EDITOR
        public void Manipulate()
        {
            width = Mathf.CeilToInt(bounds.size.x / cellSize);
            depth = Mathf.CeilToInt(bounds.size.z / cellSize);

            cells = new CellData[width * depth];
            for (var z = 0; z < depth; z++)
            {
                for (var x = 0; x < width; x++)
                {
                    var cellPos = new Vector3(
                        bounds.min.x + x * cellSize,
                        bounds.min.y,
                        bounds.min.z + z * cellSize
                    );
                    
                    cells[z * width + x] = new CellData
                    {
                        position = cellPos,
                        isOccupied = false
                    };
                }
            }
        }
#endif

        public void Reset()
        {
            bounds.center = Vector3.zero;
            bounds.size = new Vector3(120, 5, 120);
            cellSize = 1f;
            
            #if UNITY_EDITOR
            Manipulate();
            #endif
        }
        
        [System.Serializable]
        public struct CellData
        {
            public Vector3 position; // Cell's position in local space.
            public bool isOccupied;
        }
    }
}