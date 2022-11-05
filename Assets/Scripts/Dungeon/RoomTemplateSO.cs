using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room Template", menuName = "Scriptable Objects/Dungeon/Room Template")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

    #region Header ROOM PREFAB
    [Space(10)]
    [Header("ROOM PREFAB")]
    #endregion Header ROOM PREFAB

    #region Tooltip
    [Tooltip("The prefab of the room (contains all the tilemaps for the room and enviroment")]
    #endregion Tooltip

    public GameObject roomPrefab;

    [HideInInspector] public GameObject previousRoomPrefab; // recreate guid when SO is copied and prefab changed

    #region Header ROOM CONFIG
    [Space(10)]
    [Header("ROOM CONFIG")]
    #endregion Header ROOM CONFIG

    #region Tooltip
    [Tooltip("The room node type SO. Types are similiar to those from room node graph. Exception: in room node grpah we onyl hhave COrridors, in templates we have corridors NS and EW")]
    #endregion Tooltip

    public RoomNodeTypeSO roomNodeType;

    #region Tooltip
    [Tooltip("Rectangle with whole room inside, bottom left corner is a lower bound. Determined from tilemap of the room (this is local tilemap pos not world pos)")]
    #endregion Tooltip

    public Vector2Int lowerBounds;

    #region Tooltip
    [Tooltip("Rectangle with whole room inside, top right corner is a lower bound. Determined from tilemap of the room (this is local tilemap pos not world pos)")]
    #endregion

    public Vector2Int upperBounds;

    #region Tooltip
    [Tooltip("Max 4 doors for a room - each for every side of the room. each doorway is 3 tiles wide, with middle one being a coordinate pos")]
    #endregion Tooltip

    [SerializeField] public List<Doorway> doorwayList;

    #region Tooltip
    [Tooltip("Every possible spawn positions for every object in the game will be in this array")]
    #endregion

    public Vector2Int[] spawnPositionArray;

    /// <summary>
    /// Return list of Entrances to the room
    /// </summary>
    public List<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }

    #region Validate

#if UNITY_EDITOR

    // Validate fields
    private void OnValidate()
    {
        // set unique GUID if empty or when prefab changed
        if(guid == "" || previousRoomPrefab != roomPrefab)
        {
            guid = GUID.Generate().ToString();
            previousRoomPrefab = roomPrefab;
            EditorUtility.SetDirty(this);
        }

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        // Check if spawns pos are populated
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }

#endif

    #endregion Validate
}