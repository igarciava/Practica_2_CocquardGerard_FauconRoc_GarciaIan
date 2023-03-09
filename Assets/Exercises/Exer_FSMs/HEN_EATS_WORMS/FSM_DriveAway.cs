using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_DriveAway", menuName = "Finite State Machines/FSM_DriveAway", order = 1)]
public class FSM_DriveAway : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private Seek seek;
    private HEN_Blackboard blackboard;
    private AudioSource audioSource;
    private SteeringContext context;
    private GameObject chick;


    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        seek = GetComponent<Seek>();
        blackboard = GetComponent<HEN_Blackboard>();
        audioSource = GetComponent<AudioSource>();
        context = GetComponent<SteeringContext>();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        DisableAllSteerings();
        audioSource.Stop();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/

        FiniteStateMachine FSMSearchWorms = ScriptableObject.CreateInstance<FSM_SearchWorms>();

        State DriveAwayChick = new State("DriveAwayChick",
            () => {
                gameObject.transform.localScale *= 1.4f;
                context.maxAcceleration *= 2;
                context.maxSpeed *= 2;
                seek.target = chick; seek.enabled = true; 
                audioSource.clip = blackboard.angrySound; audioSource.Play();
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => {
                gameObject.transform.localScale /= 1.4f;
                context.maxAcceleration /= 2;
                context.maxSpeed /= 2;
                seek.enabled = false; audioSource.Stop();
            }  // write on exit logic inisde {}  
        );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition ChickTooClose = new Transition("ChickTooClose",
            () => { chick = SensingUtils.FindInstanceWithinRadius(gameObject, "CHICK", blackboard.chickDetectionRadius);
                return chick != null;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition ChickTooFar = new Transition("ChickTooFar",
            () => { return SensingUtils.DistanceToTarget(gameObject, chick) >= blackboard.chickFarEnoughRadius; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(FSMSearchWorms, DriveAwayChick);

        AddTransition(FSMSearchWorms, ChickTooClose, DriveAwayChick);
        AddTransition(DriveAwayChick, ChickTooFar, FSMSearchWorms);


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */

        initialState = FSMSearchWorms;

    }
}
