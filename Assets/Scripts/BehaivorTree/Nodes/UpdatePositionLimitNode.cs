using System.Collections.Generic;

using UnityEngine;

public class UpdatePositionLimitNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    private float limitX = 0f;
    private float unit = 0f;
    #endregion

    #region CONSTRUCTORS
    public UpdatePositionLimitNode(List<TreeNode> childrens, Chaimbot chaimbot, float limitX, float unit) : base(childrens)
    {
        this.chaimbot = chaimbot;
        this.limitX = limitX;
        this.unit = unit;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        Vector3 startPos = chaimbot.StartPosition;
        Vector3 movePos = chaimbot.MovePosition;

        if (movePos.x > limitX)
        {
            movePos.x -= limitX * 2;
            startPos = movePos - new Vector3(unit, 0f, 0f);
        }
        else if (movePos.x < -limitX)
        {
            movePos.x += limitX * 2;
            startPos = movePos + new Vector3(unit, 0f, 0f);
        }

        chaimbot.StartPosition = startPos;
        chaimbot.MovePosition = movePos;

        return NodeState.RUNNING;
    }
    #endregion
}
