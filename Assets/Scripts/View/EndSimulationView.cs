using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class EndSimulationView : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [Header("General Settings")]
    [SerializeField] private GameObject holder = null;
    [SerializeField] private TMP_Text title = null;
    [SerializeField] private Button backBtn = null;

    [Header("Info Settings")]
    [SerializeField] private TMP_Text chaimbotsATxt = null;
    [SerializeField] private TMP_Text foodsATxt = null;
    [SerializeField] private TMP_Text deathsATxt = null;
    [SerializeField] private TMP_Text extinctsATxt = null;
    [SerializeField] private TMP_Text fitnessATxt = null;

    [SerializeField] private TMP_Text chaimbotsBTxt = null;
    [SerializeField] private TMP_Text foodsBTxt = null;
    [SerializeField] private TMP_Text deathsBTxt = null;
    [SerializeField] private TMP_Text extinctsBTxt = null;
    [SerializeField] private TMP_Text fitnessBTxt = null;
    #endregion

    #region PRIVATE_FIELDS
    private string chaimbotsAText = string.Empty;
    private string foodsAText = string.Empty;
    private string deathsAText = string.Empty;
    private string extinctsAText = string.Empty;
    private string fitnessAText = string.Empty;

    private string chaimbotsBText = string.Empty;
    private string foodsBText = string.Empty;
    private string deathsBText = string.Empty;
    private string extinctsBText = string.Empty;
    private string fitnessBText = string.Empty;
    #endregion

    #region CONSTANTS
    private const string noWinText = "NO WINNER";
    private const string tieText = "TIE";
    private const string aWinText = "TEAM A WINNER";
    private const string bWinText = "TEAM B WINNER";
    #endregion

    #region PUBLIC_METHODS
    public void Init(Action onBackMenu)
    {
        chaimbotsAText = chaimbotsATxt.text;
        foodsAText = foodsATxt.text;
        deathsAText = deathsATxt.text;
        extinctsAText = extinctsATxt.text;
        fitnessAText = fitnessATxt.text;

        chaimbotsBText = chaimbotsBTxt.text;
        foodsBText = foodsBTxt.text;
        deathsBText = deathsBTxt.text;
        extinctsBText = extinctsBTxt.text;
        fitnessBText = fitnessBTxt.text;

        backBtn.onClick.AddListener(() => onBackMenu?.Invoke());
    }

    public void ConfigurePanel(bool noWin, TEAM winTeam, float fitnessA, float fitnessB)
    {
        ConfigureTitle(noWin, winTeam);

        chaimbotsATxt.text = string.Format(chaimbotsAText, PopulationManager.Instance.chaimbotsA);
        foodsATxt.text = string.Format(foodsAText, PopulationManager.Instance.foodsA);
        deathsATxt.text = string.Format(deathsAText, PopulationManager.Instance.deathsA);
        extinctsATxt.text = string.Format(extinctsAText, PopulationManager.Instance.extinctsA);
        fitnessATxt.text = string.Format(fitnessAText, fitnessA);

        chaimbotsBTxt.text = string.Format(chaimbotsBText, PopulationManager.Instance.chaimbotsB);
        foodsBTxt.text = string.Format(foodsBText, PopulationManager.Instance.foodsB);
        deathsBTxt.text = string.Format(deathsBText, PopulationManager.Instance.deathsB);
        extinctsBTxt.text = string.Format(extinctsBText, PopulationManager.Instance.extinctsB);
        fitnessBTxt.text = string.Format(fitnessBText, fitnessB);
    }

    public void Toggle(bool status)
    {
        holder.SetActive(status);
    }
    #endregion

    #region PRIVATE_METHODS
    private void ConfigureTitle(bool noWin, TEAM winTeam)
    {
        if (noWin)
        {
            title.text = noWinText;
        }
        else
        {
            switch (winTeam)
            {
                case TEAM.NONE:
                    title.text = tieText;
                    break;
                case TEAM.A:
                    title.text = aWinText;
                    break;
                case TEAM.B:
                    title.text = bWinText;
                    break;
                default:
                    title.text = string.Empty;
                    break;
            }
        }
    }
    #endregion
}
