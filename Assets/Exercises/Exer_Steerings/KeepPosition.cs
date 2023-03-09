using UnityEngine;

namespace Steerings
{

    public class KeepPosition : SteeringBehaviour
    {

        public GameObject target;
        public float requiredDistance;
        public float requiredAngle;

        /* COMPLETE */

        public override GameObject GetTarget()
        {
            return target;
        }

        public override Vector3 GetLinearAcceleration()
        {
            /* COMPLETE */
            return KeepPosition.GetLinearAcceleration(Context, target, requiredDistance, requiredAngle);

            //return Vector3.zero; // remove this line when exercise completed
        }

        
        public static Vector3 GetLinearAcceleration (SteeringContext me, GameObject target,
                                                     float distance, float angle)
        {
            /* COMPLETE */

            float DesiredAngle = target.transform.rotation.eulerAngles.z + angle;

            SURROGATE_TARGET.transform.position = target.transform.position + (Utils.OrientationToVector(DesiredAngle) * distance);

            return Arrive.GetLinearAcceleration(me, SURROGATE_TARGET);
            //return Vector3.zero; // remove this line when exercise completed
        }

    }
}