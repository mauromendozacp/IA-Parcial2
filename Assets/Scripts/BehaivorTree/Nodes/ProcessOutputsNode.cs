using System.Collections.Generic;

using UnityEngine;

public class ProcessOutputsNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    #endregion

    #region CONSTRUCTORS
    public ProcessOutputsNode(List<TreeNode> childrens, Chaimbot chaimbot) : base(childrens)
    {
        this.chaimbot = chaimbot;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        float[] outputs = chaimbot.Brain.Synapsis(chaimbot.Inputs);

        if (outputs != null && outputs.Length >= 3)
        {
            bool vertical = outputs[0] < 0.5f;
            float positive = outputs[1] < 0.5f ? -1f : 1f;
            bool stay = outputs[2] < 0.5f;

            Vector3 dir = new Vector3(vertical ? positive : 0f, 0f, !vertical ? positive : 0f);

            chaimbot.transform.forward = dir;
            chaimbot.MovePosition = chaimbot.transform.position + dir * chaimbot.Unit;
            chaimbot.MoveIndex = chaimbot.Index + new Vector2Int((int)dir.x, (int)dir.z);

            chaimbot.ToStay = stay;

            chaimbot.Process = false;
        }

        return NodeState.RUNNING;
    }
    #endregion
}
