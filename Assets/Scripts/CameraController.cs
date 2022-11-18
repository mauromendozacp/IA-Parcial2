using UnityEngine;

public enum CAMERA_MODE
{
    LOCKED,
    FOLLOW,
    FREE
}

public class CameraController : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [Header("Camera Locked Settings")]
    [SerializeField] private Transform startLockedPos = null;

    [Header("Camera Follow Settings")]
    [SerializeField] private Vector3 playerOffset = Vector3.zero;
    [SerializeField] private float smooth = 0f;

    [Header("Camera Free Settings")]
    [SerializeField] private Transform startFreePos = null;
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float zoomSpeed = 0f;
    #endregion

    #region PRIVATE_FIELDS
    private CAMERA_MODE mode = CAMERA_MODE.LOCKED;

    private Chaimbot followChaimbot = null;
    #endregion

    #region UNITY_CALLS
    private void LateUpdate()
    {
        switch (mode)
        {
            case CAMERA_MODE.LOCKED: Locked();
                break;
            case CAMERA_MODE.FOLLOW:
                Follow();
                PopulationManager.Instance.UpdateFollowChaimbotData(followChaimbot);
                break;
            case CAMERA_MODE.FREE: Free();
                break;
        }
    }
    #endregion

    #region PUBLIC_METHODS
    public void StartCamera()
    {
        SetMode(CAMERA_MODE.LOCKED);
    }

    public void SetMode(CAMERA_MODE mode)
    {
        this.mode = mode;

        if (mode != CAMERA_MODE.FOLLOW)
        {
            PopulationManager.Instance.UpdateFollowChaimbotData(null);
        }

        if (mode == CAMERA_MODE.FREE)
        {
            transform.position = startFreePos.position;
            transform.rotation = startFreePos.rotation;
        }
    }

    public void SetFollowAgent(Chaimbot chaimbot)
    {
        followChaimbot = chaimbot;
    }
    #endregion

    #region PRIVATE_METHODS
    private void Locked()
    {
        transform.position = startLockedPos.position;
        transform.rotation = startLockedPos.rotation;
    }

    private void Follow()
    {
        if (mode == CAMERA_MODE.FOLLOW && followChaimbot != null)
        {
            Vector3 followPosition = followChaimbot.transform.position + playerOffset;
            transform.position = Vector3.Slerp(transform.position, followPosition, smooth);

            transform.LookAt(followChaimbot.transform);
        }
    }

    private void Free()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.up * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.up * moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
        }

        transform.position += transform.forward * Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Escape))
        {
            SetMode(CAMERA_MODE.LOCKED);
        }
    }
    #endregion
}
