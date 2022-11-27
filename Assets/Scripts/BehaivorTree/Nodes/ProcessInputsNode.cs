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
        chaimbot.SetInput(0, chaimbot.Index.x);
        chaimbot.SetInput(1, chaimbot.Index.y);

        chaimbot.SetInput(2, chaimbot.Steps);

        if (chaimbot.NearFood != null)
        {
            chaimbot.SetInput(3, chaimbot.NearFood.Index.x);
            chaimbot.SetInput(4, chaimbot.NearFood.Index.y);

            chaimbot.SetInput(5, chaimbot.Index.x == chaimbot.NearFood.Index.x ? 1f : -1f);
            chaimbot.SetInput(6, chaimbot.Index.y == chaimbot.NearFood.Index.y ? 1f : -1f);
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
