using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class GameplayController : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [Header("General Settings")]
    [SerializeField] private CameraController cameraController = null;
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
    private bool isLoop = false;
    private int currentFollowAgent = 0;
    #endregion

    #region UNITY_CALLS
    private void Start()
    {
        startView.Init(StartGame);
        gameplayView.Init(PauseGame, StopSimulation, ExitGame, LockedCamera, FollowCamera, FreeCamera);

        startView.Toggle(true);
        gameplayView.Toggle(false);
    }

    private void Update()
    {
        if (!isRunning) return;

        for (int i = 0; i < Mathf.Clamp(PopulationManager.Instance.IterationCount / 100.0f * 50f, 1f, 50f); i++)
        {
            turnsTimer += Time.deltaTime;

            UpdateMoveChaimbots(turnsTimer / turnsDelay);

            if (turnsTimer > turnsDelay)
            {
                turnsTimer = 0f;

                ProcessChaimbots();
                ProcessFoods();

                PopulationManager.Instance.turnsLeft += isLoop ? 1 : -1;

                if (PopulationManager.Instance.turnsLeft <= 0 && !isLoop)
                {
                    ResetSimulation();
                }
            }
        }
    }
    #endregion

    #region PRIVATE_METHODS
    private void StartGame(bool dataLoaded)
    {
        startView.Toggle(false);
        gameplayView.Toggle(true);

        SpawnChaimbots();
        SpawnFoods();

        List<Agent> agents = new List<Agent>();
        agents.AddRange(chaimbots);
        PopulationManager.Instance.StartSimulation(agents, dataLoaded);

        SetChaimbotsPositions();
        ProcessChaimbots();

        isRunning = true;
        isLoop = dataLoaded;
        currentFollowAgent = 0;
    }

    private void ResetSimulation()
    {
        DestroyFoods();
        SpawnFoods();

        List<Agent> agents = new List<Agent>();
        agents.AddRange(chaimbots);
        PopulationManager.Instance.Epoch(agents);

        SetChaimbotsPositions();
        ProcessChaimbots();
    }

    private void UpdateMoveChaimbots(float lerp)
    {
        foreach (Chaimbot chaimbot in chaimbots)
        {
            if (!CheckLimitY(chaimbot.Index.y)) continue;

            chaimbot.Move(lerp);

            float limitX = size.x / 2f * unit;
            Vector3 pos = chaimbot.transform.position;

            if (pos.x > limitX)
            {
                pos.x -= limitX * 2;
            }
            else if (pos.x < -limitX)
            {
                pos.x += limitX * 2;
            }

            chaimbot.transform.position = pos;
        }
    }

    private void ProcessChaimbots()
    {
        foreach (Chaimbot chaimbot in chaimbots)
        {
            if (!CheckLimitY(chaimbot.Index.y)) continue;

            chaimbot.SetNearFood(GetNearFood(chaimbot.transform.position));

            chaimbot.Think();

            Vector2Int index = chaimbot.Index;
            if (index.x > size.x)
            {
                index.x = 0;
            }
            else if (index.x < 0)
            {
                index.x = size.x;
            }
            chaimbot.Index = index;
        }
    }

    private void ProcessFoods()
    {
        for (int i = 0; i < foods.Count; i++)
        {
            bool foodConsumed = false;

            for (int j = 0; j < chaimbots.Count; j++)
            {
                if (foods[i].Index == chaimbots[j].Index)
                {
                    foodConsumed = true;
                    break;
                }
            }

            if (foodConsumed)
            {
                Destroy(foods[i].gameObject);
                foods.RemoveAt(i);
                i--;
            }
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

            chaimbots[i].Index = index;
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

    private bool CheckLimitY(int posY)
    {
        return posY >= 0 && posY <= size.y;
    }

    #region CAMERA
    private void LockedCamera()
    {
        cameraController.SetMode(CAMERA_MODE.LOCKED);
    }

    private void FollowCamera(bool increment)
    {
        if (chaimbots.Count > 0)
        {
            if (currentFollowAgent >= chaimbots.Count)
            {
                currentFollowAgent = 0;
            }
            if (currentFollowAgent < 0)
            {
                currentFollowAgent = chaimbots.Count - 1;
            }

            Agent followAgent = chaimbots[currentFollowAgent];
            cameraController.SetFollowAgent(followAgent);
            cameraController.SetMode(CAMERA_MODE.FOLLOW);

            currentFollowAgent += increment ? 1 : -1;
        }
        else
        {
            LockedCamera();
        }
    }

    private void FreeCamera()
    {
        cameraController.SetMode(CAMERA_MODE.FREE);
    }
    #endregion

    #region BUTTONS_CALLBACKS
    private void PauseGame()
    {
        isRunning = !isRunning;
    }

    private void StopSimulation()
    {
        isRunning = false;

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
    #endregion
}