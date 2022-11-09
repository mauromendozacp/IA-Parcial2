using System;
using System.Collections.Generic;

public class Task : TreeNode
{
    #region PRIVATE_FIELDS
    private Action<Action, Action> onRunning = null;
    #endregion

    #region CONSTRUCTORS
    public Task(List<TreeNode> childrens, Action<Action, Action> onRunning) : base(childrens)
    {
        this.onRunning = onRunning;
    }
    #endregion

    #region PUBLIC_METHODS
    public override NodeState Evaluate()
    {
        state = NodeState.RUNNING;
        onRunning?.Invoke(Success, Failure);

        return state;
    }
    #endregion

    #region PRIVATE_METHODS
    private void Success()
    {
        state = NodeState.SUCCESS;
    }

    private void Failure()
    {
        state = NodeState.FAILURE;
    }
    #endregion
}