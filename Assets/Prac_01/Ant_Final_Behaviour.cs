using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "Ant_Final_Behaviour", menuName = "Finite State Machines/Ant_Final_Behaviour", order = 1)]
public class Ant_Final_Behaviour : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private ANT_Blackboard blackboard;
    private Flee flee;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<ANT_Blackboard>();
        flee = GetComponent<Flee>();
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

        FiniteStateMachine normal = ScriptableObject.CreateInstance<FSM_SeedCollecting>();

        State Escape = new State("Escaping",
                    () => { flee.target = blackboard.cock; flee.enabled = true; }, // write on enter logic inside {}
                    () => { }, // write in state logic inside {}
                    () => { flee.enabled = false; }  // write on exit logic inisde {}  
                );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition IsCockClose = new Transition("CockClose",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.cock) <= blackboard.cockDetectionRadius; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition IsCockFarEnough = new Transition("CockFar",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.cock) <= blackboard.cockFarEnoughRadius; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */

        AddStates(normal, Escape);

        AddTransition(normal, IsCockClose, Escape);
        AddTransition(Escape, IsCockFarEnough, normal);


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */

        initialState = normal;

    }
}
