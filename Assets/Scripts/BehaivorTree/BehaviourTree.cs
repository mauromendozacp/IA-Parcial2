using UnityEngine;

public abstract class BehaviourTree : MonoBehaviour
{
    #region PROTECTED_FIELDS
    protected TreeNode root = null;
    #endregion

    #region PROTECTED_METHODS
    protected virtual void Init()
    {
        root = Setup();
    }

    public virtual void UpdateTree()
    {
        root?.Evaluate();
    }

    protected abstract TreeNode Setup();
    #endregion
}