using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/DungeonLevel")]
public class DungeonLevelSO : ScriptableObject
{
    #region Header BASIC LEVEL DETAILS
    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]
    #endregion Header BASIC LEVEL DETAILS

    #region Tooltip
    [Tooltip("The name of the level.")]
    #endregion Tooltip

    public string levelName;

    #region Header ROOM TEMPLATES FOR LEVEL
    [Space(10)]
    [Header("List populated with room templates that will be part of the level. Templates must be included for all room node types in Room Node Graph.")]
    #endregion Header ROOM TEMPLATES FOR LEVEL
    public List<RoomTemplateSO> roomTemplateList;

    #region Header ROOM NODE GRAPH FOR LEVEL
    [Space(10)]
    [Header("Room Node Graphs for the level.")]
    #endregion Header ROOM NODE GRAPH FOR LEVEL
    #region Tooltip
    [Tooltip("List with room node graphs which will be randomly selected for level.")]
    #endregion
    public List<RoomBlockGraphSO> roomNodeGraphList;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Check for empty string
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);

        // Check for empty list or contain null value
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
            return;

        // Check for empty list or contain null value
        if (HelperUtilities.ValidateCheckEnumerableValues(this, "roomNodeGraphList", roomNodeGraphList))
            return;

        // Make sure that room templates are specified for all node types in room node graph
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        // loop through room templates to check that it's node type ha been specified
        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
                return;

            if (roomTemplateSO.roomNodeType.isCorridorEW)
                isEWCorridor = true;

            if (roomTemplateSO.roomNodeType.isCorridorNS)
                isNSCorridor = true;

            if (roomTemplateSO.roomNodeType.isEntrance)
                isEntrance = true;
        }

        if (isEWCorridor == false)
            Debug.Log(this.name.ToString() + ": " + nameof(roomTemplateList) + " does not contain a room template with a corridor EW node type.");

        if (isNSCorridor == false)
            Debug.Log(this.name.ToString() + ": " + nameof(roomTemplateList) + " does not contain a room template with a corridor NS node type.");

        if (isEntrance == false)
            Debug.Log(this.name.ToString() + ": " + nameof(roomTemplateList) + " does not contain a room template with an entrance node type.");

        // Loop through room node graphs to check that it's node type has been specified
        foreach (RoomBlockGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null)
                return;

            // Check that room template has been selected for each node type, we can skip corridors and entrance
            foreach (RoomBlockSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS || roomNodeSO.roomNodeType.isEntrance
                    || roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone)
                    continue;

                // flag for room nod type found
                bool roomNodeTypeFound = false;

                // Loop through room templates to check that it's node type ha been specified
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {
                    if (roomTemplateSO == null)
                        continue;

                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                    {
                        roomNodeTypeFound = true;
                        break;
                    }
                }

                if (roomNodeTypeFound == false)
                    Debug.Log(this.name.ToString() + ": " + nameof(roomTemplateList) + " does not contain a room template with a " + roomNodeSO.roomNodeType.name.ToString() + " node type.");
            }
        }
    }

#endif
    #endregion Validation
}