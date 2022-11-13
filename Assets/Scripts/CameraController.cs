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
    [SerializeField] private Transform initialPos = null;
    [SerializeField] private Vector3 playerOffset = Vector3.zero;
    [SerializeField] private float smooth = 0f;
    [SerializeField] private float sensivity = 0f;
    #endregion

    #region PRIVATE_FIELDS
    private CAMERA_MODE mode = CAMERA_MODE.LOCKED;

    private Agent followAgent = null;
    #endregion

    #region UNITY_CALLS
    private void LateUpdate()
    {
        switch (mode)
        {
            case CAMERA_MODE.LOCKED: Locked();
                break;
            case CAMERA_MODE.FOLLOW: Follow();
                break;
            case CAMERA_MODE.FREE: Free();
                break;
        }
    }
    #endregion

    #region PUBLIC_METHODS
    public void StartCamera()
    {
        mode = CAMERA_MODE.LOCKED;
    }

    public void SetMode(CAMERA_MODE mode)
    {
        this.mode = mode;

        Cursor.lockState = mode == CAMERA_MODE.FREE ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void SetFollowAgent(Agent agent)
    {
        followAgent = agent;
    }
    #endregion

    #region PRIVATE_METHODS
    private void Locked()
    {
        transform.position = initialPos.position;
        transform.rotation = initialPos.rotation;
    }

    private void Follow()
    {
        if (mode == CAMERA_MODE.FOLLOW && followAgent != null)
        {
            Vector3 followPosition = followAgent.transform.position + playerOffset;
            transform.position = Vector3.Slerp(transform.position, followPosition, smooth);

            transform.LookAt(followAgent.transform);
        }
    }

    private void Free()
    {
        float rotX = Input.GetAxis("Mouse X") * sensivity * Time.deltaTime;
        float rotY = Input.GetAxis("Mouse Y") * -sensivity * Time.deltaTime;

        transform.Rotate(Vector3.up * rotX);
        transform.Rotate(Vector3.right, Mathf.Clamp(rotY, -90f, 90f));

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * (sensivity / 2) * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * (sensivity / 2) * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * (sensivity / 2) * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * (sensivity / 2) * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            SetMode(CAMERA_MODE.LOCKED);
        }
    }
    #endregion
}
