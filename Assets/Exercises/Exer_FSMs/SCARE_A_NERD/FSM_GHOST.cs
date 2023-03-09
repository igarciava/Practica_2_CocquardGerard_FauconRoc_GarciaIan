using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_GHOST", menuName = "Finite State Machines/FSM_GHOST", order = 1)]
public class FSM_GHOST : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private GHOST_Blackboard blackboard;
    private Arrive arrive;
    private SteeringContext context;
    private Pursue pursue;
    private GameObject RupertTheNerd;
    private float elapsedTime;


    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<GHOST_Blackboard>();
        context = GetComponent<SteeringContext>();
        arrive = GetComponent<Arrive>();
        pursue = GetComponent<Pursue>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        blackboard = null;
        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/
         
        State GoCastle = new State("GoCastle",
            () => { 
                context.maxSpeed *= 4; 
                arrive.target = blackboard.castle; 
                arrive.enabled = true; 
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { 
                context.maxSpeed /= 4; 
                arrive.target = null; 
                arrive.enabled = false; }  // write on exit logic inisde {}  
        );

        State Hide = new State("Hide",
            () => { elapsedTime = 0; }, // write on enter logic inside {}
            () => { elapsedTime += Time.deltaTime; }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

        State SelectTarget = new State("SelectTarget",
            () => { }, // write on enter logic inside {}
            () => { RupertTheNerd = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "NERD", blackboard.nerdDetectionRadius); }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

        State Approach = new State("Approach",
            () => { 
                pursue.target = RupertTheNerd;
                pursue.enabled = true;
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

        State CryBoo = new State("CryBoo",
            () => { 
                blackboard.CryBoo(true);
                elapsedTime = 0;
            }, // write on enter logic inside {}
            () => { elapsedTime += Time.deltaTime; }, // write in state logic inside {}
            () => {
                blackboard.CryBoo(false);
                pursue.target = null; 
                pursue.enabled = false;
            }  // write on exit logic inisde {}  
        );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition CastleReached = new Transition("CastleReached",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.castle) < blackboard.castleReachedRadius; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition HideTimeOut = new Transition("TimeOut",
            () => { return elapsedTime > blackboard.hideTime; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition TargetSelected = new Transition("TargetSelected",
            () => { return RupertTheNerd != null; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition TargetIsClose = new Transition("TargetIsClose",
            () => { return SensingUtils.DistanceToTarget(gameObject, RupertTheNerd) < blackboard.closeEnoughToScare; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        ); 

        Transition BooTimeOut = new Transition("CastleReached",
            () => { return elapsedTime > blackboard.booDuration; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(GoCastle, Hide, SelectTarget, Approach, CryBoo);

        AddTransition(GoCastle, CastleReached, Hide);
        AddTransition(Hide, HideTimeOut, SelectTarget);
        AddTransition(SelectTarget, TargetSelected, Approach);
        AddTransition(Approach, TargetIsClose, CryBoo);
        AddTransition(CryBoo, BooTimeOut, GoCastle);


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = GoCastle;

    }
}
