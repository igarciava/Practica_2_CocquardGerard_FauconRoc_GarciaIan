using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Roomba_Base", menuName = "Finite State Machines/FSM_Roomba_Base", order = 1)]
public class FSM_Roomba_Base : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private ROOMBA_Blackboard blackboard;
    private GoToTarget goToTarget;
    private SteeringContext context;
    private GameObject theDust;
    private GameObject thePoo;
    float maxSpeed;
    float maxAcceleration;
    


    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<ROOMBA_Blackboard>();
        goToTarget = GetComponent<GoToTarget>();
        context = GetComponent<SteeringContext>();
        maxSpeed = context.maxSpeed;
        maxAcceleration = context.maxAcceleration;
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        context.maxSpeed = maxSpeed;
        context.maxAcceleration = maxAcceleration;
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
                goToTarget.target = null;
                goToTarget.enabled = false;
            }  // write on exit logic inisde {}  
        );

        State GoingToDust = new State("GoingToDust",
            () =>
            {
                goToTarget.target = theDust;
                goToTarget.enabled = true;
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () =>
            {
                goToTarget.target = null;
                goToTarget.enabled = false;
            }  // write on exit logic inisde {}  
        );

        State GoingToPoo = new State("GoingToPoo",
            () =>
            {
                context.maxSpeed *= 1.3f;
                context.maxAcceleration *= 2.6f;
                goToTarget.target = thePoo;
                goToTarget.enabled = true;
            }, // write on enter logic inside {}
            () => { GameObject dust = SensingUtils.FindInstanceWithinRadius(gameObject,"DUST",blackboard.dustDetectionRadius);
                    if(dust != null) blackboard.AddToMemory(dust);
                  }, // write in state logic inside {}
            () =>
            {
                goToTarget.target = null;
                goToTarget.enabled = false;
                context.maxSpeed /= 1.3f;
                context.maxAcceleration /= 2.6f;
            }  // write on exit logic inisde {}  
        );

        State CleaningTheDust = new State("CleaningTheDust",
            () =>
            {
                blackboard.RetrieveFromMemory();
                Destroy(theDust);
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

        State CleaningThePoo = new State("CleaningThePoo",
            () =>
            {
                Destroy(thePoo);
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
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

        Transition DustDetected = new Transition("DustDetected",
            () =>
            {
                theDust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.dustDetectionRadius);
                if(theDust != null) return SensingUtils.DistanceToTarget(gameObject, theDust) < blackboard.dustDetectionRadius;
                else return false;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition PooDetected = new Transition("PooDetected",
            () =>
            {
                thePoo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.pooDetectionRadius);
                if(thePoo != null) return SensingUtils.DistanceToTarget(gameObject, thePoo) < blackboard.pooDetectionRadius;
                else return false;
            }, // write the condition checkeing code in {}
            () => { if(theDust != null) blackboard.AddToMemory(theDust);
                    theDust = null;
                  }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition AnyToPatrol = new Transition("PassTransition",
            () =>
            {
                GameObject closerPoo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.pooDetectionRadius);
                bool memo = blackboard.somethingInMemory();
                return !memo && !closerPoo;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition SomethingOnMemo = new Transition("PassTransition",
            () =>
            {
                return blackboard.somethingInMemory();
            }, // write the condition checkeing code in {}
            () => { theDust = blackboard.RetrieveFromMemory(); }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition CloserPoo = new Transition("CloserPoo",
            () =>
            {
                GameObject closerPoo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.pooDetectionRadius);
                if (thePoo != closerPoo)
                {
                    thePoo = closerPoo;
                    return true;
                }
                else return false;
            }, // write the condition checkeing code in {}
            () => {  }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/

        AddStates(Patrolling, GoingToDust, GoingToPoo, CleaningTheDust, CleaningThePoo);

        AddTransition(Patrolling, RouteTerminated, Patrolling);
        AddTransition(Patrolling, PooDetected, GoingToPoo);
        AddTransition(Patrolling, DustDetected, GoingToDust);
        
        AddTransition(GoingToDust, PooDetected, GoingToPoo);
        AddTransition(GoingToDust, RouteTerminated, CleaningTheDust);
        AddTransition(CleaningTheDust, AnyToPatrol, Patrolling);
        AddTransition(CleaningTheDust, SomethingOnMemo, GoingToDust);
        
        AddTransition(GoingToPoo, CloserPoo, GoingToPoo);
        AddTransition(GoingToPoo, RouteTerminated, CleaningThePoo);
        AddTransition(CleaningThePoo, AnyToPatrol, Patrolling);
        AddTransition(CleaningThePoo, SomethingOnMemo, GoingToDust);

        /* STAGE 4: set the initial state*/

        initialState = Patrolling;

    }
}
