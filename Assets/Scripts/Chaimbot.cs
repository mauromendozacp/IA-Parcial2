using System.Collections;

using UnityEngine;

public enum TEAM
{
    NONE,
    MALE,
    FEMALE
}

public class Chaimbot : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [SerializeField] private float moveDelay = 0f;
    [SerializeField] private Animator animator = null;
    [SerializeField] private AnimationCurve curve = null;

    [Header("View Settings")]
    [SerializeField] private GameObject[] maleViews = null;
    [SerializeField] private GameObject[] femaleViews = null;
    #endregion

    #region PRIVATE_FIELDS
    private TEAM team = TEAM.NONE;
    private float unit = 0f;

    private BehaviourTree behaviourTree = null;
    #endregion

    #region CONSTANTS_FIELDS
    private const string speedKey = "speed";
    #endregion

    #region PUBLIC_METHODS
    public void Init(float unit, TEAM team)
    {
        this.unit = unit;
        this.team = team;

        SetView(team);
    }

    public void ProcessOutputs(float[] outputs)
    {
        if (outputs != null && outputs.Length >= 2)
        {
            float x = outputs[0] < 0f ? 1f : -1f;
            float z = outputs[1] < 0f ? 1f : -1f;

            Vector3 dir = new Vector3(x, 0f, z);
            Vector3 movePosition = transform.position + dir * unit;
            transform.forward = dir;

            StartCoroutine(MoveLerp(movePosition));
        }
    }
    #endregion

    #region PRIVATE_METHODS
    private IEnumerator MoveLerp(Vector3 movePosition)
    {
        float timer = 0f;
        Vector3 startPosition = transform.position;

        while (timer < moveDelay)
        {
            timer += Time.deltaTime;

            float lerp = curve.Evaluate(timer);
            transform.position = Vector3.Lerp(startPosition, movePosition, lerp);

            animator.SetFloat(speedKey, lerp < 0.5f ? lerp : 1f - lerp);

            yield return new WaitForEndOfFrame();
        }

        transform.position = movePosition;
        yield return null;
    }

    private void SetView(TEAM team)
    {
        if (team == TEAM.MALE)
        {
            for (int i = 0; i < maleViews.Length; i++)
            {
                maleViews[i].SetActive(true);
            }
        }
        else if (team == TEAM.FEMALE)
        {
            for (int i = 0; i < femaleViews.Length; i++)
            {
                femaleViews[i].SetActive(true);
            }
        }
    }
    #endregion
}