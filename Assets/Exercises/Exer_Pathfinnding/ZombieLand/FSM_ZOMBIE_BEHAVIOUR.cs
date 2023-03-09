using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_ZOMBIE_BEHAVIOUR", menuName = "Finite State Machines/FSM_ZOMBIE_BEHAVIOUR", order = 1)]
public class FSM_ZOMBIE_BEHAVIOUR : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    SteeringContext s_context;
    ZOMBIE_BLACKBOARD blackboard;
    PathFeeder pathfeeder;
    GameObject theGut;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        s_context = GetComponent<SteeringContext>();
        blackboard = GetComponent<ZOMBIE_BLACKBOARD>();
        pathfeeder = GetComponent<PathFeeder>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/
         
        State GoingWaypoint = new State("GoingWaypoint",
            () => { pathfeeder.target = blackboard.GetRandomWanderPoint(); pathfeeder.enabled = true; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { pathfeeder.enabled = false; }  // write on exit logic inisde {}  
        );

        State GoingToGut = new State("GoingToGut",
           () => { pathfeeder.target = theGut; pathfeeder.enabled = true; }, // write on enter logic inside {}
           () => { }, // write in state logic inside {}
           () => { pathfeeder.enabled = false; }  // write on exit logic inisde {}  
       );

        State TransportingGut = new State("TransportingGut",
           () => {
               theGut.transform.parent = gameObject.transform;
               pathfeeder.target = blackboard.GetRandomCollectionPoint(); 
               pathfeeder.enabled = true;
           }, // write on enter logic inside {}
           () => { }, // write in state logic inside {}
           () => { 
               pathfeeder.enabled = false;
               theGut.transform.parent = null;
               theGut.tag = "Untagged";
           }  // write on exit logic inisde {}  
       );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition WaypointDetected = new Transition("WaypointDetected",
            () => { return SensingUtils.DistanceToTarget(gameObject, pathfeeder.target) < blackboard.pointReachedRadius; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition GutDetected = new Transition("GutDetected",
            () => { 
                theGut = SensingUtils.FindInstanceWithinRadius(gameObject, "FREE_GUTS", blackboard.gutDetectedRadius);
                return SensingUtils.DistanceToTarget(gameObject, theGut) <= blackboard.gutDetectedRadius;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition GutReached = new Transition("GutReached",
            () => { return SensingUtils.DistanceToTarget(gameObject, theGut) < blackboard.gutReachedRadius; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition GutTransported = new Transition("GutTransported",
            () => { return SensingUtils.DistanceToTarget(gameObject, pathfeeder.target) < blackboard.pointReachedRadius; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */

        AddStates(GoingWaypoint, GoingToGut, TransportingGut);

        AddTransition(GoingWaypoint, WaypointDetected, GoingWaypoint);
        AddTransition(GoingWaypoint, GutDetected, GoingToGut);
        AddTransition(GoingToGut, GutReached, TransportingGut);
        AddTransition(TransportingGut, GutTransported, GoingWaypoint);


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */

        initialState = GoingWaypoint;

    }
}
