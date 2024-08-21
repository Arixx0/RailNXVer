using UnityEngine;

namespace Units
{
    public class FloatingUnitMovement : UnitMovement
    {
        [SerializeField] private float hoveringHeight = 1f;

        protected override void CalculateMoveAmount(Vector3 displacement, ref Vector3 moveAmount, ref Quaternion rotator)
        {
            base.CalculateMoveAmount(displacement, ref moveAmount, ref rotator);
            
            // handle hovering velocity
            var destHeight = hoveringHeight;
            var ray = new Ray(cachedTransform.position, Vector3.down);
            if (Physics.Raycast(ray, out var hitInfo, 1000f, collisionMask, QueryTriggerInteraction.Ignore))
            {
                destHeight = hitInfo.point.y + hoveringHeight;
            }

            var targetVelocity = destHeight - cachedTransform.position.y;
            if (Mathf.Abs(targetVelocity) < 0.1f)
            {
                targetVelocity = 0f;
            }
            
            moveAmount.y = targetVelocity;
        }
    }
}