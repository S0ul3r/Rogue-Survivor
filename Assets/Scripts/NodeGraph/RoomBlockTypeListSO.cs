using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomBlockTypeListSO", menuName = "Scriptable Objects/Dungeon/Room Block Type List")]
public class RoomBlockTypeListSO : ScriptableObject
{
    #region Header ROOM NODE TYPE LIST
    [Space(10)]
    [Header("ROOM NODE TYPE LIST")]
    #endregion
    #region Tooltip
    [Tooltip("Lista powinna byc wypelniona wszystkimi RoomBlockTypeSO gry - wykorzystujemy count() zamiast enumerate")]
    #endregion
    public List<RoomBlockTypeSO> list;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif
    #endregion
}
