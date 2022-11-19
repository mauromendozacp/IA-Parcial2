using System.Collections.Generic;

public class Not : TreeNode
{
    #region CONSTRUCTORS
    public Not(List<TreeNode> childrens) : base(childrens)
    {
    }
    #endregion

    #region PUBLIC_METHODS
    public override NodeState Evaluate()
    {
        foreach (TreeNode node in childrens)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    break;
                case NodeState.SUCCESS:
                    state = NodeState.FAILURE;
                    return state;
                case NodeState.FAILURE:
                    state = NodeState.SUCCESS;
                    return state;
                default:
                    break;
            }
        }

        state = NodeState.RUNNING;
        return state;
    }
    #endregion
}