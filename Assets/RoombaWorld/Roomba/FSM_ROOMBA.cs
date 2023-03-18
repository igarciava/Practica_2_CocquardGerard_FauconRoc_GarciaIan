using FSMs;
using UnityEngine;

[CreateAssetMenu(fileName = "FSM_ROOMBA", menuName = "Finite State Machines/FSM_ROOMBA", order = 1)]
public class FSM_ROOMBA : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private GoToTarget goToTarget;
    private ROOMBA_Blackboard blackboard;
    private GameObject chargingStation;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        goToTarget = GetComponent<GoToTarget>();
        blackboard = GetComponent<ROOMBA_Blackboard>();
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

        FiniteStateMachine BaseBehaviour = ScriptableObject.CreateInstance<FSM_Roomba_Base>();
        BaseBehaviour.Name = "Base Behaviour";

        State GoingToCharge = new State("GoingToCharge",
            () =>
            {
                chargingStation = SensingUtils.FindInstanceWithinRadius(gameObject, "ENERGY", blackboard.chargeStationDetectionRadius);
                goToTarget.target = chargingStation;
                goToTarget.enabled = true;
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () =>
            {
                goToTarget.enabled = false;
                goToTarget.target = null;
            }  // write on exit logic inisde {}  
        );

        State Charging = new State("Charging",
            () => { }, // write on enter logic inside {}
            () =>
            {
                blackboard.Recharge(Time.deltaTime);
            }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition RouteTerminated = new Transition("RouteTerminated",
            () =>
            {
                return goToTarget.routeTerminated();
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition NeedToCharge = new Transition("NeedToCharge",
            () =>
            {
                if (blackboard.currentCharge < blackboard.minCharge)
                    return true;
                return false;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition Charged = new Transition("Charged",
            () =>
            {
                if (blackboard.currentCharge > blackboard.maxCharge)
                    return true;
                return false;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/

        AddStates(BaseBehaviour, GoingToCharge, Charging);

        AddTransition(BaseBehaviour, NeedToCharge, GoingToCharge);
        AddTransition(GoingToCharge, RouteTerminated, Charging);
        AddTransition(Charging, Charged, BaseBehaviour);


        /* STAGE 4: set the initial state*/

        initialState = BaseBehaviour;

    }
}
