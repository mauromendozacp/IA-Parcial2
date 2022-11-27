using System.Collections.Generic;

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

    private bool process = false;
    private bool dead = false;
    private bool stop = false;
    private bool toStay = false;
    private bool inOutLimit = false;
    private bool canEat = false;
    private bool canDie = false;
    private int steps = 0;

    private float lerp = 0f;
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

    #region PROPERTIES
    public Vector2Int Index { get => index; set => index = value; }
    public Vector2Int MoveIndex { get => moveIndex; set => moveIndex = value; }
    public Vector3 StartPosition { get => startPosition; set => startPosition = value; }
    public Vector3 MovePosition { get => movePosition; set => movePosition = value; }

    public bool ToStay { get => toStay; set => toStay = value; }
    public bool InOutLimit { get => inOutLimit; set => inOutLimit = value; }
    public int GenerationCount { get => generationCount; set => generationCount = value; }
    public bool Process { get => process; set => process = value; }
    public float Lerp { get => lerp; set => lerp = value; }
    public bool CanEat { get => canEat; set => canEat = value; }
    public bool CanDie { get => canDie; set => canDie = value; }
    public bool Dead { get => dead; set => dead = value; }
    public int Steps { get => steps; set => steps = value; }

    public bool Stop { get => stop; }
    public float Unit { get => unit; }
    public TEAM Team { get => team; }

    public int FoodsConsumed { get => foodsConsumed; set => foodsConsumed = value; }
    public Food NearFood { get => nearFood; }
    #endregion

    #region PUBLIC_METHODS
    public void Init(float unit, int size, TEAM team)
    {
        this.unit = unit;
        this.team = team;

        limitX = size / 2f * unit;
        maxIndex = size;

        generationCount = 1;

        SetView(team);

        base.Init();
    }

    public void ResetData()
    {
        moveIndex = index;

        startPosition = transform.position;
        movePosition = transform.position;

        lerp = 0f;
        foodsConsumed = 0;    
        process = true;
        toStay = false;
        inOutLimit = false;
        canEat = false;
        canDie = false;
        steps = 0;

        UpdateTree();
        OnReset();
    }

    public void SwitchMovement()
    {
        stop = !stop;

        UpdateTree();
    }

    public void SetNearFood(Food nearFood)
    {
        this.nearFood = nearFood;
    }
    #endregion

    #region OVERRIDE_METHODS
    protected override TreeNode Setup()
    {
        TreeNode root = new Root(new List<TreeNode>()
        {
            new Sequence(new List<TreeNode>()
            {
                new CheckDeadNode(new List<TreeNode>(), this),
                new CheckLimitYNode(new List<TreeNode>(), this, maxIndex),

                new Not(new List<TreeNode>()
                {
                    new Sequence(new List<TreeNode>()
                    {
                        new CanDieNode(new List<TreeNode>(), this),
                        new DieNode(new List<TreeNode>(), this, animator)
                    })
                }),

                new Not(new List<TreeNode>()
                {
                    new Sequence(new List<TreeNode>()
                    {
                        new CheckStopNode(new List<TreeNode>(), this),
                        new StopMovementNode(new List<TreeNode>(), animator)
                    })
                }),

                new Not(new List<TreeNode>()
                {
                    new Sequence(new List<TreeNode>()
                    {
                        new CanEatNode(new List<TreeNode>(), this),
                        new ConsumeFoodNode(new List<TreeNode>(), this, consumeNearFoodFitness, consumeFoodFitness)
                    })
                }),

                new Not(new List<TreeNode>()
                {
                    new Sequence(new List<TreeNode>()
                    {
                        new CheckProcessNode(new List<TreeNode>(), this),
                        new ProcessInputsNode(new List<TreeNode>(), this),

                        new Not(new List<TreeNode>()
                        {
                            new Sequence(new List<TreeNode>()
                            {
                                new CheckStayNode(new List<TreeNode>(), this),
                                new UpdateMovementNode(new List<TreeNode>(), this)
                            })
                        }),

                        new ProcessOutputsNode(new List<TreeNode>(), this),

                        new UpdatePositionLimitNode(new List<TreeNode>(), this, limitX, unit),
                        new UpdateIndexLimitNode(new List<TreeNode>(), this, maxIndex),
                    })
                }),

                new Not(new List<TreeNode>()
                {
                    new Sequence(new List<TreeNode>()
                    {
                        new Not(new List<TreeNode>()
                        {
                            new CheckLimitYNode(new List<TreeNode>(), this, maxIndex)
                        }),

                        new CheckStayNode(new List<TreeNode>(), this),
                        new OutLimitYNode(new List<TreeNode>(), this, outLimitYFitness)
                    })
                }),

                new Sequence(new List<TreeNode>()
                {
                    new CheckStayNode(new List<TreeNode>(), this),
                    new MovementLerpNode(new List<TreeNode>(), this),
                    new MovementAnimationNode(new List<TreeNode>(), this, animator, curve)
                })
            })
        });

        return root;
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
    #endregion
}