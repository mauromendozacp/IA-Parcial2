using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class GameplaySimulationView : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [Header("General Settings")]
    [SerializeField] private GameObject holder = null;
    [SerializeField] private GameObject agentHolder = null;
    [SerializeField] private GameObject gameplayHolder = null;
    [SerializeField] private TMP_Text generationsCountTxt = null;
    [SerializeField] private TMP_Text turnsLeftTxt = null;
    [SerializeField] private TMP_Text bestFitnessTxt = null;
    [SerializeField] private TMP_Text avgFitnessTxt = null;
    [SerializeField] private TMP_Text worstFitnessTxt = null;
    [SerializeField] private TMP_Text timerTxt = null;
    [SerializeField] private Slider timerSlider = null;

    [Header("Chaimbot Info Settings")]
    [SerializeField] private TMP_Text agentNroTxt = null;
    [SerializeField] private TMP_Text agentTeamTxt = null;
    [SerializeField] private TMP_Text agentFitnessTxt = null;
    [SerializeField] private TMP_Text foodsConsumedTxt = null;
    [SerializeField] private TMP_Text rowTxt = null;
    [SerializeField] private TMP_Text columnTxt = null;

    [Header("Gameplay Info Settings")]
    [SerializeField] private TMP_Text totalChaimbotsTxt = null;
    [SerializeField] private TMP_Text totalFoodsTxt = null;
    [SerializeField] private TMP_Text chaimbotsATxt = null;
    [SerializeField] private TMP_Text foodsATxt = null;
    [SerializeField] private TMP_Text chaimbotsBTxt = null;
    [SerializeField] private TMP_Text foodsBTxt = null;

    [Header("Buttons Settings")]
    [SerializeField] private Button pauseBtn = null;
    [SerializeField] private Button saveBtn = null;
    [SerializeField] private Button stopBtn = null;
    [SerializeField] private Button exitBtn = null;

    [SerializeField] private Button lockedBtn = null;
    [SerializeField] private Button freeBtn = null;
    [SerializeField] private Button previusBtn = null;
    [SerializeField] private Button nextBtn = null;
    #endregion

    #region PRIVATE_FIELDS
    private string generationsCountText = string.Empty;
    private string turnsLeftText = string.Empty;
    private string bestFitnessText = string.Empty;
    private string avgFitnessText = string.Empty;
    private string worstFitnessText = string.Empty;
    private string timerText = string.Empty;

    private string agentNroText = string.Empty;
    private string agentTeamText = string.Empty;
    private string agentFitnessText = string.Empty;
    private string foodsConsumedText = string.Empty;
    private string rowText = string.Empty;
    private string columnText = string.Empty;

    private string totalChaimbotsText = string.Empty;
    private string totalFoodsText = string.Empty;
    private string chaimbotsAText = string.Empty;
    private string foodsAText = string.Empty;
    private string chaimbotsBText = string.Empty;
    private string foodsBText = string.Empty;
    #endregion

    #region UNITY_CALLS
    private void LateUpdate()
    {
        timerTxt.text = string.Format(timerText, PopulationManager.Instance.IterationCount);
        generationsCountTxt.text = string.Format(generationsCountText, PopulationManager.Instance.generation);
        turnsLeftTxt.text = string.Format(turnsLeftText, PopulationManager.Instance.turnsLeft);
        bestFitnessTxt.text = string.Format(bestFitnessText, PopulationManager.Instance.bestFitness);
        avgFitnessTxt.text = string.Format(avgFitnessText, PopulationManager.Instance.avgFitness);
        worstFitnessTxt.text = string.Format(worstFitnessText, PopulationManager.Instance.worstFitness);

        agentNroTxt.text = string.Format(agentNroText, PopulationManager.Instance.agentNro);
        agentTeamTxt.text = string.Format(agentTeamText, PopulationManager.Instance.agentTeam);
        agentFitnessTxt.text = string.Format(agentFitnessText, PopulationManager.Instance.agentFitness);
        foodsConsumedTxt.text = string.Format(foodsConsumedText, PopulationManager.Instance.foodsConsumed);
        rowTxt.text = string.Format(rowText, PopulationManager.Instance.row);
        columnTxt.text = string.Format(columnText, PopulationManager.Instance.column);

        totalChaimbotsTxt.text = string.Format(totalChaimbotsText, PopulationManager.Instance.totalChaimbots);
        totalFoodsTxt.text = string.Format(totalFoodsText, PopulationManager.Instance.totalFoods);
        chaimbotsATxt.text = string.Format(chaimbotsAText, PopulationManager.Instance.chaimbotsA);
        foodsATxt.text = string.Format(foodsAText, PopulationManager.Instance.foodsA);
        chaimbotsBTxt.text = string.Format(chaimbotsBText, PopulationManager.Instance.chaimbotsB);
        foodsBTxt.text = string.Format(foodsBText, PopulationManager.Instance.foodsB);
    }
    #endregion

    #region PUBLIC_METHODS
    public void Init(Action onPauseGame, Action onStopSimulation, Action onExitGame, Action onLockedCamera, Action<bool> onFollowCamera, Action onFreeCamera)
    {
        timerSlider.onValueChanged.AddListener(OnTimerChange);
        timerText = timerTxt.text;

        generationsCountText = generationsCountTxt.text;
        turnsLeftText = turnsLeftTxt.text;
        bestFitnessText = bestFitnessTxt.text;
        avgFitnessText = avgFitnessTxt.text;
        worstFitnessText = worstFitnessTxt.text;

        agentNroText = agentNroTxt.text;
        agentTeamText = agentTeamTxt.text;
        agentFitnessText = agentFitnessTxt.text;
        foodsConsumedText = foodsConsumedTxt.text;
        rowText = rowTxt.text;
        columnText = columnTxt.text;

        totalChaimbotsText = totalChaimbotsTxt.text;
        totalFoodsText = totalFoodsTxt.text;
        chaimbotsAText = chaimbotsATxt.text;
        foodsAText = foodsATxt.text;
        chaimbotsBText = chaimbotsBTxt.text;
        foodsBText = foodsBTxt.text;

        pauseBtn.onClick.AddListener(() => onPauseGame?.Invoke());
        saveBtn.onClick.AddListener(() => PopulationManager.Instance.SaveData());
        stopBtn.onClick.AddListener(() => onStopSimulation?.Invoke());
        exitBtn.onClick.AddListener(() => onExitGame?.Invoke());

        lockedBtn.onClick.AddListener(() => onLockedCamera?.Invoke());
        freeBtn.onClick.AddListener(() => onFreeCamera?.Invoke());
        previusBtn.onClick.AddListener(() => onFollowCamera?.Invoke(false));
        nextBtn.onClick.AddListener(() => onFollowCamera?.Invoke(true));
    }

    public void ToggleSaveButtonStatus(bool status)
    {
        saveBtn.interactable = status;
    }

    public void Toggle(bool status)
    {
        holder.SetActive(status);
    }

    public void ToggleAgentInfoStatus(bool status)
    {
        agentHolder.SetActive(status);
    }

    public void ToggleGameplayInfoStatus(bool status)
    {
        gameplayHolder.SetActive(status);
    }
    #endregion

    #region PRIVATE_METHODS
    void OnTimerChange(float value)
    {
        PopulationManager.Instance.IterationCount = (int)value;
        timerTxt.text = string.Format(timerText, PopulationManager.Instance.IterationCount);
    }
    #endregion
}