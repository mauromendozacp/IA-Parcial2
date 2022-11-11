using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class StartSimulationView : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [SerializeField] private GameObject holder = null;

    [Header("Sliders Settings")]
    [SerializeField] private TMP_Text populationCountTxt = null;
    [SerializeField] private Slider populationCountSlider = null;
    [SerializeField] private TMP_Text turnsTxt = null;
    [SerializeField] private Slider turnsSlider = null;
    [SerializeField] private TMP_Text eliteCountTxt = null;
    [SerializeField] private Slider eliteCountSlider = null;
    [SerializeField] private TMP_Text mutationChanceTxt = null;
    [SerializeField] private Slider mutationChanceSlider = null;
    [SerializeField] private TMP_Text mutationRateTxt = null;
    [SerializeField] private Slider mutationRateSlider = null;
    [SerializeField] private TMP_Text hiddenLayersCountTxt = null;
    [SerializeField] private Slider hiddenLayersCountSlider = null;
    [SerializeField] private TMP_Text neuronsPerHLCountTxt = null;
    [SerializeField] private Slider neuronsPerHLSlider = null;
    [SerializeField] private TMP_Text biasTxt = null;
    [SerializeField] private Slider biasSlider = null;
    [SerializeField] private TMP_Text sigmoidSlopeTxt = null;
    [SerializeField] private Slider sigmoidSlopeSlider = null;

    [Header("Buttons Settings")]
    [SerializeField] private Button startButton = null;
    [SerializeField] private Button loadButton = null;
    #endregion

    #region PRIVATE_FIELDS
    private string populationText = string.Empty;
    private string turnsText = string.Empty;
    private string elitesText = string.Empty;
    private string mutationChanceText = string.Empty;
    private string mutationRateText = string.Empty;
    private string hiddenLayersCountText = string.Empty;
    private string biasText = string.Empty;
    private string sigmoidSlopeText = string.Empty;
    private string neuronsPerHLCountText = string.Empty;
    #endregion

    #region PUBLIC_METHODS
    public void Init(Action onStartGame)
    {
        populationCountSlider.onValueChanged.AddListener(OnPopulationCountChange);
        turnsSlider.onValueChanged.AddListener(OnTurnsChange);
        eliteCountSlider.onValueChanged.AddListener(OnEliteCountChange);
        mutationChanceSlider.onValueChanged.AddListener(OnMutationChanceChange);
        mutationRateSlider.onValueChanged.AddListener(OnMutationRateChange);
        hiddenLayersCountSlider.onValueChanged.AddListener(OnHiddenLayersCountChange);
        neuronsPerHLSlider.onValueChanged.AddListener(OnNeuronsPerHLChange);
        biasSlider.onValueChanged.AddListener(OnBiasChange);
        sigmoidSlopeSlider.onValueChanged.AddListener(OnSigmoidSlopeChange);

        populationText = populationCountTxt.text;
        turnsText = turnsTxt.text;
        elitesText = eliteCountTxt.text;
        mutationChanceText = mutationChanceTxt.text;
        mutationRateText = mutationRateTxt.text;
        hiddenLayersCountText = hiddenLayersCountTxt.text;
        neuronsPerHLCountText = neuronsPerHLCountTxt.text;
        biasText = biasTxt.text;
        sigmoidSlopeText = sigmoidSlopeTxt.text;

        populationCountSlider.value = PopulationManager.Instance.PopulationCount;
        turnsSlider.value = PopulationManager.Instance.Turns;
        eliteCountSlider.value = PopulationManager.Instance.EliteCount;
        mutationChanceSlider.value = PopulationManager.Instance.MutationChance * 100.0f;
        mutationRateSlider.value = PopulationManager.Instance.MutationRate * 100.0f;
        hiddenLayersCountSlider.value = PopulationManager.Instance.HiddenLayers;
        neuronsPerHLSlider.value = PopulationManager.Instance.NeuronsCountPerHL;
        biasSlider.value = PopulationManager.Instance.Bias;
        sigmoidSlopeSlider.value = PopulationManager.Instance.P;

        startButton.onClick.AddListener(() => onStartGame?.Invoke());
        loadButton.onClick.AddListener(() => PopulationManager.Instance.LoadData(onStartGame));
    }

    public void Toggle(bool status)
    {
        holder.SetActive(status);
    }
    #endregion

    #region PRIVATE_METHODS
    private void OnPopulationCountChange(float value)
    {
        PopulationManager.Instance.PopulationCount = (int)value;

        populationCountTxt.text = string.Format(populationText, PopulationManager.Instance.PopulationCount);
    }

    private void OnTurnsChange(float value)
    {
        PopulationManager.Instance.Turns = (int)value;

        turnsTxt.text = string.Format(turnsText, PopulationManager.Instance.Turns);
    }

    private void OnEliteCountChange(float value)
    {
        PopulationManager.Instance.EliteCount = (int)value;

        eliteCountTxt.text = string.Format(elitesText, PopulationManager.Instance.EliteCount);
    }

    private void OnMutationChanceChange(float value)
    {
        PopulationManager.Instance.MutationChance = value / 100.0f;

        mutationChanceTxt.text = string.Format(mutationChanceText, (int)(PopulationManager.Instance.MutationChance * 100));
    }

    private void OnMutationRateChange(float value)
    {
        PopulationManager.Instance.MutationRate = value / 100.0f;

        mutationRateTxt.text = string.Format(mutationRateText, (int)(PopulationManager.Instance.MutationRate * 100));
    }

    private void OnHiddenLayersCountChange(float value)
    {
        PopulationManager.Instance.HiddenLayers = (int)value;

        hiddenLayersCountTxt.text = string.Format(hiddenLayersCountText, PopulationManager.Instance.HiddenLayers);
    }

    private void OnNeuronsPerHLChange(float value)
    {
        PopulationManager.Instance.NeuronsCountPerHL = (int)value;

        neuronsPerHLCountTxt.text = string.Format(neuronsPerHLCountText, PopulationManager.Instance.NeuronsCountPerHL);
    }

    private void OnBiasChange(float value)
    {
        PopulationManager.Instance.Bias = value;

        biasTxt.text = string.Format(biasText, PopulationManager.Instance.Bias.ToString("0.00"));
    }

    private void OnSigmoidSlopeChange(float value)
    {
        PopulationManager.Instance.P = value;

        sigmoidSlopeTxt.text = string.Format(sigmoidSlopeText, PopulationManager.Instance.P.ToString("0.00"));
    }
    #endregion
}