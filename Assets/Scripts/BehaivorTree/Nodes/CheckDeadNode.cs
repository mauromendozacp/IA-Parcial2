using System.Collections.Generic;

public class CheckDeadNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    #endregion

    #region CONSTRUCTORS
    public CheckDeadNode(List<TreeNode> childrens, Chaimbot chaimbot) : base(childrens)
    {
        this.chaimbot = chaimbot;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        return chaimbot.Dead ? NodeState.FAILURE : NodeState.SUCCESS;
    }
    #endregion
}
