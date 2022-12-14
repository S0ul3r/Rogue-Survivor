using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RoomBlockSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomBlockGraphSO roomNodeGraph;
    public RoomBlockTypeSO roomNodeType;
    [HideInInspector] public RoomBlockTypeListSO roomNodeTypeList;

    // Node layout values
    private const float RoomNodeWidth = 160f;
    private const float RoomNodeHeight = 75f;

    private const float CorridorNodeWidth = 120f;
    private const float CorridorNodeHeight = 75f;

    #region Editor Code

#if UNITY_EDITOR
    [HideInInspector] public GUIStyle style;
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    /// <summary>
    /// Initialise node
    /// </summary>
    public void Initialise(Rect rect, RoomBlockGraphSO nodeGraph, RoomBlockTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        // Load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }


    /// <summary>
    /// Draw node with the nodestyle
    /// </summary>
    public void Draw(GUIStyle nodeStyle)
    {
        style = nodeStyle;
        //  Draw Node Box Using Begin Area
        GUILayout.BeginArea(rect, nodeStyle);

        // Start change for popup window
        EditorGUI.BeginChangeCheck();

        // Color nodes depending which node they are
        if (roomNodeType.isCorridor)
        {
            rect.width = CorridorNodeWidth;
            rect.height = CorridorNodeHeight;
            GUI.contentColor = Color.white;
        }
        else if (roomNodeType.isChestRoom)
        {
            rect.width = RoomNodeWidth;
            rect.height = RoomNodeHeight;
            GUI.contentColor = Color.black;
        }
        else
        {
            rect.width = RoomNodeWidth;
            rect.height = RoomNodeHeight;
            GUI.contentColor = Color.white;
        }

        // Check if room node has a parent node or is Entrance node type then display label else display popup
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            // Show popup using the RoomNodeType.name values that can be selected from (default to the currently set roomNodeType)
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeFromIDTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];

            // If selection for room type has changed and some child connection can be invalid
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor ||
                !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor ||
                !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                // check if it is selected and has children
                if (childRoomNodeIDList.Count > 0)
                {
                    // loop through children
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        // get child room node
                        RoomBlockSO childRoomNode = roomNodeGraph.GetRoomNodeFromID(childRoomNodeIDList[i]);

                        // Remove child from parent and parent from child
                        if (childRoomNode != null)
                        {
                            RemoveChildIDFromRoomNodeIDList(childRoomNode.id);
                            childRoomNode.RemoveParentIDFromRoomNodeIDList(id);
                        }
                    }
                }
            }
        }

        // End change for popup window
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    /// <summary>
    /// Fill string array with the room node types for display in popup window
    /// </summary>
    public string[] GetRoomNodeFromIDTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }

    /// <summary>
    /// Process Events for nodes
    /// </summary>
    public void ProcessEvents(Event thisEvent)
    {
        switch (thisEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(thisEvent);
                break;

            case EventType.MouseUp:
                ProcessMouseUpEvent(thisEvent);
                break;

            case EventType.MouseDrag:
                ProcessMouseDragEvent(thisEvent);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// For mouse down event
    /// </summary> 
    private void ProcessMouseDownEvent(Event thisEvent)
    {
        // LMB
        if (thisEvent.button == 0)
        {
            ProcessLeftClickDownEvent(thisEvent);
        }

        // RMB
        if (thisEvent.button == 1)
        {
            ProcessRightClickDownEvent(thisEvent);
        }
    }

    /// <summary>
    /// For left click down event
    /// </summary> 
    private void ProcessLeftClickDownEvent(Event thisEvent)
    {
        Selection.activeObject = this;

        // If shift or ctrl is held down, add to the selection
        if (thisEvent.shift || thisEvent.control)
        {
            isSelected = !isSelected;
        }
        else
        {
            // Select the node  and deselect all other nodes
            foreach (var roomNode in roomNodeGraph.roomNodeList.Where(
                            roomNode => roomNode != this && roomNode.isSelected))
                roomNode.isSelected = false;
            isSelected = true;
        }
    }

    /// <summary>
    /// For right click down event
    /// </summary>
    private void ProcessRightClickDownEvent(Event thisEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, thisEvent.mousePosition);
    }

    /// <summary>
    /// For mouse up event
    /// </summary> 
    private void ProcessMouseUpEvent(Event thisEvent)
    {
        if (thisEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    /// <summary>
    /// For left click up event
    /// </summary>
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    /// <summary>
    /// for mouse drag event
    /// </summary>
    private void ProcessMouseDragEvent(Event thisEvent)
    {
        if (thisEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(thisEvent);
        }
    }

    /// <summary>
    /// for left mouse drag
    /// </summary>
    private void ProcessLeftMouseDragEvent(Event thisEvent)
    {
        isLeftClickDragging = true;

        DragNode(thisEvent.delta);
        GUI.changed = true;
    }

    /// <summary>
    /// drag node
    /// </summary>
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    ///  Add ParentID to childRoomNodeIDList 
    /// </summary>
    public bool AddParentIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    /// <summary>
    /// Add ChildID bool to childRoomNodeIDList
    /// </summary>
    public bool AddChildIDToRoomNode(string childID)
    {
        // child room node validation 
        if (isChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Remove childID from childRoomNodeIDList
    /// </summary>
    public bool RemoveChildIDFromRoomNodeIDList(string childID)
    {
        // If there is a node with such ID, remove it
        if (childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Remove parentID from parentRoomNodeIDList
    /// </summary>
    public bool RemoveParentIDFromRoomNodeIDList(string parentID)
    {
        // If there is a node with such ID, remove it
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Validaton for child room if it can be added to parent room
    /// </summary>
    public bool isChildRoomValid(string childID)
    {
        bool hasBossRoom = false;

        // Check if there is boss room in node graph and is connected 
        foreach (RoomBlockSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
                hasBossRoom = true;
        }

        // Check if child node is not connected to itself
        if (childID == id)
            return false;

        // Check if child node is already connected to parent node
        if (parentRoomNodeIDList.Contains(childID))
            return false;

        // Check if the child node had a parent node
        if (roomNodeGraph.GetRoomNodeFromID(childID).parentRoomNodeIDList.Count > 0)
            return false;

        // Check if child room is already connected to boss room and is not boss room
        if (roomNodeGraph.GetRoomNodeFromID(childID).roomNodeType.isBossRoom && hasBossRoom)
            return false;

        // Check if child room is none type
        if (roomNodeGraph.GetRoomNodeFromID(childID).roomNodeType.isNone)
            return false;

        // check if the node already has a child with this childID
        if (childRoomNodeIDList.Contains(childID))
            return false;

        // Check if child node is corridor it cannot be connected to corridor
        if (roomNodeType.isCorridor && roomNodeGraph.GetRoomNodeFromID(childID).roomNodeType.isCorridor)
            return false;

        // Check if a child node is not a corridor it cannot be connected to non corridor
        if (!roomNodeType.isCorridor && !roomNodeGraph.GetRoomNodeFromID(childID).roomNodeType.isCorridor)
            return false;

        // Check if child room is an entrance (entrance is always top parent node)
        if (roomNodeGraph.GetRoomNodeFromID(childID).roomNodeType.isEntrance)
            return false;


        // When adding child node room to corridor check if that corridor doesnt have any other child rooms
        if (childRoomNodeIDList.Count > 0 && !roomNodeGraph.GetRoomNodeFromID(childID).roomNodeType.isCorridor)
            return false;

        // When a child node is a corridor it cannot have more than max child corridors
        if (roomNodeGraph.GetRoomNodeFromID(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
            return false;

        return true;
    }

#endif

    #endregion Editor Code
}
