using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow
{
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    public static void OpenWindow()
    {
        // Get existing open window or if none, make a new one:
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }
}
