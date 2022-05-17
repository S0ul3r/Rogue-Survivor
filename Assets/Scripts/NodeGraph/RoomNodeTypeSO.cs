using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;
    
    #region Header
    [Header("Flaguj jedynie RoomNodeTypes ktore powinne byc widzialne w edytorze")]
    #endregion Header
    public bool isVisibleInNodeGraphEditor;
    #region Header
    [Header("Jeden z typów powinnen byc korytarzem")]
    #endregion Header
    public bool isCorridor;
    #region Header
    [Header("Jeden z typów powinnen byc korytarzemNS")]
    #endregion Header
    public bool isCorridorNS;
    #region Header
    [Header("Jeden z typów powinnen byc korytarzemEW")]
    #endregion Header
    public bool isCorridorEW;
    #region Header
    [Header("Jeden z typów powinnen byc wejsciem")]
    #endregion Header
    public bool isEntrance;
    #region Header
    [Header("Jeden z typów powinnen byc Boos Room")]
    #endregion Header
    public bool isBossRoom;
    #region Header
    [Header("Jeden z typów powinnen byc niczym")]
    #endregion Header
    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
    #endregion Validation
}
