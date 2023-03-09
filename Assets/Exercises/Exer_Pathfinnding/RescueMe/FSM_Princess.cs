using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Princess", menuName = "Finite State Machines/FSM_Princess", order = 1)]
public class FSM_Princess : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
      * states and transitions and/or set in OnEnter or used in OnExit 
      * For instance: steering behaviours, blackboard, ...*/

    private ROYAL_Blackboard blackboard;
    private PathFeeder pathFeeder;
    private PathFollowing pathFollowing;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        blackboard = GetComponent<ROYAL_Blackboard>();
        pathFeeder = GetComponent<PathFeeder>();
        pathFollowing = GetComponent<PathFollowing>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {

        /* COMPLETE */

        /* STAGE 1: create the states with their logic(s)
          *-----------------------------------------------*/

        State BeQuiet = new State("BeQuiet",
            () => { }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

        State GoingToPartner = new State("GoingToPartner",
            () => { pathFeeder.target = blackboard.partner; pathFeeder.enabled = true; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { pathFeeder.enabled = false; }  // write on exit logic inisde {}  
        );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition PartnerCloseEnough = new Transition("PartnerCloseEnough",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.partner) < 5.0f; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/

        AddStates(BeQuiet, GoingToPartner);

        AddTransition(BeQuiet, PartnerCloseEnough, GoingToPartner);


        /* STAGE 4: set the initial state*/

        initialState = BeQuiet;
        
    }

}
