using System.Collections.Generic;

using UnityEngine;

public class StopMovementNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Animator animator = null;
    #endregion

    #region CONSTANTS
    private const string speedKey = "speed";
    #endregion

    #region CONSTRUCTORS
    public StopMovementNode(List<TreeNode> childrens, Animator animator) : base(childrens)
    {
        this.animator = animator;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        animator.SetFloat(speedKey, 0f);

        return NodeState.SUCCESS;
    }
    #endregion
}
