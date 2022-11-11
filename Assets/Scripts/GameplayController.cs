using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class GameplayController : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [Header("General Settings")]
    [SerializeField] private Vector2Int size = Vector2Int.zero;
    [SerializeField] private float unit = 0f;
    [SerializeField] private float turnsDelay = 0f;

    [Header("Chaimbot Settings")]
    [SerializeField] private GameObject chaimbotPrefab = null;
    [SerializeField] private Transform chaimbotHolder = null;

    [Header("Food Settings")]
    [SerializeField] private GameObject foodPrefab = null;
    [SerializeField] private Transform foodHolder = null;
    [SerializeField] private float startPosY = 0f;
    [SerializeField] private FoodModel[] foodModels = null;

    [Header("UI Settings")]
    [SerializeField] private StartSimulationView startView = null;
    [SerializeField] private GameplaySimulationView gameplayView = null;
    #endregion

    #region PRIVATE_FIELDS
    private List<Chaimbot> chaimbots = new List<Chaimbot>();
    private List<Food> foods = new List<Food>();

    private float turnsTimer = 0f;
    private bool isRunning = false;
    #endregion

    #region UNITY_CALLS
    private void Start()
    {
        startView.Init(StartGame);
        gameplayView.Init(PauseGame, StopSimulation, ExitGame);

        startView.Toggle(true);
        gameplayView.Toggle(false);
    }

    private void Update()
    {
        if (!isRunning) return;

        for (int i = 0; i < Mathf.Clamp(PopulationManager.Instance.IterationCount / 100.0f * 50f, 1f, 50f); i++)
        {
            turnsTimer += Time.deltaTime;

            foreach (Chaimbot chaimbot in chaimbots)
            {
                chaimbot.Move(turnsTimer / turnsDelay);
            }

            if (turnsTimer > turnsDelay)
            {
                turnsTimer = 0f;

                ProcessChaimbots();

                PopulationManager.Instance.turnsLeft--;
                if (PopulationManager.Instance.turnsLeft <= 0)
                {
                    PopulationManager.Instance.Epoch(chaimbots.ToArray());
                    SetChaimbotsPositions();
                }
            }
        }
    }
    #endregion

    #region PRIVATE_METHODS
    private void StartGame()
    {
        startView.Toggle(false);
        gameplayView.Toggle(true);

        SpawnChaimbots();
        SpawnFoods();

        PopulationManager.Instance.StartSimulation(chaimbots.ToArray());

        SetChaimbotsPositions();
        ProcessChaimbots();

        isRunning = true;
    }

    private void ProcessChaimbots()
    {
        foreach (Chaimbot chaimbot in chaimbots)
        {
            chaimbot.SetNearFood(GetNearFood(chaimbot.transform.position));

            chaimbot.OnThink();
        }
    }
    
    private void SpawnChaimbots()
    {
        for (int i = 0; i < PopulationManager.Instance.PopulationCount; i++)
        {
            GameObject chaimbotGO = Instantiate(chaimbotPrefab, chaimbotHolder);
            Chaimbot chaimbot = chaimbotGO.GetComponent<Chaimbot>();
            
            if (i < PopulationManager.Instance.PopulationCount / 2)
            {
                chaimbot.Init(unit, TEAM.A);
            }
            else
            {
                chaimbot.Init(unit, TEAM.B);
            }

            chaimbots.Add(chaimbot);
        }
    }

    private void SpawnFoods()
    {
        List<Vector2Int> foodUsedIndexs = new List<Vector2Int>();

        for (int i = 0; i < PopulationManager.Instance.PopulationCount; i++)
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

    private void DestroyChaimbots()
    {
        for (int i = 0; i < chaimbots.Count; i++)
        {
            Destroy(chaimbots[i].gameObject);
        }

        chaimbots.Clear();
    }

    private void DestroyFoods()
    {
        for (int i = 0; i < foods.Count; i++)
        {
            Destroy(foods[i].gameObject);
        }

        foods.Clear();
    }

    private void SetChaimbotsPositions()
    {
        Vector3 startPosition = new Vector3(-size.x / 2, 0f, -size.y / 2);

        for (int i = 0; i < chaimbots.Count; i++)
        {
            Vector2Int index;

            if (i < chaimbots.Count / 2)
            {
                index = new Vector2Int(i * 2, 0);

                chaimbots[i].transform.position = (startPosition + new Vector3(index.x, 0f, index.y)) * unit;
                chaimbots[i].transform.forward = transform.forward;
            }
            else
            {
                index = new Vector2Int((i - chaimbots.Count / 2) * 2, size.y);

                chaimbots[i].transform.position = (startPosition + new Vector3(index.x, 0f, index.y)) * unit;
                chaimbots[i].transform.forward = -transform.forward;
            }

            chaimbots[i].SetIndex(index);
            chaimbots[i].ResetPositions();
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

    private Food GetNearFood(Vector3 position)
    {
        Food nearest = foods[0];
        float distance = (position - nearest.transform.position).sqrMagnitude;

        foreach (Food food in foods)
        {
            float newDist = (food.transform.position - position).sqrMagnitude;
            if (newDist < distance)
            {
                nearest = food;
                distance = newDist;
            }
        }

        return nearest;
    }

    private void PauseGame()
    {
        isRunning = !isRunning;
    }

    private void StopSimulation()
    {
        startView.Toggle(true);
        gameplayView.Toggle(false);

        DestroyChaimbots();
        DestroyFoods();
    }

    private void ExitGame()
    {
        Application.Quit();
    }
    #endregion
}