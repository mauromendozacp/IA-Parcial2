using UnityEngine;

public class Chaimbot : Agent
{
    #region EXPOSED_FIELDS
    [SerializeField] private Animator animator = null;
    [SerializeField] private AnimationCurve curve = null;

    [Header("View Settings")]
    [SerializeField] private GameObject[] maleViews = null;
    [SerializeField] private GameObject[] femaleViews = null;

    [Header("Fitness Settings")]
    [SerializeField] private float consumeNearFoodFitness = 0f;
    [SerializeField] private float consumeFoodFitness = 0f;
    [SerializeField] private float outLimitYFitness = 0f;
    #endregion

    #region PRIVATE_FIELDS
    private float unit = 0f;
    private bool dead = false;
    private bool toStay = false;
    private int generationCount = 0;

    private Food nearFood = null;
    private int foodsConsumed = 0;

    private Vector2Int index = Vector2Int.zero;
    private Vector2Int moveIndex = Vector2Int.zero;
    private Vector3 startPosition = Vector3.zero;
    private Vector3 movePosition = Vector3.zero;

    private float limitX = 0f;
    private int maxIndex = 0;
    #endregion

    #region CONSTANTS_FIELDS
    private const string speedKey = "speed";
    private const string deadKey = "dead";
    #endregion

    #region PROPERTIES
    public Vector2Int Index { get => index; set => index = value; }
    public int GenerationCount { get => generationCount; set => generationCount = value; }

    public int FoodsConsumed { get => foodsConsumed; }
    public TEAM Team { get => team; }
    public bool ToStay { get => toStay; }
    public bool Dead { get => dead; }
    #endregion

    #region PUBLIC_METHODS
    public void Init(float unit, int size, TEAM team)
    {
        this.unit = unit;
        this.team = team;

        limitX = size / 2f * unit;
        maxIndex = size;

        generationCount = 0;

        SetView(team);
    }

    public void Move(float lerp)
    {
        if (dead) return;

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
    }

    public void StopMovement()
    {
        animator.SetFloat(speedKey, 0f);
    }

    public void ResetData()
    {
        moveIndex = index;

        startPosition = transform.position;
        movePosition = transform.position;

        generationCount++;

        OnReset();
    }

    public void ConsumeFood()
    {
        UpdateFitness(index == nearFood.Index ? consumeNearFoodFitness : consumeFoodFitness);

        foodsConsumed++;

        PopulationManager.Instance.AddFoodsConsumed(team);
    }

    public void Death()
    {
        dead = true;

        animator.SetTrigger(deadKey);

        PopulationManager.Instance.AddDeaths(team);
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

        if (outputs != null && outputs.Length >= 3)
        {
            bool vertical = outputs[0] < 0.5f;
            float positive = outputs[1] < 0.5f ? -1f : 1f;

            Vector3 dir = new Vector3(vertical ? positive : 0f, 0f, !vertical ? positive : 0f);
            movePosition = transform.position + dir * unit;
            transform.forward = dir;

            moveIndex = index + new Vector2Int((int)dir.x, (int)dir.z);

            toStay = outputs[2] < 0.5f;

            UpdatePositionLimit();
            UpdateIndexLimit();

            if (CheckOutLimitY())
            {
                UpdateFitness(outLimitYFitness);
            }
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

    private void UpdatePositionLimit()
    {
        Vector3 pos = movePosition;
        if (pos.x > limitX)
        {
            pos.x -= limitX * 2;
            startPosition = pos - new Vector3(unit, 0f, 0f);
        }
        else if (pos.x < -limitX)
        {
            pos.x += limitX * 2;
            startPosition = pos + new Vector3(unit, 0f, 0f);
        }
        movePosition = pos;
    }

    private void UpdateIndexLimit()
    {
        Vector2Int auxIndex = moveIndex;
        if (auxIndex.x > maxIndex)
        {
            auxIndex.x = 0;
        }
        else if (auxIndex.x < 0)
        {
            auxIndex.x = maxIndex;
        }
        moveIndex = auxIndex;
    }

    private bool CheckOutLimitY()
    {
        return moveIndex.y < 0 || moveIndex.y > maxIndex;
    }
    #endregion
}