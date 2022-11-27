using System.Collections.Generic;

public class UpdateMovementNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    #endregion

    #region CONSTRUCTORS
    public UpdateMovementNode(List<TreeNode> childrens, Chaimbot chaimbot) : base(childrens)
    {
        this.chaimbot = chaimbot;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        chaimbot.transform.position = chaimbot.MovePosition;
        chaimbot.StartPosition = chaimbot.transform.position;
        chaimbot.Index = chaimbot.MoveIndex;
        chaimbot.Steps++;

        return NodeState.RUNNING;
    }
    #endregion
}
