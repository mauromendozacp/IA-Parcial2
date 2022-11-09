using UnityEngine;

public abstract class BehaviourTree : MonoBehaviour
{
    #region PROTECTED_FIELDS
    protected TreeNode root = null;
    protected bool finish = false;
    #endregion

    #region UNITY_CALLS
    private void Start()
    {
        root = Setup();
    }

    private void Update()
    {
        if (root != null && !finish)
        {
            finish = root.Evaluate() == NodeState.RUNNING ? false : true;
        }
    }
    #endregion

    #region PROTECTED_METHODS
    protected abstract TreeNode Setup();
    #endregion
}