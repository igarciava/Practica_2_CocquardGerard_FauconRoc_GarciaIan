using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_SeedCollecting", menuName = "Finite State Machines/FSM_SeedCollecting", order = 1)]
public class FSM_SeedCollecting : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private ANT_Blackboard blackboard;
    private Arrive arrive;
    private GameObject seed;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<ANT_Blackboard>();
        arrive = GetComponent<Arrive>();
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
        FiniteStateMachine FSMTwoPoint;
        FSMTwoPoint = ScriptableObject.CreateInstance<FSM_TwoPointWandering>();

        State GoingToSeed = new State("GoingToSeed",
            () => { arrive.target = seed; arrive.enabled = true; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { arrive.enabled = false; }  // write on exit logic inisde {}  
        );

        State TransportingSeed = new State("TransportingSeed",
            () => {
                seed.tag = "NO_SEED";
                seed.transform.SetParent(gameObject.transform);
                arrive.target = blackboard.nest;
                arrive.enabled = true;
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {} 
            () => { 
                arrive.enabled = false;
                seed.transform.SetParent(null);
            }  // write on exit logic inisde {}  
        );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition NearbySeedDetected = new Transition("NearbySeedDetected",
            () => { seed = SensingUtils.FindInstanceWithinRadius(gameObject, "SEED", blackboard.seedDetectionRadius); return seed != null; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition SeedReached = new Transition("SeedReached",
            () => { return SensingUtils.DistanceToTarget(gameObject, seed) < blackboard.seedReachedRadius; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition NestReached = new Transition("NestReached",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.nest) < blackboard.nestReachedRadius; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition SeedTaken = new Transition("SeedTaken",
            () => { return seed.tag != "SEED"; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(FSMTwoPoint, GoingToSeed, TransportingSeed);

        AddTransition(FSMTwoPoint, NearbySeedDetected, GoingToSeed);
        AddTransition(GoingToSeed, SeedTaken, FSMTwoPoint);
        AddTransition(GoingToSeed, SeedReached, TransportingSeed);
        AddTransition(TransportingSeed, NestReached, FSMTwoPoint);


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */

        initialState = FSMTwoPoint;

    }
}
