using System.Collections.Generic;

#region ENUMS
public enum NodeState
{
    RUNNING,
    SUCCESS,
    FAILURE
}
#endregion

public abstract class TreeNode
{
    #region PROTECTED_FIELDS
    protected TreeNode parent = null;
    protected NodeState state = default;
    protected List<TreeNode> childrens = new List<TreeNode>();
    #endregion

    #region PRIVATE_FIELDS
    private Dictionary<string, object> data = new Dictionary<string, object>();
    #endregion

    #region PROPERTIES
    public NodeState State => state;
    #endregion

    #region CONSTRUCTORS
    public TreeNode()
    {
        parent = null;
    }

    public TreeNode(List<TreeNode> childrens)
    {
        if (childrens != null)
        {
            foreach (TreeNode n in childrens)
            {
                Attach(n);
            }
        }
    }
    #endregion

    #region PUBLIC_METHODS
    public void Attach(TreeNode node)
    {
        node.parent = this;
        childrens.Add(node);
    }

    public virtual NodeState Evaluate() => NodeState.FAILURE;

    public void SetData(string key, object value)
    {
        if (data.ContainsKey(key))
        {
            data[key] = value;
        }
        else
        {
            data.Add(key, value);
        }
    }

    public object GetData(string key)
    {
        if (data.TryGetValue(key, out object value))
        {
            return value;
        }

        TreeNode parentNode = parent;
        while (parentNode != null)
        {
            value = parentNode.GetData(key);
            if (value != null)
            {
                return value;
            }
            parentNode = parent;
        }

        return null;
    }

    public bool RemoveData(string key)
    {
        if (data.ContainsKey(key))
        {
            data.Remove(key);
            return true;
        }

        TreeNode parentNode = parent;
        while (parentNode != null)
        {
            if (parentNode.RemoveData(key)) return true;

            parentNode = parent;
        }

        return false;
    }
    #endregion
}