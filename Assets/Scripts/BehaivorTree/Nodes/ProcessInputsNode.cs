using System.Collections.Generic;

using UnityEngine;

public class ProcessInputsNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    #endregion

    #region CONSTRUCTORS
    public ProcessInputsNode(List<TreeNode> childrens, Chaimbot chaimbot) : base(childrens)
    {
        this.chaimbot = chaimbot;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        if (chaimbot.NearFood != null)
        {
            Vector3 foodPosition = chaimbot.NearFood.transform.position;
            Vector3 foodDirection = GetDirToFood(foodPosition);

            chaimbot.SetInput(0, foodPosition.x);
            chaimbot.SetInput(1, foodPosition.z);
            chaimbot.SetInput(2, foodDirection.x);
            chaimbot.SetInput(3, foodDirection.z);
        }

        return NodeState.RUNNING;
    }
    #endregion

    #region PRIVATE_METHODS
    private Vector3 GetDirToFood(Vector3 foodPosition)
    {
        return (foodPosition - chaimbot.transform.position).normalized;
    }
    #endregion
}
