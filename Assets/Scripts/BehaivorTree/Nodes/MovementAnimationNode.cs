using System.Collections.Generic;

using UnityEngine;

public class MovementAnimationNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    private Animator animator = null;
    private AnimationCurve curve = null;
    #endregion

    #region CONSTANTS
    private const string speedKey = "speed";
    #endregion

    #region CONSTRUCTORS
    public MovementAnimationNode(List<TreeNode> childrens, Chaimbot chaimbot, Animator animator, AnimationCurve curve) : base(childrens)
    {
        this.chaimbot = chaimbot;
        this.animator = animator;
        this.curve = curve;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        float lerp = chaimbot.Lerp;

        animator.SetFloat(speedKey, curve.Evaluate(lerp));

        return NodeState.RUNNING;
    }
    #endregion
}
