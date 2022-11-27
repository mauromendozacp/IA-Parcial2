using System.Collections.Generic;
using UnityEngine;

public class ConsumeFoodNode : TreeNode
{
    #region PRIVATE_FIELDS
    private Chaimbot chaimbot = null;
    private float consumeNearFoodFitness = 0f;
    private float consumeFoodFitness = 0f;
    #endregion

    #region CONSTRUCTORS
    public ConsumeFoodNode(List<TreeNode> childrens, Chaimbot chaimbot, float consumeNearFoodFitness, float consumeFoodFitness) : base(childrens)
    {
        this.chaimbot = chaimbot;
        this.consumeNearFoodFitness = consumeNearFoodFitness;
        this.consumeFoodFitness = consumeFoodFitness;
    }
    #endregion

    #region OVERRIDE_METHODS
    public override NodeState Evaluate()
    {
        bool isNearFood = chaimbot.NearFood != null && chaimbot.Index == chaimbot.NearFood.Index;
        chaimbot.UpdateFitness(isNearFood ? consumeNearFoodFitness : consumeFoodFitness);

        chaimbot.FoodsConsumed++;
        chaimbot.CanEat = false;
        chaimbot.Steps = 0;

        PopulationManager.Instance.AddFoodsConsumed(chaimbot.Team);

        return NodeState.RUNNING;
    }
    #endregion
}
