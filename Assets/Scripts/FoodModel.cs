using UnityEngine;

[CreateAssetMenu(fileName = "Food_", menuName = "Models/Food", order = 1)]
public class FoodModel : ScriptableObject
{
    #region EXPOSED_FIELDS
    [SerializeField] private Mesh mesh = null;
    [SerializeField] private Material material = null;
    #endregion

    #region PROPERTIES
    public Mesh Mesh => mesh;
    public Material Material => material;
    #endregion
}
