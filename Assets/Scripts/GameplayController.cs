using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [SerializeField] private Vector2Int size = Vector2Int.zero;
    [SerializeField] private float unit = 0f;
    [SerializeField] private float turnsDelay = 0f;

    [Header("Chaimbot Settings")]
    [SerializeField] private int chaimbotsCount = 0;
    [SerializeField] private GameObject chaimbotPrefab = null;
    [SerializeField] private Transform chaimbotHolder = null;

    [Header("Food Settings")]
    [SerializeField] private GameObject foodPrefab = null;
    [SerializeField] private Transform foodHolder = null;
    [SerializeField] private float startPosY = 0f;
    [SerializeField] private FoodModel[] foodModels = null;
    #endregion

    #region PRIVATE_FIELDS
    private List<Chaimbot> chaimbots = new List<Chaimbot>();
    private List<Food> foods = new List<Food>();

    private float turnsTimer = 0f;
    #endregion

    #region UNITY_CALLS
    private void Start()
    {
        SpawnChaimbots();
        SpawnFoods();
    }

    private void Update()
    {

    }
    #endregion

    #region PRIVATE_METHODS
    private void SpawnChaimbots()
    {
        Vector3 startPosition = new Vector3(-size.x / 2, 0f, -size.y / 2);

        for (int i = 0; i < chaimbotsCount; i++)
        {
            GameObject chaimbotGO = Instantiate(chaimbotPrefab, chaimbotHolder);
            Chaimbot chaimbot = chaimbotGO.GetComponent<Chaimbot>();
            Vector2Int index;
            
            if (i < chaimbotsCount / 2)
            {
                index = new Vector2Int(i * 2, 0);

                chaimbot.transform.position = (startPosition + new Vector3(index.x, 0f, index.y)) * unit;
                chaimbot.transform.forward = transform.forward;
                chaimbot.Init(unit, index, TEAM.MALE);
            }
            else
            {
                index = new Vector2Int((i - chaimbotsCount / 2) * 2, size.y);

                chaimbot.transform.position = (startPosition + new Vector3(index.x, 0f, index.y)) * unit;
                chaimbot.transform.forward = -transform.forward;
                chaimbot.Init(unit, index, TEAM.FEMALE);
            }

            chaimbots.Add(chaimbot);
        }
    }

    private void SpawnFoods()
    {
        List<Vector2Int> foodUsedIndexs = new List<Vector2Int>();

        for (int i = 0; i < chaimbotsCount; i++)
        {
            GameObject foodGO = Instantiate(foodPrefab, foodHolder);
            Food food = foodGO.GetComponent<Food>();

            int modelIndex = Random.Range(0, foodModels.Length);

            Vector2Int foodIndex = GetRandomIndex(foodUsedIndexs.ToArray());

            Vector3 startPosition = new Vector3(-size.x / 2f, startPosY, -size.y / 2f);
            food.transform.position = (startPosition + new Vector3(foodIndex.x, 0f, foodIndex.y)) * unit;
            food.Init(foodModels[modelIndex], foodIndex);

            foods.Add(food);
            foodUsedIndexs.Add(foodIndex);
        }
    }

    private Vector2Int GetRandomIndex(params Vector2Int[] usedIndexs)
    {
        Vector2Int index;
        bool repeat;

        do
        {
            repeat = false;

            int x = Random.Range(0, size.x);
            int y = Random.Range(0, size.y);
            index = new Vector2Int(x, y);

            if (usedIndexs != null && usedIndexs.Length > 0)
            {
                repeat = usedIndexs.Contains(index);
            }

        } while (repeat);

        return index;
    }
    #endregion
}
