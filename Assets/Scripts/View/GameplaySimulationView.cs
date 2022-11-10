using UnityEngine;
using UnityEngine.UI;

using UnityEditor;

using TMPro;
using System;

public class GameplaySimulationView : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [Header("General Settings")]
    [SerializeField] private GameObject holder = null;
    [SerializeField] private TMP_Text generationsCountTxt = null;
    [SerializeField] private TMP_Text bestFitnessTxt = null;
    [SerializeField] private TMP_Text avgFitnessTxt = null;
    [SerializeField] private TMP_Text worstFitnessTxt = null;
    [SerializeField] private TMP_Text timerTxt = null;
    [SerializeField] private Slider timerSlider = null;

    [Header("Buttons Settings")]
    [SerializeField] private Button pauseBtn = null;
    [SerializeField] private Button saveBtn = null;
    [SerializeField] private Button stopBtn = null;
    [SerializeField] private Button exitBtn = null;
    #endregion

    #region PRIVATE_FIELDS
    private string generationsCountText = string.Empty;
    private string bestFitnessText = string.Empty;
    private string avgFitnessText = string.Empty;
    private string worstFitnessText = string.Empty;
    private string timerText = string.Empty;
    #endregion

    #region UNITY_CALLS
    private void LateUpdate()
    {
        timerTxt.text = string.Format(timerText, PopulationManager.Instance.IterationCount);
        generationsCountTxt.text = string.Format(generationsCountText, PopulationManager.Instance.generation);
        bestFitnessTxt.text = string.Format(bestFitnessText, PopulationManager.Instance.GetBestFitness());
        avgFitnessTxt.text = string.Format(avgFitnessText, PopulationManager.Instance.GetAvgFitness());
        worstFitnessTxt.text = string.Format(worstFitnessText, PopulationManager.Instance.GetWorstFitness());
    }
    #endregion

    #region PUBLIC_METHODS
    public void Init(Action onPauseGame, Action onStopSimulation, Action onExitGame)
    {
        timerSlider.onValueChanged.AddListener(OnTimerChange);
        timerText = timerTxt.text;

        generationsCountText = generationsCountTxt.text;
        bestFitnessText = bestFitnessTxt.text;
        avgFitnessText = avgFitnessTxt.text;
        worstFitnessText = worstFitnessTxt.text;

        pauseBtn.onClick.AddListener(() => onPauseGame?.Invoke());
        saveBtn.onClick.AddListener(() => PopulationManager.Instance.SaveData());
        stopBtn.onClick.AddListener(() => onStopSimulation?.Invoke());
        exitBtn.onClick.AddListener(() => onExitGame?.Invoke());
    }

    public void Toggle(bool status)
    {
        holder.SetActive(status);
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