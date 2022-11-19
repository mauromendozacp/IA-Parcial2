using System.Collections.Generic;

public class CheckProcessNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    #endregion

    #region CONSTRUCTORS
    public CheckProcessNode(List<TreeNode> childrens, Chaimbot chaimbot) : base(childrens)
    {
        this.chaimbot = chaimbot;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        return chaimbot.Process ? NodeState.SUCCESS : NodeState.FAILURE;
    }
    #endregion
}
