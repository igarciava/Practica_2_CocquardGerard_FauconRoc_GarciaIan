using BTs;
using UnityEngine;

[CreateAssetMenu(fileName = "BT_Three", menuName = "Behaviour Trees/BT_Three", order = 1)]
public class BT_Three : BehaviourTree
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
    public BT_Three()
    {
        /* Receive BT parameters and set them. Remember all are of type string */
    }

    public override void OnConstruction()
    {
        root = new Sequence(
            new ACTION_Arrive("home"),
            new Selector(
                new Sequence(
                    new CONDITION_InstanceNear("moneyDetectionRadius", "moneyTag", "false", "theMoney"),
                    new ACTION_Take("theMoney"),
                    new ACTION_Arrive("bank"),
                    new ACTION_Drop("theMoney"),
                    new ACTION_Arrive("home")),
                new Sequence(
                    new CONDITION_InstanceNear("trashDetectionRadius", "trashTag", "false", "theTrash"),
                    new ACTION_Take("theTrash"),
                    new ACTION_Arrive("dump"),
                    new ACTION_Drop("theTrash"),
                    new ACTION_Arrive("home")),
                new Sequence(
                    new ACTION_Speak("Nothing\nto do"),
                    new ACTION_WaitForSeconds("2"),
                    new ACTION_Quiet())
                )
            );
    }
}
