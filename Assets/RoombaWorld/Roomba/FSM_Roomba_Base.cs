using FSMs;
using UnityEngine;

[CreateAssetMenu(fileName = "FSM_Roomba_Base", menuName = "Finite State Machines/FSM_Roomba_Base", order = 1)]
public class FSM_Roomba_Base : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private ROOMBA_Blackboard blackboard;
    private GoToTarget goToTarget;
    private GameObject theDust;
    private GameObject theLastDust;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<ROOMBA_Blackboard>();
        goToTarget = GetComponent<GoToTarget>();
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
        /* STAGE 1: create the states with their logic(s)*/

        State Patrolling = new State("Patrolling",
            () =>
            {
                goToTarget.target = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "PATROLPOINT", blackboard.patrolPointRadius);
                goToTarget.enabled = true;
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () =>
            {
                goToTarget.enabled = false;
                goToTarget.target = null;
            }  // write on exit logic inisde {}  
        );

        State GoingToDust = new State("GoingToDust",
            () =>
            {
                goToTarget.target = theLastDust;
                goToTarget.enabled = true;
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () =>
            {
                goToTarget.enabled = false;
                goToTarget.target = null;
            }  // write on exit logic inisde {}  
        );

        State Cleaning = new State("Cleaning",
            () =>
            {
                Destroy(theLastDust);
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition RouteTerminated = new Transition("ToAnotherPatrolPoint",
            () =>
            {
                return goToTarget.routeTerminated();
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition DustDetected = new Transition("DustDetected",
            () =>
            {
                theDust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.dustDetectionRadius);
                blackboard.AddToMemory(theDust);
                theLastDust = blackboard.RetrieveFromMemory();
                return SensingUtils.DistanceToTarget(gameObject, theLastDust) < blackboard.dustDetectionRadius;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition DustReached = new Transition("DustReached",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, theLastDust) < blackboard.dustReachedRadius;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/

        AddStates(Patrolling, GoingToDust, Cleaning);
        AddTransition(Patrolling, RouteTerminated, Patrolling);
        AddTransition(Patrolling, DustDetected, GoingToDust);
        AddTransition(GoingToDust, DustReached, Cleaning);
        AddTransition(Cleaning, RouteTerminated, Patrolling);


        /* STAGE 4: set the initial state*/

        initialState = Patrolling;

    }
}
