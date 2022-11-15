using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class GameplayController : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [Header("General Settings")]
    [SerializeField] private CameraController cameraController = null;
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

    private int size = 0;
    private int foodSize = 0;

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
        gameplayView.ToggleSaveButtonStatus(!dataLoaded);

        startView.Toggle(false);
        gameplayView.Toggle(true);

        size = PopulationManager.Instance.PopulationCount;
        foodSize = size * 2;

        SpawnChaimbots(dataLoaded);
        SpawnFoods();

        PopulationManager.Instance.StartSimulation(chaimbots, dataLoaded);

        SetChaimbotsPositions();
        ProcessChaimbots();

        isRunning = true;
        isLoop = dataLoaded;
        currentFollowAgent = -1;
    }

    private void ResetSimulation()
    {
        DestroyFoods();
        SpawnFoods();

        PopulationManager.Instance.Epoch(chaimbots, SpawnNewChaimbots);

        SetChaimbotsPositions();
        ProcessChaimbots();
    }

    private void UpdateMoveChaimbots(float lerp)
    {
        foreach (Chaimbot chaimbot in chaimbots)
        {
            if (!CheckLimitY(chaimbot.Index.y)) continue;

            chaimbot.Move(lerp);
        }
    }

    private void ProcessChaimbots()
    {
        foreach (Chaimbot chaimbot in chaimbots)
        {
            if (!CheckLimitY(chaimbot.Index.y)) continue;

            chaimbot.SetNearFood(GetNearFood(chaimbot.transform.position));

            float limitX = size / 2f * unit;
            Vector3 pos = chaimbot.MovePosition;
            if (pos.x > limitX)
            {
                pos.x -= limitX * 2;
            }
            else if (pos.x < -limitX)
            {
                pos.x += limitX * 2;
            }
            chaimbot.MovePosition = pos;

            Vector2Int index = chaimbot.MoveIndex;
            if (index.x > size)
            {
                index.x = 0;
            }
            else if (index.x < 0)
            {
                index.x = size;
            }
            chaimbot.MoveIndex = index;

            chaimbot.Think();
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
    
    private void SpawnChaimbots(bool dataLoaded)
    {
        int totalChaimbotsA = PopulationManager.Instance.PopulationCount;
        int totalChaimbotsB = PopulationManager.Instance.PopulationCount;

        if (dataLoaded)
        {
            totalChaimbotsA = PopulationManager.Instance.GetPopulationACount();
            totalChaimbotsB = PopulationManager.Instance.GetPopulationBCount();
        }

        for (int i = 0; i < totalChaimbotsA; i++)
        {
            GameObject chaimbotGO = Instantiate(chaimbotPrefab, chaimbotHolder);
            Chaimbot chaimbot = chaimbotGO.GetComponent<Chaimbot>();
            chaimbot.Init(unit, TEAM.A);

            chaimbots.Add(chaimbot);
        }

        for (int i = 0; i < totalChaimbotsB; i++)
        {
            GameObject chaimbotGO = Instantiate(chaimbotPrefab, chaimbotHolder);
            Chaimbot chaimbot = chaimbotGO.GetComponent<Chaimbot>();
            chaimbot.Init(unit, TEAM.B);

            chaimbots.Add(chaimbot);
        }
    }

    private void SpawnNewChaimbots(Genome[] newGenomes, NeuralNetwork[] brains, TEAM team)
    {
        for (int i = 0; i < newGenomes.Length; i++)
        {
            GameObject chaimbotGO = Instantiate(chaimbotPrefab, chaimbotHolder);
            Chaimbot chaimbot = chaimbotGO.GetComponent<Chaimbot>();
            chaimbot.Init(unit, team);

            NeuralNetwork brain = brains[i];
            brain.SetWeights(newGenomes[i].genome);
            chaimbot.SetBrain(newGenomes[i], brain);

            chaimbots.Add(chaimbot);
        }
    }

    private void SpawnFoods()
    {
        List<Vector2Int> foodUsedIndexs = new List<Vector2Int>();

        for (int i = 0; i < foodSize; i++)
        {
            GameObject foodGO = Instantiate(foodPrefab, foodHolder);
            Food food = foodGO.GetComponent<Food>();

            int modelIndex = Random.Range(0, foodModels.Length);

            Vector2Int foodIndex = GetRandomIndex(foodUsedIndexs.ToArray());

            Vector3 startPosition = new Vector3(-size / 2f, startPosY, -size / 2f);
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
        Vector3 startPosition = new Vector3(-size / 2, 0f, -size / 2);
        int aIndex = 0;
        int bIndex = 0;

        for (int i = 0; i < chaimbots.Count; i++)
        {
            Vector2Int index;

            if (chaimbots[i].Team == TEAM.A)
            {
                index = new Vector2Int(aIndex, 0);
                aIndex++;
            }
            else
            {
                index = new Vector2Int(bIndex - chaimbots.Count / 2, size);
                bIndex++;
            }

            chaimbots[i].Index = index;
            chaimbots[i].transform.position = (startPosition + new Vector3(index.x, 0f, index.y)) * unit;
            chaimbots[i].ResetData();
        }
    }

    private Vector2Int GetRandomIndex(params Vector2Int[] usedIndexs)
    {
        Vector2Int index;
        bool repeat;

        do
        {
            repeat = false;

            int x = Random.Range(0, size);
            int y = Random.Range(0, size);
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
        return posY >= 0 && posY <= size;
    }

    #region CAMERA
    private void LockedCamera()
    {
        cameraController.SetMode(CAMERA_MODE.LOCKED);

        gameplayView.ToggleGameplayInfoStatus(true);
        gameplayView.ToggleAgentInfoStatus(false);
    }

    private void FollowCamera(bool increment)
    {
        if (chaimbots.Count > 0)
        {
            currentFollowAgent += increment ? 1 : -1;

            if (currentFollowAgent >= chaimbots.Count)
            {
                currentFollowAgent = 0;
            }
            if (currentFollowAgent < 0)
            {
                currentFollowAgent = chaimbots.Count - 1;
            }

            Chaimbot followChaimbot = chaimbots[currentFollowAgent];
            PopulationManager.Instance.agentNro = currentFollowAgent;
            PopulationManager.Instance.agentTeam = followChaimbot.Team.ToString();

            cameraController.SetFollowAgent(followChaimbot);
            cameraController.SetMode(CAMERA_MODE.FOLLOW);

            gameplayView.ToggleGameplayInfoStatus(false);
            gameplayView.ToggleAgentInfoStatus(true);
        }
        else
        {
            LockedCamera();
        }
    }

    private void FreeCamera()
    {
        cameraController.SetMode(CAMERA_MODE.FREE);

        gameplayView.ToggleGameplayInfoStatus(true);
        gameplayView.ToggleAgentInfoStatus(false);
    }
    #endregion

    #region BUTTONS_CALLBACKS
    private void PauseGame()
    {
        isRunning = !isRunning;

        if (!isRunning)
        {
            for (int i = 0; i < chaimbots.Count; i++)
            {
                chaimbots[i].StopMovement();
            }
        }
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