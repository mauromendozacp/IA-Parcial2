using System.Collections.Generic;

using UnityEngine;

public class MovementLerpNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    #endregion

    #region CONSTRUCTORS
    public MovementLerpNode(List<TreeNode> childrens, Chaimbot chaimbot) : base(childrens)
    {
        this.chaimbot = chaimbot;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        Vector3 startPosition = chaimbot.StartPosition;
        Vector3 movePosition = chaimbot.MovePosition;
        float lerp = chaimbot.Lerp;

        chaimbot.transform.position = Vector3.Lerp(startPosition, movePosition, lerp);

        return NodeState.RUNNING;
    }
    #endregion
}
