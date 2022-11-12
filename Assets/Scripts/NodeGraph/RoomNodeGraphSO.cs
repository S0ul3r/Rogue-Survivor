using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

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
        foreach (RoomNodeSO node in roomNodeList)
        {
            roomNodeDictionary[node.id] = node;
        }
    }

    /// <summary>
    /// Get room node by roomNodeType
    /// </summary>
    public RoomNodeSO GetRoomNodeByType(RoomNodeTypeSO roomNodeType)
    {
        foreach (RoomNodeSO node in roomNodeList)
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
    public RoomNodeSO GetRoomNodeFromID(string id)
    {
        if (roomNodeDictionary.TryGetValue(id, out RoomNodeSO roomNode))
        {
            return roomNode;
        }
        Debug.LogError("RoomNodeDictionary does not contain key: " + id);
        return null;
    }

    /// <summary>
    ///  Get child room node for given parent room node
    /// </summary>
    public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentRoomNode)
    {
        foreach (string childNodeID in parentRoomNode.childRoomNodeIDList)
        {
            yield return GetRoomNodeFromID(childNodeID);
        }
    }

    #region Editor Code

    // This part shoud only be run in Unity
#if UNITY_EDITOR
    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePos;

    // Refresh node dict every time change has been made
    public void OnValidate()
    {
        LoadRoomNodeDict();
    }

    public void SetNodeToDrawConnectionLineFrom(RoomNodeSO roomNodeSO, Vector2 pos)
    {
        roomNodeToDrawLineFrom = roomNodeSO;
        linePos = pos;
    }
#endif

    #endregion
}
