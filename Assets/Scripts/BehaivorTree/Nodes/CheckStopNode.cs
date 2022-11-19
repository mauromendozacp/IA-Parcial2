using System.Collections.Generic;

public class CheckStopNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    #endregion

    #region CONSTRUCTORS
    public CheckStopNode(List<TreeNode> childrens, Chaimbot chaimbot) : base(childrens)
    {
        this.chaimbot = chaimbot;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        return chaimbot.Stop ? NodeState.SUCCESS : NodeState.FAILURE;
    }
    #endregion
}
