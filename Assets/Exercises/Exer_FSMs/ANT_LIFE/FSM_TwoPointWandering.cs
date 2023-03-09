using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_TwoPointWandering", menuName = "Finite State Machines/FSM_TwoPointWandering", order = 1)]
public class FSM_TwoPointWandering : FiniteStateMachine
{
    

    private WanderAround wanderAround;
    private SteeringContext steeringContext;
    private ANT_Blackboard blackboard;

    private float elapsedTime = 0;


    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is executed every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        /* COMPLETE */
        wanderAround = GetComponent<WanderAround>();
        steeringContext = GetComponent<SteeringContext>();
        blackboard = GetComponent<ANT_Blackboard>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        /* COMPLETE */
        base.DisableAllSteerings();

        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         */

        State goingA = new State("Going_A",
           () => { /* COMPLETE */ 
               elapsedTime = 0; 
               wanderAround.enabled = true; 
               wanderAround.attractor = blackboard.LocationA; 
           },
           () => { elapsedTime += Time.deltaTime;}, 
           () => {/* COMPLETE */ 
               wanderAround.enabled = false; 
               wanderAround.attractor = null; 
           }
       );

        State goingB = new State("Going_B",
           () => {/* COMPLETE */ 
               elapsedTime = 0; 
               wanderAround.enabled = true; 
               wanderAround.attractor = blackboard.LocationB; 
           },
           () => { elapsedTime += Time.deltaTime; },
           () => { /* COMPLETE */ 
               wanderAround.enabled = false; 
               wanderAround.attractor = null; 
           }
       );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------
        */

        /* COMPLETE, create the transitions */
        
        Transition ReachedA = new Transition("TransitionAtoB",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.LocationA) < blackboard.locationReachedRadius; }, // write the condition checkeing code in {}
            () => { steeringContext.seekWeight = blackboard.initialSeekWeight; }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition ReachedB = new Transition("TransitionBtoA",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.LocationB) < blackboard.locationReachedRadius; }, // write the condition checkeing code in {}
            () => { steeringContext.seekWeight = blackboard.initialSeekWeight; }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition TimeOut = new Transition("TransitionTimeOut",
            () => { return elapsedTime >= blackboard.intervalBetweenTimeOuts; }, // write the condition checkeing code in {}
            () => { steeringContext.seekWeight += blackboard.seekIncrement; }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );



        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
         */

        AddStates(goingA, goingB);

        /* COMPLETE, add the transitions */

        AddTransition(goingA, ReachedA, goingB);
        AddTransition(goingB, ReachedB, goingA);
        AddTransition(goingA, TimeOut, goingA);
        AddTransition(goingB, TimeOut, goingB);

        /* STAGE 4: set the initial state */

        initialState = goingA;
    }
}
