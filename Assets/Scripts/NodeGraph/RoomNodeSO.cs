using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code

#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    /// <summary>
    /// Initialise node
    /// </summary>
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
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
        //  Draw Node Box Using Begin Area
        GUILayout.BeginArea(rect, nodeStyle);

        // Start change for popup window
        EditorGUI.BeginChangeCheck();

        // Show popup using the RoomNodeType.name values that can be selected from (default to the currently set roomNodeType)
        int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

        roomNodeType = roomNodeTypeList.list[selection];

        // End change for popup window
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    /// <summary>
    /// Fill string array with the room node types for display in popup window
    /// </summary>
    public string[] GetRoomNodeTypesToDisplay()
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
            ProcessLeftClickDownEvent();
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
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;
        
        if (isSelected) 
        {
            isSelected = false;
        }
        else
        {
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
    private void ProcessLeftClickUpEvent() {
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
    private void DragNode(Vector2 delta)
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
        childRoomNodeIDList.Add(childID);
        return true;
    }

#endif

    #endregion Editor Code
}
