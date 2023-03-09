using UnityEngine;

namespace Steerings
{

    public class Interfere : SteeringBehaviour
    {
        public GameObject target;
        public float requiredDistance;
        /*
        // remove comments for steerings that must be provided with a target 
        // remove whole block if no explicit target required
        // (if FT or FTI policies make sense, then this method must be present)
        public GameObject target;*/

        public override GameObject GetTarget()
        {
            return target;
        }
        
        
        public override Vector3 GetLinearAcceleration()
        {
            return Interfere.GetLinearAcceleration(Context, target, requiredDistance /* add extra parameters (target?) if required */);
        }

        
        public static Vector3 GetLinearAcceleration (SteeringContext me, GameObject target, float requiredDistance /* add extra parameters (target?) if required */)
        {
            /* COMPLETE this method. It must return the linear acceleration (Vector3) */

            SteeringContext targetContext = target.GetComponent<SteeringContext>();
            if (targetContext == null)
            {
                Debug.LogWarning("Velocity Matching invoked with a target " +
                                  "that has no context attached. Zero acceleration returned");
                return Vector3.zero;
            }

            
            Vector3 displacement = targetContext.velocity.normalized * requiredDistance;
            SURROGATE_TARGET.transform.position = target.transform.position + displacement;

            return Arrive.GetLinearAcceleration(me, SURROGATE_TARGET);
        }

    }
}