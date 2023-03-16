using FSMs;
using UnityEngine;

[CreateAssetMenu(fileName = "FSM_Roomba_Fase2", menuName = "Finite State Machines/FSM_Roomba_Fase2", order = 1)]
public class FSM_Roomba_Fase2 : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private ROOMBA_Blackboard blackboard;
    private GoToTarget goToTarget;
    private GameObject thePoo;

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
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/

        FiniteStateMachine RoombaBase = ScriptableObject.CreateInstance<FSM_Roomba_Base>();
        RoombaBase.Name = "DefaultBehaviour";

        State GoingToPoo = new State("GoingToPoo",
            () =>
            {
                goToTarget.target = thePoo;
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
                Destroy(thePoo);
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition PooDetected = new Transition("PooDetected",
            () =>
            {
                thePoo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.pooDetectionRadius);
                return SensingUtils.DistanceToTarget(gameObject, thePoo) < blackboard.pooDetectionRadius;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition PooReached = new Transition("PooReached",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, thePoo) < blackboard.pooReachedRadius;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition RouteTerminated = new Transition("RouteTerminated",
            () =>
            {

                return goToTarget.routeTerminated();
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/

        AddStates(RoombaBase, GoingToPoo, Cleaning);
        AddTransition(RoombaBase, PooDetected, GoingToPoo);
        AddTransition(GoingToPoo, PooReached, Cleaning);
        AddTransition(Cleaning, RouteTerminated, RoombaBase);

        /* STAGE 4: set the initial state*/

        initialState = RoombaBase;

    }
}
