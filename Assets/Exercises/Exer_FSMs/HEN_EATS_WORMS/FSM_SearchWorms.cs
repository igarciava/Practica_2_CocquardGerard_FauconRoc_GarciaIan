using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_SearchWorms", menuName = "Finite State Machines/FSM_SearchWorms", order = 1)]
public class FSM_SearchWorms : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private HEN_Blackboard blackboard;
    private WanderAround wanderAround;
    private Arrive arrive;
    private AudioSource audioSource;
    private GameObject theWorm;
    private float elapsedTime;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        /* COMPLETE */
        blackboard = GetComponent<HEN_Blackboard>();
        wanderAround = GetComponent<WanderAround>();
        arrive = GetComponent<Arrive>();
        audioSource = GetComponent<AudioSource>();
        elapsedTime = 0.0f;

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        /* COMPLETE */
        audioSource.Stop();
        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* COMPLETE */
        
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/
         
        State WanderAround = new State("Wander",
            () => { wanderAround.enabled = true; wanderAround.attractor = blackboard.attractor; 
                audioSource.clip = blackboard.cluckingSound; audioSource.Play(); }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { wanderAround.enabled = false; wanderAround.attractor = null;
                audioSource.Stop(); audioSource.clip = null;
            }  // write on exit logic inisde {}  
        );

        State ReachWorm = new State("ReachWorm",
            () => { arrive.target = theWorm; arrive.enabled = true; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { arrive.enabled = false; arrive.target = null; }  // write on exit logic inisde {}  
        );

        State Eating = new State("Eating",
            () => { elapsedTime = 0.0f; audioSource.clip = blackboard.eatingSound; audioSource.Play(); }, // write on enter logic inside {}
            () => { elapsedTime += Time.deltaTime; }, // write in state logic inside {}
            () => { Destroy(theWorm); audioSource.Stop(); audioSource.clip = null; }  // write on exit logic inisde {}  
        );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition WormDetected = new Transition("WormDetected",
            () => { theWorm = SensingUtils.FindInstanceWithinRadius(gameObject, "WORM", blackboard.wormDetectableRadius);
                return theWorm != null;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition WormReached = new Transition("WormReached",
            () => { return SensingUtils.DistanceToTarget(gameObject, theWorm) < blackboard.wormReachedRadius; },
            () => { }
        );

        Transition WormVanished = new Transition("WormVanished",
            () => { return theWorm == null || theWorm.Equals(null); },
            () => { }
        );

        Transition TimeOut = new Transition("TimeOut",
            () => { return elapsedTime > blackboard.timeToEatWorm; },
            () => { }
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(WanderAround, ReachWorm, Eating);

        AddTransition(WanderAround, WormDetected, ReachWorm);
        AddTransition(ReachWorm, WormVanished, WanderAround);
        AddTransition(ReachWorm, WormReached, Eating);
        AddTransition(Eating, TimeOut, WanderAround);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */

        initialState = WanderAround;
    }
}
