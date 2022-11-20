using System.Collections.Generic;

public class Root : TreeNode
{
    #region CONSTRUCTORS
    public Root(List<TreeNode> childrens) : base(childrens)
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
                    state = NodeState.RUNNING;
                    return state;
                case NodeState.SUCCESS:
                    state = NodeState.SUCCESS;
                    return state;
                case NodeState.FAILURE:
                    break;
                default:
                    break;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
    #endregion
}