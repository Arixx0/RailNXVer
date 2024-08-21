using UnityEngine;

namespace Environments
{
    public class EnvironmentEntityManager : MonoBehaviour
    {
        [Header("Obstacles Properties")]
        [SerializeField] private GameObject[] obstacleEntityPrefabs;
        [SerializeField, Range(0, 1)] private float obstacleNoiseThreshold = 0.95f;
        [SerializeField] private float obstacleSpawnDensity = 0.2f;

        [Header("Foliage Properties")]
        [SerializeField] private GameObject[] foliageEntityPrefabs;
        [SerializeField, Range(0, 1)] private float foliageNoiseThreshold = 0.75f;
        [SerializeField] private float foliageSpawnDensity = 0.6f;

        [Space]
        [SerializeField] private float noiseSamplingScale;
        [SerializeField] private int noiseResolution;
        [SerializeField] private float noiseOffsetX;
        [SerializeField] private float noiseOffsetY;
        
        [Space]
        [SerializeField] private MapSector parentSector;
        [SerializeField] private LayerMask conformingLayerMask;
        [SerializeField] private float conformingCheckDistance = 10f;

        public Bounds SpawnBounds => parentSector.SpawnMap.Bounds;
        
        public Texture2D NoiseMapTex2D { get; private set; }

        // public void ManipulateNoiseMap()
        // {
        //     var pixel = new Color[noiseResolution * noiseResolution];
        //
        //     NoiseMapTex2D = new Texture2D(noiseResolution, noiseResolution);
        //
        //     for (var y = 0f; y < NoiseMapTex2D.height; y++)
        //     {
        //         for (var x = 0f; x < NoiseMapTex2D.width; x++)
        //         {
        //             var xCoord = noiseOffsetX + x / NoiseMapTex2D.width * noiseSamplingScale;
        //             var yCoord = noiseOffsetY + y / NoiseMapTex2D.height * noiseSamplingScale;
        //             var sample = Mathf.PerlinNoise(xCoord, yCoord);
        //             pixel[(int)y * NoiseMapTex2D.width + (int)x] = new Color(sample, sample, sample);
        //         }
        //     }
        //     
        //     NoiseMapTex2D.SetPixels(pixel);
        //     NoiseMapTex2D.Apply();
        // }

        // public void ManipulateSpawnGrid()
        // {
        //     var gridXCount = Mathf.CeilToInt(spawnBounds.size.x / gridCellSize);
        //     var gridZCount = Mathf.CeilToInt(spawnBounds.size.z / gridCellSize);
        //     
        //     SpawnGrid = new GridCellData[gridXCount * gridZCount];
        //     for (var z = 0; z < gridZCount; z++)
        //     {
        //         for (var x = 0; x < gridXCount; x++)
        //         {
        //             var spawnPos = new Vector3(
        //                 spawnBounds.min.x + x * gridCellSize,
        //                 spawnBounds.min.y,
        //                 spawnBounds.min.z + z * gridCellSize
        //             );
        //
        //             var isValid = !parentSector.Path.IsCollidedFromPoint(
        //                 transform.TransformPoint(spawnPos));
        //             
        //             var typeId = isValid ? 1 : 0;
        //             if (isValid)
        //             {
        //                 var xDelta = Mathf.InverseLerp(0, gridXCount, x) * noiseResolution;
        //                 var zDelta = Mathf.InverseLerp(0, gridZCount, z) * noiseResolution;
        //                 
        //                 var noiseSample = NoiseMapTex2D.GetPixel((int)xDelta, (int)zDelta).r;
        //                 if (noiseSample > obstacleNoiseThreshold) { typeId = 2; }
        //                 else if (noiseSample > foliageNoiseThreshold) { typeId = 3; }
        //             }
        //
        //             SpawnGrid[z * gridXCount + x] = new GridCellData()
        //             {
        //                 entityTypeId = 0,
        //                 position = spawnPos
        //             };
        //         }
        //     }
        // }

        public void SpawnEntities()
        {
            var spawnBounds = parentSector.SpawnMap.Bounds;
            var cellSize = parentSector.SpawnMap.CellSize;
            
            var obstacleInstanceCount = spawnBounds.size.x / cellSize * obstacleSpawnDensity;
            for (var i = 0; i < obstacleInstanceCount; i++)
            {
                var randomPos = new Vector3(
                    UnityEngine.Random.Range(spawnBounds.min.x, spawnBounds.max.x),
                    spawnBounds.max.y,
                    UnityEngine.Random.Range(spawnBounds.min.z, spawnBounds.max.z));

                var cellData = parentSector.SpawnMap[randomPos.x, randomPos.z];
                if (cellData.isOccupied)
                {
                    i--;
                    
                    // mark sector as occupied because it's collided with rail path.
                    cellData.isOccupied = true;
                    parentSector.SpawnMap[randomPos.x, randomPos.z] = cellData;
                    
                    continue;
                }

                cellData.isOccupied = true;
                parentSector.SpawnMap[randomPos.x, randomPos.z] = cellData;

                randomPos = transform.TransformPoint(randomPos);
                var entity = obstacleEntityPrefabs[UnityEngine.Random.Range(0, obstacleEntityPrefabs.Length)];
                var ray = new Ray(randomPos, Vector3.down);
                if (Physics.Raycast(ray, out var hitInfo, conformingCheckDistance, conformingLayerMask, QueryTriggerInteraction.Ignore))
                {
                    randomPos = hitInfo.point + Vector3.up * (entity.transform.lossyScale.y * 0.5f);
                }

                var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
                Instantiate(entity, randomPos, rotation, transform);
            }
            
            var foliageInstanceCount = spawnBounds.size.x / cellSize * foliageSpawnDensity;
            for (var i = 0; i < foliageInstanceCount; i++)
            {
                var randomPos = new Vector3(
                    UnityEngine.Random.Range(spawnBounds.min.x, spawnBounds.max.x),
                    spawnBounds.max.y,
                    UnityEngine.Random.Range(spawnBounds.min.z, spawnBounds.max.z));

                var cellData = parentSector.SpawnMap[randomPos.x, randomPos.z];
                if (cellData.isOccupied)
                {
                    i--;
                    
                    // mark sector as occupied because it's collided with rail path.
                    cellData.isOccupied = true;
                    parentSector.SpawnMap[randomPos.x, randomPos.z] = cellData;

                    continue;
                }
                
                cellData.isOccupied = true;
                parentSector.SpawnMap[randomPos.x, randomPos.z] = cellData;

                randomPos = transform.TransformPoint(randomPos);
                var entity = foliageEntityPrefabs[UnityEngine.Random.Range(0, foliageEntityPrefabs.Length)];
                var ray = new Ray(randomPos, Vector3.down);
                if (Physics.Raycast(ray, out var hitInfo, conformingCheckDistance, conformingLayerMask, QueryTriggerInteraction.Ignore))
                {
                    randomPos = hitInfo.point + Vector3.up * entity.transform.lossyScale.y * 0.5f;
                }

                var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
                Instantiate(entity, randomPos, rotation, transform);
            }
        }
    }
}