using UnityEngine;

public class Chaimbot : Agent
{
    #region EXPOSED_FIELDS
    [SerializeField] private Animator animator = null;
    [SerializeField] private AnimationCurve curve = null;

    [Header("View Settings")]
    [SerializeField] private GameObject[] maleViews = null;
    [SerializeField] private GameObject[] femaleViews = null;
    #endregion

    #region PRIVATE_FIELDS
    private float unit = 0f;
    private Vector2Int index = Vector2Int.zero;

    private BehaviourTree behaviourTree = null;

    private Food nearFood = null;
    private int foodsConsumed = 0;

    private Vector2Int moveIndex = Vector2Int.zero;
    private Vector3 startPosition = Vector3.zero;
    private Vector3 movePosition = Vector3.zero;
    #endregion

    #region CONSTANTS_FIELDS
    private const string speedKey = "speed";
    #endregion

    #region PROPERTIES
    public Vector2Int Index { get => index; set => index = value; }
    public Vector2Int MoveIndex { get => moveIndex; set => moveIndex = value; }
    public Vector3 MovePosition { get => movePosition; set => movePosition = value; }
    public int FoodsConsumed { get => foodsConsumed; }
    public TEAM Team { get => team; }
    #endregion

    #region PUBLIC_METHODS
    public void Init(float unit, TEAM team)
    {
        this.unit = unit;
        this.team = team;

        SetView(team);
    }

    public void ResetPositions()
    {
        moveIndex = index;

        startPosition = transform.position;
        movePosition = transform.position;
    }

    public void Move(float lerp)
    {
        transform.position = Vector3.Lerp(startPosition, movePosition, lerp);

        animator.SetFloat(speedKey, curve.Evaluate(lerp));
    }

    public void SetNearFood(Food nearFood)
    {
        this.nearFood = nearFood;
    }

    public void Think()
    {
        OnThink();

        if (index == nearFood.Index)
        {
            SetGoodFitness();

            foodsConsumed++;
        }
    }
    #endregion

    #region OVERRIDE_METHODS
    protected override void ProcessInputs()
    {
        if (nearFood != null)
        {
            Vector3 foodPosition = nearFood.transform.position;
            Vector3 foodDirection = GetDirToFood(foodPosition);

            inputs[0] = foodPosition.x;
            inputs[1] = foodPosition.z;
            inputs[2] = foodDirection.x;
            inputs[3] = foodDirection.z;
        }
    }

    protected override void ProcessOutputs(float[] outputs)
    {
        transform.position = movePosition;
        startPosition = transform.position;
        index = moveIndex;

        if (outputs != null && outputs.Length >= 2)
        {
            bool vertical = outputs[0] < 0.5f;
            float positive = outputs[1] < 0.5f ? -1f : 1f;

            Vector3 dir = new Vector3(vertical ? positive : 0f, 0f, !vertical ? positive : 0f);
            movePosition = transform.position + dir * unit;
            transform.forward = dir;

            moveIndex = index + new Vector2Int((int)dir.x, (int)dir.z);
        }
    }

    protected override void OnReset()
    {
        base.OnReset();

        foodsConsumed = 0;
    }
    #endregion

    #region PRIVATE_METHODS
    private void SetView(TEAM team)
    {
        for (int i = 0; i < maleViews.Length; i++)
        {
            maleViews[i].SetActive(team == TEAM.A);
        }
        for (int i = 0; i < femaleViews.Length; i++)
        {
            femaleViews[i].SetActive(team == TEAM.B);
        }
    }

    private Vector3 GetDirToFood(Vector3 foodPosition)
    {
        return (foodPosition - transform.position).normalized;
    }
    #endregion
}