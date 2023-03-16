using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Mouse", menuName = "Finite State Machines/FSM_Mouse", order = 1)]
public class FSM_Mouse : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    GoToTarget goToTarget;
    MOUSE_Blackboard blackboard;
    float elapsedTime = 0;
    ArrivePlusOA arrive;


    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        goToTarget = GetComponent<GoToTarget>();
        blackboard = GetComponent<MOUSE_Blackboard>();
        arrive = GetComponent<ArrivePlusOA>();
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
         *-----------------------------------------------
         
        State varName = new State("StateName",
            () => { }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

         */
        State randomWalkablePoint = new State("RandomWalkablePoint",
            () => { goToTarget.enabled = true; blackboard.surrogateTarget.transform.position = RandomLocationGenerator.RandomWalkableLocation(); goToTarget.target = blackboard.surrogateTarget; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { goToTarget.enabled = false; }  // write on exit logic inisde {}  
        );

        State poo = new State("Poo",
            () => { Instantiate(blackboard.pooPrefab, transform.position,Quaternion.identity); elapsedTime = 0; }, // write on enter logic inside {}
            () => { elapsedTime += Time.deltaTime; }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );
        State randomExitPoint = new State("RandomExitPoint",
            () => { arrive.enabled = true; arrive.target = RandomLocationGenerator.RandomEnterExitLocation(); }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { arrive.enabled = false; }  // write on exit logic inisde {}  
        );
        State scared = new State("Scared",
            () => { arrive.enabled = true; arrive.target = blackboard.NearestExitPoint(); }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { arrive.enabled = false; }  // write on exit logic inisde {}  
        );
        State destroy = new State("Destroy",
            () => { Destroy(gameObject); }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );
        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition walkableToPoo = new Transition("WalkableToPoo",
            () => { Debug.Log(goToTarget.target); return SensingUtils.DistanceToTarget(gameObject, goToTarget.target) < blackboard.closeEnoughToTarget; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition pooToExitRandom = new Transition("PooToExitRandom",
            () => { return elapsedTime > 0; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition anyToScared = new Transition("WalkableToPoo",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.roomba) < blackboard.roombaDetectionRadius; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition anyToDestroy = new Transition("AnyToDestroy",
            () => { return SensingUtils.DistanceToTarget(gameObject, arrive.target) < blackboard.closeEnoughToTarget; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(scared,poo,randomWalkablePoint,randomExitPoint,destroy);

        AddTransition(randomWalkablePoint, walkableToPoo, poo);
        AddTransition(poo, pooToExitRandom, randomExitPoint);
        AddTransition(randomWalkablePoint, anyToScared, scared);
        AddTransition(poo, anyToScared, scared);
        AddTransition(randomExitPoint, anyToScared, scared);
        AddTransition(randomExitPoint, anyToDestroy, destroy);
        AddTransition(scared, anyToDestroy, destroy);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = randomWalkablePoint;

    }
}
