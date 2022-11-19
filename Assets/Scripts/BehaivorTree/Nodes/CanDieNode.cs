using System.Collections.Generic;

public class CanDieNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    #endregion

    #region CONSTRUCTORS
    public CanDieNode(List<TreeNode> childrens, Chaimbot chaimbot) : base(childrens)
    {
        this.chaimbot = chaimbot;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        return chaimbot.CanDie ? NodeState.SUCCESS : NodeState.FAILURE;
    }
    #endregion
}
