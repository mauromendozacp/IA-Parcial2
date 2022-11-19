using System.Collections.Generic;

using UnityEngine;

public class DieNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    private Animator animator = null;
    #endregion

    #region CONSTANTS
    private const string deadKey = "dead";
    #endregion

    #region CONSTRUCTORS
    public DieNode(List<TreeNode> childrens, Chaimbot chaimbot, Animator animator) : base(childrens)
    {
        this.chaimbot = chaimbot;
        this.animator = animator;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        chaimbot.Dead = true;

        animator.SetTrigger(deadKey);

        PopulationManager.Instance.AddDeaths(chaimbot.Team);

        return NodeState.SUCCESS;
    }
    #endregion
}
