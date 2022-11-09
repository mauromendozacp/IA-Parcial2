using System.Collections.Generic;

using UnityEngine;

public class GameplayController : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [SerializeField] private Vector2Int size = Vector2Int.zero;
    [SerializeField] private float unit = 0f;
    [SerializeField] private float turnsDelay = 0f;
    [SerializeField] private int chaimbotsCount = 0;
    [SerializeField] private GameObject chaimbotPrefab = null;
    [SerializeField] private Transform chaimbotHolder = null;
    #endregion

    #region PRIVATE_FIELDS
    private List<Chaimbot> chaimbots = new List<Chaimbot>();

    private float turnsTimer = 0f;
    #endregion

    #region UNITY_CALLS
    private void Start()
    {
        SpawnChaimbots();
    }

    private void Update()
    {

    }
    #endregion

    #region PRIVATE_METHODS
    private void SpawnChaimbots()
    {
        Vector3 maleStartPosition = new Vector3(-size.x / 2 * unit, 0f, -size.y / 2 * unit);
        Vector3 femaleStartPosition = new Vector3(-size.x / 2 * unit, 0f, size.y / 2 * unit);

        for (int i = 0; i < chaimbotsCount; i++)
        {
            GameObject chaimbotGO = Instantiate(chaimbotPrefab, chaimbotHolder);
            Chaimbot chaimbot = chaimbotGO.GetComponent<Chaimbot>();
            
            if (i < chaimbotsCount / 2)
            {
                chaimbot.transform.position = maleStartPosition + new Vector3(i * unit * 2, 0f, 0f);
                chaimbot.transform.forward = transform.forward;
                chaimbot.Init(unit, TEAM.MALE);
            }
            else
            {
                chaimbot.transform.position = femaleStartPosition + new Vector3((i - chaimbotsCount / 2) * unit * 2, 0f, 0f);
                chaimbot.transform.forward = -transform.forward;
                chaimbot.Init(unit, TEAM.FEMALE);
            }

            chaimbots.Add(chaimbot);
        }
    }
    #endregion
}
