using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomBlockGraphSO : ScriptableObject
{
    [HideInInspector] public RoomBlockTypeListSO roomNodeTypeList;
    [HideInInspector] public List<RoomBlockSO> roomNodeList = new List<RoomBlockSO>();
    [HideInInspector] public Dictionary<string, RoomBlockSO> roomNodeDictionary = new Dictionary<string, RoomBlockSO>();

    private void Awake()
    {
        LoadRoomNodeDict();
    }

    /// <summary>
    /// Load roomNodeDictionary z roomNodeList
    /// </summary>
    private void LoadRoomNodeDict()
    {
        roomNodeDictionary.Clear();

        // Populate dictionary
        foreach (RoomBlockSO node in roomNodeList)
        {
            roomNodeDictionary[node.id] = node;
        }
    }

    /// <summary>
    /// Get room node by roomNodeType
    /// </summary>
    public RoomBlockSO GetRoomNodeByType(RoomBlockTypeSO roomNodeType)
    {
        foreach (RoomBlockSO node in roomNodeList)
        {
            if (node.roomNodeType == roomNodeType)
            {
                return node;
            }
        }
        return null;
    }

    /// <summary>
    ///  Get room node by id
    /// </summary>
    public RoomBlockSO GetRoomNodeFromID(string id)
    {
        if (roomNodeDictionary.TryGetValue(id, out RoomBlockSO roomNode))
        {
            return roomNode;
        }
        Debug.LogError("RoomNodeDictionary does not contain key: " + id);
        return null;
    }

    /// <summary>
    ///  Get child room node for given parent room node
    /// </summary>
    public IEnumerable<RoomBlockSO> GetChildRoomNodes(RoomBlockSO parentRoomNode)
    {
        foreach (string childNodeID in parentRoomNode.childRoomNodeIDList)
        {
            yield return GetRoomNodeFromID(childNodeID);
        }
    }

    #region Editor Code

    // This part shoud only be run in Unity
#if UNITY_EDITOR
    [HideInInspector] public RoomBlockSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePos;

    // Refresh node dict every time change has been made
    public void OnValidate()
    {
        LoadRoomNodeDict();
    }

    public void SetNodeToDrawConnectionLineFrom(RoomBlockSO roomNodeSO, Vector2 pos)
    {
        roomNodeToDrawLineFrom = roomNodeSO;
        linePos = pos;
    }
#endif

    #endregion
}
