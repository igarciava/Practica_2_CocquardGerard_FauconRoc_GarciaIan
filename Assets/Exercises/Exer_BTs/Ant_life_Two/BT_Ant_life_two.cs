using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_Ant_life_two", menuName = "Behaviour Trees/BT_Ant_life_two", order = 1)]
public class BT_Ant_life_two : BehaviourTree
{
    /* If necessary declare BT parameters here. 
       All public parameters must be of type string. All public parameters must be
       regarded as keys in/for the blackboard context.
       Use prefix "key" for input parameters (information stored in the blackboard that must be retrieved)
       use prefix "keyout" for output parameters (information that must be stored in the blackboard)

       e.g.
       public string keyDistance;
       public string keyoutObject 

       NOTICE: BT's with parameters cannot be constructed using ScriptableObject.CreateInstance<>
       An explicit constructor with new must be used. Unity will complain...
       Whenever possible, use parameter-less BT's. Use blackboard to pass information.
       TOP-level BTs (those attached to the executor) cannot have parameters
       
       In future versions, BT parameters may cease to exit

     */

     // construtor
    public BT_Ant_life_two()  { 
        /* Receive BT parameters and set them. Remember all are of type string */
    }
    
    public override void OnConstruction()
    {
        DynamicSelector feelFreeOrNot = new DynamicSelector();

        feelFreeOrNot.AddChild(
            new CONDITION_FeelUnsafe("attractor", "safeRadius", "extraSafeRadius"),
            new ACTION_WanderAround("attractor", "highSW")
            );

        feelFreeOrNot.AddChild(
            new CONDITION_AlwaysTrue(),
            new ACTION_WanderAround("attractor", "lowSW")
            );

        root = new RepeatForeverDecorator(feelFreeOrNot);
    }
}
