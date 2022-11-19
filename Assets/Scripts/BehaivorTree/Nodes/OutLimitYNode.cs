using System.Collections.Generic;

public class OutLimitYNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    private float outLimitYFitness = 0f;
    #endregion

    #region CONSTRUCTORS
    public OutLimitYNode(List<TreeNode> childrens, Chaimbot chaimbot, float outLimitYFitness) : base(childrens)
    {
        this.chaimbot = chaimbot;
        this.outLimitYFitness = outLimitYFitness;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        chaimbot.UpdateFitness(outLimitYFitness);
        chaimbot.InOutLimit = true;

        return NodeState.FAILURE;
    }
    #endregion
}
