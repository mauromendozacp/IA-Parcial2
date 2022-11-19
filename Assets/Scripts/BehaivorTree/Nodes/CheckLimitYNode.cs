using System.Collections.Generic;

public class CheckLimitYNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    private int limitY = 0;
    #endregion

    #region CONSTRUCTORS
    public CheckLimitYNode(List<TreeNode> childrens, Chaimbot chaimbot, int limitY) : base(childrens)
    {
        this.chaimbot = chaimbot;
        this.limitY = limitY;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        return chaimbot.Index.y >= 0 && chaimbot.Index.y <= limitY ? NodeState.SUCCESS : NodeState.FAILURE;
    }
    #endregion
}
