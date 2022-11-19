using System.Collections.Generic;

using UnityEngine;

public class UpdateIndexLimitNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    private int maxIndex = 0;
    #endregion

    #region CONSTRUCTORS
    public UpdateIndexLimitNode(List<TreeNode> childrens, Chaimbot chaimbot, int maxIndex) : base(childrens)
    {
        this.chaimbot = chaimbot;
        this.maxIndex = maxIndex;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        Vector2Int moveIndex = chaimbot.MoveIndex;

        if (moveIndex.x > maxIndex)
        {
            moveIndex.x = 0;
        }
        else if (moveIndex.x < 0)
        {
            moveIndex.x = maxIndex;
        }

        chaimbot.MoveIndex = moveIndex;

        return NodeState.RUNNING;
    }
    #endregion
}
