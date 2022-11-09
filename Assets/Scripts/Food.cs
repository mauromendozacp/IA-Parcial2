using UnityEngine;

public class Food : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [SerializeField] private AnimationCurve curve = null;
    [SerializeField] private float floatingDistance = 0f;

    [SerializeField] private MeshFilter meshFilter = null;
    [SerializeField] private MeshRenderer meshRenderer = null;
    #endregion

    #region PRIVATE_FIELDS
    private Vector2Int index = Vector2Int.zero;

    private Vector3 startPosition = Vector3.zero;
    private Vector3 endPosition = Vector3.zero;
    private float floatingTimer = 0f;
    #endregion

    #region UNITY_CALLS
    private void Start()
    {
        startPosition = transform.position - new Vector3(0f, floatingDistance / 2f, 0f);
        endPosition = transform.position + new Vector3(0f, floatingDistance / 2f, 0f);
    }

    private void Update()
    {
        UpdateFloating();
    }
    #endregion

    #region PUBLIC_METHODS
    public void Init(FoodModel model, Vector2Int index)
    {
        this.index = index;

        meshFilter.mesh = model.Mesh;
        meshRenderer.material = model.Material;
    }
    #endregion

    #region PRIVATE_METHODS
    private void UpdateFloating()
    {
        floatingTimer += Time.deltaTime;
        if (floatingTimer > 1f) floatingTimer = 0f;

        transform.position = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(floatingTimer));
    }
    #endregion
}