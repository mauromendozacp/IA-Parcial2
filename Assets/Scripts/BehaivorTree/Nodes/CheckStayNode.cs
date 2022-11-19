using System.Collections.Generic;

public class CheckStayNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    #endregion

    #region CONSTRUCTORS
    public CheckStayNode(List<TreeNode> childrens, Chaimbot chaimbot) : base(childrens)
    {
        this.chaimbot = chaimbot;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        return chaimbot.ToStay ? NodeState.FAILURE : NodeState.SUCCESS;
    }
    #endregion
}
