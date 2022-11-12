using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    #region Header
    [Header("Flaguj jedynie RoomNodeTypes ktore powinne byc widzialne w edytorze")]
    #endregion Header
    public bool displayInNodeGraphEditor = true;
    #region Header
    [Header("Czy jest pokojem ze skrzynia")]
    #endregion Header
    public bool isChestRoom;
    #region Header
    [Header("Czy jest korytarzem")]
    #endregion Header
    public bool isCorridor;
    #region Header
    [Header("Czy jest CorridorNS ")]
    #endregion Header
    public bool isCorridorNS;
    #region Header
    [Header("Czy jest CorridorEW")]
    #endregion Header
    public bool isCorridorEW;
    #region Header
    [Header("Czy jest Entrance")]
    #endregion Header
    public bool isEntrance;
    #region Header
    [Header("Czy jest Boss Room")]
    #endregion Header
    public bool isBossRoom;
    #region Header
    [Header("Czy jest None (Unassigned)")]
    #endregion Header
    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
    #endregion
}
