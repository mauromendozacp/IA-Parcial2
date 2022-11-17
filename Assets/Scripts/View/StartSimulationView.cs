using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class StartSimulationView : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [Header("General Settings")]
    [SerializeField] private GameObject holder = null;
    [SerializeField] private TMP_Text populationCountTxt = null;
    [SerializeField] private Slider populationCountSlider = null;
    [SerializeField] private TMP_Text turnsTxt = null;
    [SerializeField] private Slider turnsSlider = null;
    [SerializeField] private TMP_Text agentMaxGenerationTxt = null;
    [SerializeField] private Slider agentMaxGenerationSlider = null;

    [Header("Team A Settings")]
    [SerializeField] private TMP_Text eliteCountATxt = null;
    [SerializeField] private Slider eliteCountASlider = null;
    [SerializeField] private TMP_Text mutationChanceATxt = null;
    [SerializeField] private Slider mutationChanceASlider = null;
    [SerializeField] private TMP_Text mutationRateATxt = null;
    [SerializeField] private Slider mutationRateASlider = null;
    [SerializeField] private TMP_Text hiddenLayersCountATxt = null;
    [SerializeField] private Slider hiddenLayersCountASlider = null;
    [SerializeField] private TMP_Text neuronsPerHLCountATxt = null;
    [SerializeField] private Slider neuronsPerHLASlider = null;
    [SerializeField] private TMP_Text biasATxt = null;
    [SerializeField] private Slider biasASlider = null;
    [SerializeField] private TMP_Text sigmoidSlopeATxt = null;
    [SerializeField] private Slider sigmoidSlopeASlider = null;

    [Header("Team B Settings")]
    [SerializeField] private TMP_Text eliteCountBTxt = null;
    [SerializeField] private Slider eliteCountBSlider = null;
    [SerializeField] private TMP_Text mutationChanceBTxt = null;
    [SerializeField] private Slider mutationChanceBSlider = null;
    [SerializeField] private TMP_Text mutationRateBTxt = null;
    [SerializeField] private Slider mutationRateBSlider = null;
    [SerializeField] private TMP_Text hiddenLayersCountBTxt = null;
    [SerializeField] private Slider hiddenLayersCountBSlider = null;
    [SerializeField] private TMP_Text neuronsPerHLCountBTxt = null;
    [SerializeField] private Slider neuronsPerHLBSlider = null;
    [SerializeField] private TMP_Text biasBTxt = null;
    [SerializeField] private Slider biasBSlider = null;
    [SerializeField] private TMP_Text sigmoidSlopeBTxt = null;
    [SerializeField] private Slider sigmoidSlopeBSlider = null;

    [Header("Buttons Settings")]
    [SerializeField] private Button startButton = null;
    [SerializeField] private Button loadButton = null;
    #endregion

    #region PRIVATE_FIELDS
    private string populationText = string.Empty;
    private string turnsText = string.Empty;
    private string agentMaxGenerationText = string.Empty;

    private string elitesAText = string.Empty;
    private string mutationChanceAText = string.Empty;
    private string mutationRateAText = string.Empty;
    private string hiddenLayersCountAText = string.Empty;
    private string biasAText = string.Empty;
    private string sigmoidSlopeAText = string.Empty;
    private string neuronsPerHLCountAText = string.Empty;

    private string elitesBText = string.Empty;
    private string mutationChanceBText = string.Empty;
    private string mutationRateBText = string.Empty;
    private string hiddenLayersCountBText = string.Empty;
    private string biasBText = string.Empty;
    private string sigmoidSlopeBText = string.Empty;
    private string neuronsPerHLCountBText = string.Empty;
    #endregion

    #region PUBLIC_METHODS
    public void Init(Action<bool> onStartGame)
    {
        populationCountSlider.onValueChanged.AddListener(OnPopulationCountChange);
        turnsSlider.onValueChanged.AddListener(OnTurnsChange);
        agentMaxGenerationSlider.onValueChanged.AddListener(OnAgentMaxGenerationChange);

        eliteCountASlider.onValueChanged.AddListener(OnEliteCountAChange);
        mutationChanceASlider.onValueChanged.AddListener(OnMutationChanceAChange);
        mutationRateASlider.onValueChanged.AddListener(OnMutationRateAChange);
        hiddenLayersCountASlider.onValueChanged.AddListener(OnHiddenLayersCountAChange);
        neuronsPerHLASlider.onValueChanged.AddListener(OnNeuronsPerHLAChange);
        biasASlider.onValueChanged.AddListener(OnBiasAChange);
        sigmoidSlopeASlider.onValueChanged.AddListener(OnSigmoidSlopeAChange);

        eliteCountBSlider.onValueChanged.AddListener(OnEliteCountBChange);
        mutationChanceBSlider.onValueChanged.AddListener(OnMutationChanceBChange);
        mutationRateBSlider.onValueChanged.AddListener(OnMutationRateBChange);
        hiddenLayersCountBSlider.onValueChanged.AddListener(OnHiddenLayersCountBChange);
        neuronsPerHLBSlider.onValueChanged.AddListener(OnNeuronsPerHLBChange);
        biasBSlider.onValueChanged.AddListener(OnBiasBChange);
        sigmoidSlopeBSlider.onValueChanged.AddListener(OnSigmoidSlopeBChange);

        populationText = populationCountTxt.text;
        turnsText = turnsTxt.text;
        agentMaxGenerationText = agentMaxGenerationTxt.text;

        elitesAText = eliteCountATxt.text;
        mutationChanceAText = mutationChanceATxt.text;
        mutationRateAText = mutationRateATxt.text;
        hiddenLayersCountAText = hiddenLayersCountATxt.text;
        neuronsPerHLCountAText = neuronsPerHLCountATxt.text;
        biasAText = biasATxt.text;
        sigmoidSlopeAText = sigmoidSlopeATxt.text;

        elitesBText = eliteCountBTxt.text;
        mutationChanceBText = mutationChanceBTxt.text;
        mutationRateBText = mutationRateBTxt.text;
        hiddenLayersCountBText = hiddenLayersCountBTxt.text;
        neuronsPerHLCountBText = neuronsPerHLCountBTxt.text;
        biasBText = biasBTxt.text;
        sigmoidSlopeBText = sigmoidSlopeBTxt.text;

        populationCountSlider.value = PopulationManager.Instance.PopulationCount;
        turnsSlider.value = PopulationManager.Instance.Turns;
        agentMaxGenerationSlider.value = PopulationManager.Instance.AgentMaxGeneration;

        eliteCountASlider.value = PopulationManager.Instance.A_EliteCount;
        mutationChanceASlider.value = PopulationManager.Instance.A_MutationChance * 100.0f;
        mutationRateASlider.value = PopulationManager.Instance.A_MutationRate * 100.0f;
        hiddenLayersCountASlider.value = PopulationManager.Instance.A_HiddenLayers;
        neuronsPerHLASlider.value = PopulationManager.Instance.A_NeuronsCountPerHL;
        biasASlider.value = PopulationManager.Instance.A_Bias;
        sigmoidSlopeASlider.value = PopulationManager.Instance.A_P;

        eliteCountBSlider.value = PopulationManager.Instance.B_EliteCount;
        mutationChanceBSlider.value = PopulationManager.Instance.B_MutationChance * 100.0f;
        mutationRateBSlider.value = PopulationManager.Instance.B_MutationRate * 100.0f;
        hiddenLayersCountBSlider.value = PopulationManager.Instance.B_HiddenLayers;
        neuronsPerHLBSlider.value = PopulationManager.Instance.B_NeuronsCountPerHL;
        biasBSlider.value = PopulationManager.Instance.B_Bias;
        sigmoidSlopeBSlider.value = PopulationManager.Instance.B_P;

        OnPopulationCountChange(populationCountSlider.value);
        OnTurnsChange(turnsSlider.value);
        OnAgentMaxGenerationChange(agentMaxGenerationSlider.value);

        OnEliteCountAChange(eliteCountASlider.value);
        OnMutationChanceAChange(mutationChanceASlider.value);
        OnMutationRateAChange(mutationRateASlider.value);
        OnHiddenLayersCountAChange(hiddenLayersCountASlider.value);
        OnNeuronsPerHLAChange(neuronsPerHLASlider.value);
        OnBiasAChange(biasASlider.value);
        OnSigmoidSlopeAChange(sigmoidSlopeASlider.value);

        OnEliteCountBChange(eliteCountBSlider.value);
        OnMutationChanceBChange(mutationChanceBSlider.value);
        OnMutationRateBChange(mutationRateBSlider.value);
        OnHiddenLayersCountBChange(hiddenLayersCountBSlider.value);
        OnNeuronsPerHLBChange(neuronsPerHLBSlider.value);
        OnBiasBChange(biasBSlider.value);
        OnSigmoidSlopeBChange(sigmoidSlopeBSlider.value);

        startButton.onClick.AddListener(() => { onStartGame?.Invoke(false); });
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

    private void OnAgentMaxGenerationChange(float value)
    {
        PopulationManager.Instance.AgentMaxGeneration = (int)value;

        agentMaxGenerationTxt.text = string.Format(agentMaxGenerationText, PopulationManager.Instance.AgentMaxGeneration);
    }

    private void OnEliteCountAChange(float value)
    {
        PopulationManager.Instance.A_EliteCount = (int)value;

        eliteCountATxt.text = string.Format(elitesAText, PopulationManager.Instance.A_EliteCount);
    }

    private void OnMutationChanceAChange(float value)
    {
        PopulationManager.Instance.A_MutationChance = value / 100.0f;

        mutationChanceATxt.text = string.Format(mutationChanceAText, (int)(PopulationManager.Instance.A_MutationChance * 100));
    }

    private void OnMutationRateAChange(float value)
    {
        PopulationManager.Instance.A_MutationRate = value / 100.0f;

        mutationRateATxt.text = string.Format(mutationRateAText, (int)(PopulationManager.Instance.A_MutationRate * 100));
    }

    private void OnHiddenLayersCountAChange(float value)
    {
        PopulationManager.Instance.A_HiddenLayers = (int)value;

        hiddenLayersCountATxt.text = string.Format(hiddenLayersCountAText, PopulationManager.Instance.A_HiddenLayers);
    }

    private void OnNeuronsPerHLAChange(float value)
    {
        PopulationManager.Instance.A_NeuronsCountPerHL = (int)value;

        neuronsPerHLCountATxt.text = string.Format(neuronsPerHLCountAText, PopulationManager.Instance.A_NeuronsCountPerHL);
    }

    private void OnBiasAChange(float value)
    {
        PopulationManager.Instance.A_Bias = value;

        biasATxt.text = string.Format(biasAText, PopulationManager.Instance.A_Bias.ToString("0.00"));
    }

    private void OnSigmoidSlopeAChange(float value)
    {
        PopulationManager.Instance.A_P = value;

        sigmoidSlopeATxt.text = string.Format(sigmoidSlopeAText, PopulationManager.Instance.A_P.ToString("0.00"));
    }

    private void OnEliteCountBChange(float value)
    {
        PopulationManager.Instance.B_EliteCount = (int)value;

        eliteCountBTxt.text = string.Format(elitesBText, PopulationManager.Instance.B_EliteCount);
    }

    private void OnMutationChanceBChange(float value)
    {
        PopulationManager.Instance.B_MutationChance = value / 100.0f;

        mutationChanceBTxt.text = string.Format(mutationChanceBText, (int)(PopulationManager.Instance.B_MutationChance * 100));
    }

    private void OnMutationRateBChange(float value)
    {
        PopulationManager.Instance.B_MutationRate = value / 100.0f;

        mutationRateBTxt.text = string.Format(mutationRateBText, (int)(PopulationManager.Instance.B_MutationRate * 100));
    }

    private void OnHiddenLayersCountBChange(float value)
    {
        PopulationManager.Instance.B_HiddenLayers = (int)value;

        hiddenLayersCountBTxt.text = string.Format(hiddenLayersCountBText, PopulationManager.Instance.B_HiddenLayers);
    }

    private void OnNeuronsPerHLBChange(float value)
    {
        PopulationManager.Instance.B_NeuronsCountPerHL = (int)value;

        neuronsPerHLCountBTxt.text = string.Format(neuronsPerHLCountBText, PopulationManager.Instance.B_NeuronsCountPerHL);
    }

    private void OnBiasBChange(float value)
    {
        PopulationManager.Instance.B_Bias = value;

        biasBTxt.text = string.Format(biasBText, PopulationManager.Instance.B_Bias.ToString("0.00"));
    }

    private void OnSigmoidSlopeBChange(float value)
    {
        PopulationManager.Instance.B_P = value;

        sigmoidSlopeBTxt.text = string.Format(sigmoidSlopeBText, PopulationManager.Instance.B_P.ToString("0.00"));
    }
    #endregion
}