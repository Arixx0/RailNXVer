using Units;
using Units.Stats;
using UnityEngine;

namespace TrainScripts
{
    public partial class Train
    {
        [Header("Train Stats")]
        [SerializeField] private float totalTrainLength;
        [SerializeField] private float validTrainLength;

        [SerializeField] private float enemyAreaTopLeftX = 15;
        [SerializeField] private float enemyAreaTopLeftZ = -40;
        [SerializeField] private float enemyAreaBottomRightX = 5;
        [SerializeField] private float enemyAreaBottomRightZ = 15;

        private void UpdateTrainStats()
        {
            totalTrainLength = Mathf.Max(cars.Count - 1, 0) * gapBetweenCars;
            foreach (var car in cars)
            {
                totalTrainLength += car.UnitLength;
            }

            if (cars.Count > 1)
            {
                validTrainLength = totalTrainLength - cars[^1].UnitLengthHalf;

                if (regardTrainAsAlignTarget)
                {
                    validTrainLength -= cars[0].UnitLengthHalf;
                }
            }
            else
            {
                validTrainLength = 0;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            DrawRectangle();
        }

        private void DrawRectangle()
        {
            Vector3 topLeft = new Vector3(transform.position.x + enemyAreaTopLeftX, 1f, transform.position.z + enemyAreaTopLeftZ);
            Vector3 topRight = new Vector3(transform.position.x + enemyAreaBottomRightX, 1f, transform.position.z + enemyAreaTopLeftZ);
            Vector3 bottomRight = new Vector3(transform.position.x + enemyAreaBottomRightX, 1f, transform.position.z + enemyAreaBottomRightZ);
            Vector3 bottomLeft = new Vector3(transform.position.x + enemyAreaTopLeftX, 1f, transform.position.z + enemyAreaBottomRightZ);

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);

            topLeft = new Vector3(transform.position.x - enemyAreaTopLeftX, 1f, transform.position.z + enemyAreaTopLeftZ);
            topRight = new Vector3(transform.position.x - enemyAreaBottomRightX, 1f, transform.position.z + enemyAreaTopLeftZ);
            bottomRight = new Vector3(transform.position.x - enemyAreaBottomRightX, 1f, transform.position.z + enemyAreaBottomRightZ);
            bottomLeft = new Vector3(transform.position.x - enemyAreaTopLeftX, 1f, transform.position.z + enemyAreaBottomRightZ);

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }

        public Vector3 EnemyAreaPosition(Units.Enemies.EnemyAreaComponent enemyAreaComponent)
        {
            float minX = Mathf.Min(transform.position.x + enemyAreaTopLeftX, transform.position.x + enemyAreaBottomRightX);
            float maxX = 0;
            float minZ = 0;
            float maxZ = 0;
            if (enemyAreaComponent.TryGetComponent(out UnitStatComponent statComponent))
            {
                maxX = transform.position.x + statComponent.AttackRange;
            }
            if (enemyAreaComponent.EnemyAttackType == 0)
            {
                minZ = Mathf.Min(transform.position.z + enemyAreaTopLeftZ, transform.position.z + enemyAreaBottomRightZ);
                maxZ = Mathf.Max(transform.position.z + enemyAreaTopLeftZ, transform.position.z + enemyAreaBottomRightZ);
            }
            else if (enemyAreaComponent.CurrentTarget.TryGetComponent(out Collider collider))
            {
                minZ = (collider.bounds.center - (collider.bounds.size / 2.0f)).z;
                maxZ = (collider.bounds.center + (collider.bounds.size / 2.0f)).z;
            }

            float x = Random.Range(minX * enemyAreaComponent.DirectionMultiplier, maxX * enemyAreaComponent.DirectionMultiplier);
            float z = Random.Range(minZ, maxZ);

            return new Vector3(x, 1f, z);
        }
    }
}