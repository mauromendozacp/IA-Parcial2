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

    #region CONSTANTS
    private const int probabilityToDie = 75;
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

        if (CheckEndGame()) isRunning = false;

        for (int i = 0; i < Mathf.Clamp(PopulationManager.Instance.IterationCount / 100.0f * 50f, 1f, 50f); i++)
        {
            turnsTimer += Time.deltaTime;

            UpdateMoveChaimbots(turnsTimer / turnsDelay);

            if (turnsTimer > turnsDelay)
            {
                turnsTimer = 0f;

                ProcessChaimbots();
                ProcessChaimbotsInSameIndex();

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

        ProcessChaimbotsNextGeneration();

        PopulationManager.Instance.Epoch(chaimbots, SpawnNewChaimbots);

        SetChaimbotsPositions();
        ProcessChaimbots();

        LockedCamera();
    }

    private void UpdateMoveChaimbots(float lerp)
    {
        foreach (Chaimbot chaimbot in chaimbots)
        {
            if (!CheckLimitY(chaimbot.Index.y)) continue;

            chaimbot.Move(lerp);
        }
    }

    private void SetChaimbotsPositions()
    {
        Vector3 startPosition = new Vector3(-size / 2f, 0f, -size / 2f);
        int aIndexX = 0;
        int aIndexY = 0;
        int bIndexX = size;
        int bIndexY = size;

        for (int i = 0; i < chaimbots.Count; i++)
        {
            Vector2Int index;

            if (chaimbots[i].Team == TEAM.A)
            {
                index = new Vector2Int(aIndexX, aIndexY);
                aIndexX++;

                if (aIndexX > size)
                {
                    aIndexX = 0;
                    aIndexY++;
                }
            }
            else
            {
                index = new Vector2Int(bIndexX, bIndexY);
                bIndexX--;

                if (bIndexX < 0)
                {
                    bIndexX = size;
                    bIndexY--;
                }
            }

            chaimbots[i].Index = index;
            chaimbots[i].transform.position = (startPosition + new Vector3(index.x, 0f, index.y)) * unit;
            chaimbots[i].ResetData();
        }
    }

    private Food GetNearFood(Vector3 position)
    {
        Food nearest = null;

        if (foods.Count > 0)
        {
            nearest = foods[0];
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
        }

        return nearest;
    }

    #region PROCESS
    private void ProcessChaimbots()
    {
        for (int i = 0; i < chaimbots.Count; i++)
        {
            if (!CheckLimitY(chaimbots[i].Index.y) || chaimbots[i].Dead) continue;

            chaimbots[i].SetNearFood(GetNearFood(chaimbots[i].transform.position));
            chaimbots[i].Think();
        }
    }

    private void ProcessChaimbotsInSameIndex()
    {
        Dictionary<Vector2Int, List<Chaimbot>> chaimbotsSameIndex = new Dictionary<Vector2Int, List<Chaimbot>>();
        for (int i = 0; i < chaimbots.Count; i++)
        {
            if (chaimbots[i].Dead || chaimbotsSameIndex.ContainsKey(chaimbots[i].Index)) continue;
            bool inFoodIndex = CheckIndexInFood(chaimbots[i].Index);

            for (int j = 0; j < chaimbots.Count; j++)
            {
                if (i == j || chaimbots[j].Dead) continue;

                if (chaimbots[i].Index == chaimbots[j].Index || inFoodIndex)
                {
                    if (!chaimbotsSameIndex.ContainsKey(chaimbots[i].Index))
                    {
                        chaimbotsSameIndex.Add(chaimbots[i].Index, new List<Chaimbot>() { chaimbots[i] });
                    }

                    if (!inFoodIndex)
                    {
                        chaimbotsSameIndex[chaimbots[i].Index].Add(chaimbots[j]);
                    }
                }
            }
        }

        foreach (KeyValuePair<Vector2Int, List<Chaimbot>> entry in chaimbotsSameIndex)
        {
            if (CheckIndexInFood(entry.Key))
            {
                List<Chaimbot> eatingChaimbots = entry.Value;
                bool foodConsumed = false;

                if (eatingChaimbots.Count > 1)
                {
                    eatingChaimbots.RemoveAll(c => !c.ToStay);

                    if (eatingChaimbots.Count > 0)
                    {
                        if (!CheckSameTeamsInList(eatingChaimbots))
                        {
                            TEAM executeTeam = GetRandomTeam();

                            for (int i = 0; i < eatingChaimbots.Count; i++)
                            {
                                if (eatingChaimbots[i].Team == executeTeam)
                                {
                                    eatingChaimbots[i].Death();
                                }
                            }

                            eatingChaimbots.RemoveAll(c => c.Team == executeTeam);
                        }

                        int chaimbotEatingIndex = 0;
                        if (eatingChaimbots.Count > 1)
                        {
                            chaimbotEatingIndex = Random.Range(0, eatingChaimbots.Count);

                            for (int i = 0; i < eatingChaimbots.Count; i++)
                            {
                                if (i != chaimbotEatingIndex)
                                {
                                    //hacer que se mueva
                                }
                            }
                        }

                        eatingChaimbots[chaimbotEatingIndex].ConsumeFood();
                        foodConsumed = true;
                    }
                }
                else
                {
                    eatingChaimbots[0].ConsumeFood();
                    foodConsumed = true;
                }

                if (foodConsumed)
                {
                    Food food = foods.Find(f => f.Index == entry.Key);
                    if (food != null)
                    {
                        Destroy(food.gameObject);
                        foods.Remove(food);
                    }
                }
            }
            else
            {
                List<Chaimbot> chaimbotsCowards = entry.Value.FindAll(c => !c.ToStay);
                if (!CheckSameTeamsInList(chaimbotsCowards))
                {
                    for (int i = 0; i < chaimbotsCowards.Count; i++)
                    {
                        int prob = Random.Range(0, 101);
                        if (prob < probabilityToDie)
                        {
                            chaimbotsCowards[i].Death();
                        }
                    }
                }

                entry.Value.RemoveAll(c => !c.ToStay);
                if (entry.Value.Count > 1 && !CheckSameTeamsInList(entry.Value))
                {
                    TEAM executeTeam = GetRandomTeam();

                    for (int i = 0; i < entry.Value.Count; i++)
                    {
                        if (entry.Value[i].Team == executeTeam)
                        {
                            entry.Value[i].Death();
                        }
                    }
                }
            }
        }
    }

    private void ProcessChaimbotsNextGeneration()
    {
        for (int i = 0; i < chaimbots.Count; i++)
        {
            chaimbots[i].GenerationCount++;
        }
    }
    #endregion

    #region RANDOM
    private Vector2Int GetRandomIndex(params Vector2Int[] usedIndexs)
    {
        Vector2Int index;
        bool repeat;

        do
        {
            repeat = false;

            int x = Random.Range(0, size);
            int y = Random.Range(1, size - 1);
            index = new Vector2Int(x, y);

            if (usedIndexs != null && usedIndexs.Length > 0)
            {
                repeat = usedIndexs.Contains(index);
            }

        } while (repeat);

        return index;
    }

    private TEAM GetRandomTeam()
    {
        return (TEAM)Random.Range((int)TEAM.NONE + 1, (int)TEAM.COUNT) + 1;
    }
    #endregion

    #region SPAWN
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
            chaimbot.Init(unit, size, TEAM.A);

            chaimbots.Add(chaimbot);
        }

        for (int i = 0; i < totalChaimbotsB; i++)
        {
            GameObject chaimbotGO = Instantiate(chaimbotPrefab, chaimbotHolder);
            Chaimbot chaimbot = chaimbotGO.GetComponent<Chaimbot>();
            chaimbot.Init(unit, size, TEAM.B);

            chaimbots.Add(chaimbot);
        }
    }

    private void SpawnNewChaimbots(Genome[] newGenomes, NeuralNetwork[] brains, TEAM team)
    {
        for (int i = 0; i < newGenomes.Length; i++)
        {
            GameObject chaimbotGO = Instantiate(chaimbotPrefab, chaimbotHolder);
            Chaimbot chaimbot = chaimbotGO.GetComponent<Chaimbot>();
            chaimbot.Init(unit, size, team);

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
    #endregion

    #region DESPAWN
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
    #endregion

    #region CHECKS
    private bool CheckLimitY(int posY)
    {
        return posY >= 0 && posY <= size;
    }

    private bool CheckEndGame()
    {
        return chaimbots.Count == 0 || foods.Count == 0;
    }

    private bool CheckIndexInFood(Vector2Int checkIndex)
    {
        for (int i = 0; i < foods.Count; i++)
        {
            if (foods[i].Index == checkIndex)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckSameTeamsInList(List<Chaimbot> auxChaimbots)
    {
        if (auxChaimbots.Count > 0)
        {
            TEAM team = auxChaimbots[0].Team;

            for (int i = 1; i < auxChaimbots.Count; i++)
            {
                if (auxChaimbots[i].Team != team)
                {
                    return false;
                }
            }
        }

        return true;
    }
    #endregion

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
            PopulationManager.Instance.agentGeneration = followChaimbot.GenerationCount;

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

        DestroyChaimbots();
        DestroyFoods();

        LockedCamera();

        startView.Toggle(true);
        gameplayView.Toggle(false);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    #endregion
}