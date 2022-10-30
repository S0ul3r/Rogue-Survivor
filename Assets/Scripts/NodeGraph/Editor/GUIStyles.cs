using UnityEditor;
using UnityEngine;

public class GUIStyles
{
    public GUIStyle entranceNodeStyle = new GUIStyle();
    public GUIStyle entranceNodeSelectedStyle = new GUIStyle();

    public GUIStyle roomNodeStyle = new GUIStyle();
    public GUIStyle roomNodeSelectedStyle = new GUIStyle();

    public GUIStyle bossRoomNodeStyle = new GUIStyle();
    public GUIStyle bossRoomNodeSelectedStyle = new GUIStyle();

    public GUIStyle corridorNodeStyle = new GUIStyle();
    public GUIStyle corridorNodeSelectedStyle = new GUIStyle();

    public GUIStyle chestRoomNodeStyle = new GUIStyle();
    public GUIStyle chestRoomNodeSelectedStyle = new GUIStyle();

    // Node layout values
    private const int NodePadding = 20; // Spacing inside the GUI element
    private const int NodeBorder = 12; // Spacing outside the GUI element

    public void Initialize()
    {
        SetupEntranceNodeStyle();
        SetupRoomNodeStyle();
        SetupBossRoomNodeStyle();
        SetupCorridorNodeStyle();
        SetupChestRoomNodeStyle();

        void SetupEntranceNodeStyle()
        {
            entranceNodeStyle = new GUIStyle();
            entranceNodeStyle.normal.background = EditorGUIUtility.Load("node2") as Texture2D;
            entranceNodeStyle.normal.textColor = Color.white;
            entranceNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            entranceNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            entranceNodeSelectedStyle = new GUIStyle();
            entranceNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node2 on") as Texture2D;
            entranceNodeSelectedStyle.normal.textColor = Color.white;
            entranceNodeSelectedStyle.padding = entranceNodeStyle.padding;
            entranceNodeSelectedStyle.border = entranceNodeStyle.border;
        }

        void SetupRoomNodeStyle()
        {
            roomNodeStyle = new GUIStyle();
            roomNodeStyle.normal.background = EditorGUIUtility.Load("node5") as Texture2D;
            roomNodeStyle.normal.textColor = Color.white;
            roomNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            roomNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            roomNodeSelectedStyle = new GUIStyle();
            roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node5 on") as Texture2D;
            roomNodeSelectedStyle.normal.textColor = Color.white;
            roomNodeSelectedStyle.padding = roomNodeStyle.padding;
            roomNodeSelectedStyle.border = roomNodeStyle.border;
        }

        void SetupBossRoomNodeStyle()
        {
            bossRoomNodeStyle = new GUIStyle();
            bossRoomNodeStyle.normal.background = EditorGUIUtility.Load("node6") as Texture2D;
            bossRoomNodeStyle.normal.textColor = Color.black;
            bossRoomNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            bossRoomNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            bossRoomNodeSelectedStyle = new GUIStyle();
            bossRoomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node6 on") as Texture2D;
            bossRoomNodeSelectedStyle.normal.textColor = Color.black;
            bossRoomNodeSelectedStyle.padding = bossRoomNodeStyle.padding;
            bossRoomNodeSelectedStyle.border = bossRoomNodeStyle.border;
        }

        void SetupCorridorNodeStyle()
        {
            corridorNodeStyle = new GUIStyle();
            corridorNodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            corridorNodeStyle.normal.textColor = Color.white;
            corridorNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            corridorNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            corridorNodeSelectedStyle = new GUIStyle();
            corridorNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node0 on") as Texture2D;
            corridorNodeSelectedStyle.normal.textColor = Color.white;
            corridorNodeSelectedStyle.padding = corridorNodeStyle.padding;
            corridorNodeSelectedStyle.border = corridorNodeStyle.border;
        }

        void SetupChestRoomNodeStyle()
        {
            chestRoomNodeStyle = new GUIStyle();
            chestRoomNodeStyle.normal.background = EditorGUIUtility.Load("node4") as Texture2D;
            chestRoomNodeStyle.normal.textColor = Color.black;
            chestRoomNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            chestRoomNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            chestRoomNodeSelectedStyle = new GUIStyle();
            chestRoomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node4 on") as Texture2D;
            chestRoomNodeSelectedStyle.normal.textColor = Color.black;
            chestRoomNodeSelectedStyle.padding = chestRoomNodeStyle.padding;
            chestRoomNodeSelectedStyle.border = chestRoomNodeStyle.border;
        }

    }
}