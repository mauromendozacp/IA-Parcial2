using System.Collections.Generic;

using UnityEngine;

public class ChaimbotDeadNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Animator animator = null;
    #endregion

    #region CONSTANTS
    private const string deadKey = "dead";
    private const string teamKey = "team";
    #endregion

    #region CONSTRUCTORS
    public ChaimbotDeadNode(List<TreeNode> childrens, Animator animator) : base(childrens)
    {
        this.animator = animator;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        TEAM team = (TEAM)GetData(teamKey);

        animator.SetTrigger(deadKey);
        PopulationManager.Instance.AddDeaths(team);

        return NodeState.RUNNING;
    }
    #endregion
}
