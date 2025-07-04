using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class RoomBlockGraphEditor : EditorWindow
{
    private readonly GUIStyles _styles = new GUIStyles();
    private static RoomBlockGraphSO currentRoomNodeGraph;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private RoomBlockSO currentRoomNode = null;
    private RoomBlockTypeListSO roomNodeTypeList;

    // Layout for node variables
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;

    // Widths of lines
    private const float arrowLineWidth = 4f;
    private const float lineArrowSize = 7f;

    // Grid spacing
    private const float gridLarge = 100f;
    private const float gridSmall = 20f;


    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomBlockGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        // Selection changed event
        Selection.selectionChanged += OnSelectionChanged;

        // Styles for room nodes
        _styles.Initialize();

        // Load Room node types
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        // Selection changed event cleanup
        Selection.selectionChanged -= OnSelectionChanged;
    }

    /// <summary>
    /// Open the room node graph editor window after double click on a scriptable object asset
    /// </summary>

    [OnOpenAsset(0)]  // Need the namespace UnityEditor.Callbacks
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomBlockGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomBlockGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();

            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }
        return false;
    }


    /// <summary>
    /// Draw Editor Gui
    /// </summary>
    private void OnGUI()
    {

        // If a scriptable object of type RoomBlockGraphSO has been selected then process
        if (currentRoomNodeGraph != null)
        {
            // Drag Grid
            DrawGrid(gridSmall, 0.2f, Color.gray);
            DrawGrid(gridLarge, 0.4f, Color.gray);

            // Draw draged line
            DrawDraggedLine();

            // Process Events
            ProcessEvents(Event.current);

            // Draw Connection lines to nodes
            DrawRoomNodeConnections();

            // Draw Room Nodes
            DrawRoomNodes();
        }

        if (GUI.changed)
            Repaint();
    }

    /// <summary>
    /// Drawing Grid for the editor window
    /// </summary>
    /// <param name="gridSpacing">Spacing between grid lines</param>
    /// <param name="gridOpacity">Opacity of grid lines</param>
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int numberOfVerticalLines = Mathf.CeilToInt((position.width + gridSpacing) / gridSpacing);
        int numverOfHorizontalLines = Mathf.CeilToInt((position.height + gridSpacing) / gridSpacing);
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
        graphOffset += graphDrag * 0.5f;
        Vector3 newOffset = new Vector3(graphOffset.x % gridSpacing, graphOffset.y % gridSpacing, 0);

        // Draw vertical lines
        for (int i = 0; i < numberOfVerticalLines; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        // Draw horizontal lines
        for (int i = 0; i < numverOfHorizontalLines; i++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * i, 0) + newOffset, new Vector3(position.width, gridSpacing * i, 0f) + newOffset);
        }

        Handles.color = Color.white;
    }

    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePos != Vector2.zero)
        {
            // Draw dragging arrow line from primary node
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
                currentRoomNodeGraph.linePos,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center + Vector2.left * 100f,
                currentRoomNodeGraph.linePos - Vector2.left * 100f,
                Color.magenta,
                null,
                arrowLineWidth);
        }
    }

    private void ProcessEvents(Event thisEvent)
    {
        // Reset drag of graph
        graphDrag = Vector2.zero;

        if (thisEvent.keyCode is KeyCode.Delete) RemoveSelectedRoomNodes();

        if (thisEvent.shift && thisEvent.type is EventType.KeyDown)
        {
            if (thisEvent.keyCode is KeyCode.D)
                DuplicateSelectedRoomNodes();
        }

        // check if mouse is over a node, check before if it is null or is being dragged
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = CheckMouseOverNode(thisEvent);
        }

        // if mouse isnt over a room node or is being dragged then execute Process Graph events
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(thisEvent);
        }
        else
        {
            //process room node events
            currentRoomNode.ProcessEvents(thisEvent);
        }
    }

    /// <summary>
    /// Check if mouse is over a node
    /// </summary> 
    private RoomBlockSO CheckMouseOverNode(Event thisEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(thisEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Process Room Node Graph Events
    /// </summary>
    private void ProcessRoomNodeGraphEvents(Event thisEvent)
    {
        switch (thisEvent.type)
        {
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(thisEvent);
                break;

            // Process Mouse Drag Events
            case EventType.MouseDrag:
                ProcessMouseDragEvent(thisEvent);
                break;

            // Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(thisEvent);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Process Mouse Up Events
    /// </summary>
    private void ProcessMouseUpEvent(Event thisEvent)
    {
        // We release RMB from dragged line
        if (thisEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            RoomBlockSO mouseOverNode = CheckMouseOverNode(thisEvent);

            // Check if mouse is over a node and node is not the same as the one we are drawing the line from
            // If is over a node set is as a child to the parent node we are drawing the line from
            if (mouseOverNode != null && mouseOverNode != currentRoomNodeGraph.roomNodeToDrawLineFrom &&
                currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildIDToRoomNode(mouseOverNode.id))
            {
                mouseOverNode.AddParentIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
            }

            ClearDraggedLine();
        }
    }

    /// <summary>
    /// Process Mouse Down Events in our menu
    /// </summary>
    private void ProcessMouseDownEvent(Event thisEvent)
    {
        // Process RMB click on graph event (show context menu)
        if (thisEvent.button == 1)
        {
            ShowContextMenu(thisEvent.mousePosition);
        }

        // Process LMB clck on graph event (clear line and all chosen room nodes)
        if (thisEvent.button == 0)
        {
            ClearDraggedLine();
            ClearAllSelectedRoomNodes();
        }
    }

    /// <summary>
    /// Process Mouse Drag Events in our menu
    /// </summary>
    private void ProcessMouseDragEvent(Event thisEvent)
    {
        // process LMB drag - drag graph
        if (thisEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(thisEvent.delta);
        }

        // Process RMB drag event - drow line
        if (thisEvent.button == 1)
        {
            ProcessRightMouseDragEvent(thisEvent);
        }
    }

    /// <summary>
    /// Process LMB drag event - drag graph
    /// </summary>
    private void ProcessLeftMouseDragEvent(Vector2 delta)
    {
        graphDrag = delta;

        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            currentRoomNodeGraph.roomNodeList[i].DragNode(delta);
        }

        GUI.changed = true;
    }

    /// <summary>
    /// Process RMB drag event - drow line
    /// </summary>
    private void ProcessRightMouseDragEvent(Event thisEvent)
    {
        // If we have a room node then create a new line
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(thisEvent.delta);
            GUI.changed = true;
        }
    }

    /// <summary>
    /// Updating line pos for drag from room node
    /// </summary>
    private void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePos += delta;
    }

    /// <summary>
    /// Show the context menu
    /// </summary>
    private void ShowContextMenu(Vector2 mousePos)
    {
        GenericMenu menuItem = new GenericMenu();

        menuItem.AddItem(new GUIContent("Add Room Node"), false, AddRoomNode, mousePos);
        menuItem.AddSeparator("");
        menuItem.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
        menuItem.AddSeparator("");
        menuItem.AddItem(new GUIContent("Remove Selected Room Nodes"), false, RemoveSelectedRoomNodes);
        menuItem.AddSeparator("");
        menuItem.AddItem(new GUIContent("Remove Selected Connections"), false, RemoveSelectedConnections);

        menuItem.ShowAsContext();
    }

    /// <summary>
    /// Create a room node where our mouse is
    /// </summary>
    private void AddRoomNode(object mousePosObject)
    {
        // Add entrance room node if there is no room node in the graph
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            AddRoomNode(new Vector2(100f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

        AddRoomNode(mousePosObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    /// <summary>
    /// Create a room node where our mouse is - overloaded with additional RoomNodeType
    /// </summary>
    private void AddRoomNode(object mousePosObject, RoomBlockTypeSO roomNodeType)
    {
        Vector2 mousePos = (Vector2)mousePosObject;

        // create room node scriptable object asset
        RoomBlockSO roomNode = ScriptableObject.CreateInstance<RoomBlockSO>();

        // add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        // set room node values
        roomNode.Initialise(new Rect(mousePos, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        // add room node to room node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        // Refresh room node graph dict
        currentRoomNodeGraph.OnValidate();
    }

    private void DuplicateSelectedRoomNodes()
    {
        var selectedRoomNodeList = currentRoomNodeGraph.roomNodeList.FindAll(node => node.isSelected);

        foreach (var roomNode in selectedRoomNodeList.Where(roomNode => !roomNode.roomNodeType.isEntrance))
        {
            AddRoomNode(roomNode.rect.position + new Vector2(0, 100), roomNode.roomNodeType);
        }
    }

    /// <summary>
    /// remove selected room nodes
    /// </summary>
    private void RemoveSelectedRoomNodes()
    {
        Queue<RoomBlockSO> roomNodesToRemove = new Queue<RoomBlockSO>();

        // loop through all room nodes
        foreach (RoomBlockSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            // if room node is selected add it to the queue and is not entrance
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodesToRemove.Enqueue(roomNode);

                // go through child room nodes and remove parent id from them
                foreach (string childID in roomNode.childRoomNodeIDList)
                {
                    RoomBlockSO childRoomNode = currentRoomNodeGraph.GetRoomNodeFromID(childID);

                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentIDFromRoomNodeIDList(roomNode.id);
                    }
                }

                // go through parent room nodes and remove child id from them
                foreach (string parentID in roomNode.parentRoomNodeIDList)
                {
                    RoomBlockSO parentRoomNode = currentRoomNodeGraph.GetRoomNodeFromID(parentID);

                    if (parentRoomNode != null)
                    {
                        parentRoomNode.RemoveChildIDFromRoomNodeIDList(roomNode.id);
                    }
                }
            }
        }

        // remove room nodes from our roomNodesToRemove queue
        while (roomNodesToRemove.Count > 0)
        {
            RoomBlockSO roomNode = roomNodesToRemove.Dequeue();

            // remove node from dictionary 
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNode.id);

            // remove node from room node list
            currentRoomNodeGraph.roomNodeList.Remove(roomNode);

            // remove room node from room node graph scriptable object asset database
            DestroyImmediate(roomNode, true);

            AssetDatabase.SaveAssets();
        }

    }

    /// <summary>
    /// Remove connections between selected room nodes
    /// </summary>
    private void RemoveSelectedConnections()
    {
        // loop through room nodes
        foreach (RoomBlockSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            // check if it is selected and has children
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                // loop through children
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    // get child room node
                    RoomBlockSO childRoomNode = currentRoomNodeGraph.GetRoomNodeFromID(roomNode.childRoomNodeIDList[i]);

                    // if child room node is selected remove it from parent room node child list
                    // and remove parent room node from child room node parent list
                    if (childRoomNode.isSelected && childRoomNode != null)
                    {
                        roomNode.RemoveChildIDFromRoomNodeIDList(childRoomNode.id);
                        childRoomNode.RemoveParentIDFromRoomNodeIDList(roomNode.id);
                    }
                }
            }
        }
        // Clear selected nodes
        ClearAllSelectedRoomNodes();
    }

    ///<summary>
    /// Select all room nodes
    ///</summary>
    private void SelectAllRoomNodes()
    {
        foreach (RoomBlockSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    /// <summary>
    /// Clear selection from room nodes
    /// </summary>
    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomBlockSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }

    /// <summary>
    /// Draw room nodes in the graph window
    /// </summary>
    /// Draw room nodes in the graph editor window
    private void DrawRoomNodes()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(GetRoomNodeStyle(roomNode));
        }
        GUI.changed = true;
    }

    private GUIStyle GetRoomNodeStyle(RoomBlockSO roomNode)
    {
        if (roomNode.roomNodeType.isEntrance)
        {
            return roomNode.isSelected ? _styles.entranceNodeSelectedStyle : _styles.entranceNodeStyle;
        }
        if (roomNode.roomNodeType.isCorridor)
        {
            return roomNode.isSelected ? _styles.corridorNodeSelectedStyle : _styles.corridorNodeStyle;
        }
        if (roomNode.roomNodeType.isBossRoom)
        {
            return roomNode.isSelected ? _styles.bossRoomNodeSelectedStyle : _styles.bossRoomNodeStyle;
        }
        if (roomNode.roomNodeType.isChestRoom)
        {
            return roomNode.isSelected ? _styles.chestRoomNodeSelectedStyle : _styles.chestRoomNodeStyle;
        }
        return roomNode.isSelected ? _styles.roomNodeSelectedStyle : _styles.roomNodeStyle;
    }

    /// <summary>
    /// Drawing lines between parent and child room nodes
    /// </summary>
    private void DrawNodeLine(RoomBlockSO parentRoomNode, RoomBlockSO childRoomNode, Color color)
    {
        // Get positions
        Vector2 startPos = parentRoomNode.rect.center;
        Vector2 endPos = childRoomNode.rect.center;

        // Calculate middle of the line
        Vector2 middlePos = (startPos + endPos) * 0.5f;

        // Calculate direction of the line
        Vector2 direction = (endPos - startPos).normalized;

        // Calculate normalised perpendicular direction of the line from middle of the line
        Vector2 perpendicularDirection1 = middlePos - new Vector2(-direction.y, direction.x).normalized * lineArrowSize;
        Vector2 perpendicularDirection2 = middlePos + new Vector2(-direction.y, direction.x).normalized * lineArrowSize;

        // Calculate mid point of arrow head
        Vector2 arrowHeadPos = middlePos + direction * lineArrowSize;

        // Draw arrow
        Handles.DrawBezier(arrowHeadPos, perpendicularDirection1, arrowHeadPos, perpendicularDirection1, color, null, lineArrowSize);
        Handles.DrawBezier(arrowHeadPos, perpendicularDirection2, arrowHeadPos, perpendicularDirection2, color, null, lineArrowSize);

        // Draw line
        Handles.DrawBezier(startPos, endPos, startPos, endPos, color, null, arrowLineWidth);

        GUI.changed = true;
    }

    /// <summary>
    /// Clean dragged line
    /// </summary>
    private void ClearDraggedLine()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePos = Vector2.zero;
        GUI.changed = true;
    }

    /// <summary>
    /// Draw lines between room nodes
    /// </summary>
    private void DrawRoomNodeConnections()
    {
        // Loop through all room nodes and draw lines between them
        foreach (RoomBlockSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                // Loop through child room nodes
                foreach (string childID in roomNode.childRoomNodeIDList)
                {
                    // Get child room node
                    RoomBlockSO childRoomNode = currentRoomNodeGraph.roomNodeDictionary[childID];

                    if (childRoomNode != null)
                    {
                        DrawNodeLine(roomNode, childRoomNode, Color.magenta);
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Selection changed event - update room node graph
    /// </summary>
    private void OnSelectionChanged()
    {
        // Get selected object
        Object selectedObject = Selection.activeObject;

        // If selected object is a room node graph then set it as current room node graph
        if (selectedObject != null && selectedObject.GetType() == typeof(RoomBlockGraphSO))
        {
            currentRoomNodeGraph = (RoomBlockGraphSO)selectedObject;
            GUI.changed = true;
        }
    }
}
