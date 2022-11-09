using System.Collections.Generic;

public class Sequence : TreeNode
{
    #region CONSTRUCTORS
    public Sequence(List<TreeNode> childrens) : base(childrens)
    {
    }
    #endregion

    #region PUBLIC_METHODS
    public override NodeState Evaluate()
    {
        bool anyRunningNode = false;
        foreach (TreeNode node in childrens)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    anyRunningNode = true;
                    break;
                case NodeState.SUCCESS:
                    break;
                case NodeState.FAILURE:
                    state = NodeState.FAILURE;
                    return state;
                default:
                    break;
            }
        }

        state = anyRunningNode ? NodeState.RUNNING : NodeState.SUCCESS;
        return state;
    }
    #endregion
}