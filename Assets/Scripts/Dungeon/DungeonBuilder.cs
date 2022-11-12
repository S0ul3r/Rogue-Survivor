using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonoBehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungBuilderRoomDict = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDict = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungBuildComplete;

    protected override void Awake()
    {
        base.Awake();

        // Load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;

        // Set dimmed material to visible
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// Generate dungeon, return true if dung has been successfully built
    /// </summary>
    public bool GenerateDungeon(DungeonLevelSO currentDungLevel)
    {
        // Load room templates
        roomTemplateList = currentDungLevel.roomTemplateList;

        // Load roomTemplatesSO into dictionary
        LoadRoomTemplatesIntoDict();

        // Create dungeon
        dungBuildComplete = false;
        int dungBuildAttempts = 0;

        while (!dungBuildComplete && dungBuildAttempts < Settings.maxDungBuildAttempts)
        {
            dungBuildAttempts++;

            // Select random room node graph
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungLevel.roomNodeGraphList);

            int dungRebuildAttemptsForRoomGraph = 0;
            dungBuildComplete = false;

            // Build dungeon untill dungBuildComplete is true or maxDungRebuildAttemptsForRoomGraph is reached
            while (!dungBuildComplete && dungRebuildAttemptsForRoomGraph < Settings.maxDungRebuildAttemptsForRoomGraph)
            {
                // Clear dungeon
                ClearDungeon();

                dungRebuildAttemptsForRoomGraph++;

                // Try to build a random dungeon for selected room node graph
                dungBuildComplete = TryToBuildRandomDung(roomNodeGraph);
            }

            if (dungBuildComplete)
            {
                // Instantiate Room gameObjects
                InstantiateRoomGameObjects();
            }
        }
        return dungBuildComplete;
    }

    /// <summary
    /// Load room templates into dictionary
    /// </summary>
    private void LoadRoomTemplatesIntoDict()
    {
        // Clear room temp dict
        roomTemplateDict.Clear();

        // Load room templates into dict
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDict.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDict.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicated Room Template In " + roomTemplateList);
            }
        }
    }

    /// <summary>
    /// Select random room node graph from our list of them\
    /// </summary>
    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        // Select random room node graph
        int randomIndex = Random.Range(0, roomNodeGraphList.Count);
        if (roomNodeGraphList.Count > 0)
        {
            RoomNodeGraphSO roomNodeGraph = roomNodeGraphList[randomIndex];
            return roomNodeGraph;
        }
        else
        {
            Debug.Log("List of room node graphs is empty");
            return null;
        }
    }

    /// <summary>
    /// Instantiate Room Game Objects from prefabs
    /// </summary>
    private void InstantiateRoomGameObjects()
    {
        
    }

    /// <summary>
    /// Get room template by ID
    /// </summary>
    public RoomTemplateSO GetRoomTemplateByID(string roomTemplateID)
    {
        if (roomTemplateDict.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        else
        {
            Debug.Log("Room template with ID " + roomTemplateID + " not found");
            return null;
        }
    }

    /// <summary>
    /// Get room by roomID
    /// </summary>
    public Room GetRoomByID(string roomID)
    {
        if (dungBuilderRoomDict.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        else
        {
            Debug.Log("Room with ID " + roomID + " not found");
            return null;
        }
    }

    /// <summary>
    /// Clear dungeon from all rooms and dungeon room dictionary
    /// </summary>
    private void ClearDungeon()
    {
        // Destroy dungeon game objects and clear dungBuilderRoomDict dictionary
        if (dungBuilderRoomDict.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyValuePair in dungBuilderRoomDict)
            {
                Room room = keyValuePair.Value;

                if (room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }
            dungBuilderRoomDict.Clear();
        }
    }

    /// <summary>
    /// Try to randomly build dungeon for selected RoomNodeGraph, return true if layout was created successfully
    /// </summary>
    private bool TryToBuildRandomDung(RoomNodeGraphSO roomNodeGraph)
    {
        // Create Queue for room nodes
        Queue<RoomNodeSO> roomNodeQueue = new Queue<RoomNodeSO>();

        // Add Entrance Node to the queue
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNodeByType(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null)
        {
            roomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("No Entrance Node");
            // dungeon not built
            return false;
        }

        bool noRoomOverlaps = true;

        // process room nodes queue
        noRoomOverlaps = ProcessRoomsInRoomNodeQueue(roomNodeGraph, roomNodeQueue, noRoomOverlaps);

        if (roomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Process rooms in room node queue, true if there are 0 overlaps
    /// </summary>
    private bool ProcessRoomsInRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> roomNodeQueue, bool noRoomOverlaps)
    {
        // lopp when queue is filled and no overlaps detected
        while (roomNodeQueue.Count > 0 && noRoomOverlaps)
        {
            // Get next room node from queue
            RoomNodeSO roomNode = roomNodeQueue.Dequeue();

            // add all child room nodes from the list
            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                roomNodeQueue.Enqueue(childRoomNode);
            }

            // if the room is rntrance mark as positioned and add to dict
            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);
                room.isPositioned = true;
                dungBuilderRoomDict.Add(room.id, room);
            }
            // if room isnt an entrance
            else
            {
                // get parent room for node
                Room parentRoom = dungBuilderRoomDict[roomNode.parentRoomNodeIDList[0]];

                // check if it can be placed with no overlaps
                noRoomOverlaps = CanBePlacedWithoutOverlaps(roomNode, parentRoom);
            }
        }
        return noRoomOverlaps;
    }

    /// <summary>
    /// Check if room can be placed without overlaps
    /// </summary>
    private bool CanBePlacedWithoutOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {
        // flag
        bool roomOverlaps = true;

        // loop as long as there are overlaps
        while (roomOverlaps)
        {
            // Select random available doorway for parent\
            List<Doorway> availableParentDoorwayList = GetAvailableDoorways(parentRoom.doorWayList).ToList();

            if (availableParentDoorwayList.Count == 0)
                return false;

            // choose random availableParentDoorwayList
            Doorway doorwayParent = availableParentDoorwayList[Random.Range(0, availableParentDoorwayList.Count)];

            // Get random room template for chosen room node that fits to parent
            RoomTemplateSO roomTemplate = GetRandomTemplateForRoomFittingToParent(roomNode, doorwayParent);

            // create a room
            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            // try to place the room - true if no overlaps
            if (PlaceRoom(parentRoom, doorwayParent, room))
            {
                // if no overlap change flag, position room flag and add to dict
                roomOverlaps = false;
                room.isPositioned = true;
                dungBuilderRoomDict.Add(room.id, room);
            }
            else
            {
                roomOverlaps = true;
            }
        }
        return true; // no overlaps
    }

    /// <summary>
    /// Get avaiable doorawy
    /// </summary>
    private IEnumerable<Doorway> GetAvailableDoorways(List<Doorway> roomDoorwayList)
    {
        // Loop doorway list
        foreach(Doorway doorway in roomDoorwayList)
        {
            if (!doorway.isUnavailable && !doorway.isConnected)
                yield return doorway;
        }
    }

    /// <summary>
    /// Get random room template for chosen room node that fits to parent
    /// </summary>
    private RoomTemplateSO GetRandomTemplateForRoomFittingToParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomTemplate = null;

        // if room node is a corridor then get random corridor template
        if (roomNode.roomNodeType.isCorridor)
        {
            // check orientation
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;
                    
                case Orientation.east:
                case Orientation.west:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;
                    
                case Orientation.none:
                    break;

                default:
                    break;
            }
        }
        else
        {
            // get random room template for room node
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomTemplate;
    }

    /// <summary>
    /// Place room - true if no overlap
    /// </summary>
    private bool PlaceRoom (Room parentRoom, Doorway doorwayParent, Room room)
    {
        // get curr room dorway pos
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

        // Return false if no doorway found
        if (doorway == null)
        {
            // Mark parent as unavailable so we dont try to place another room on it
            doorwayParent.isUnavailable = true;

            return false;
        }

        // calculate position in grid of parent doorway
        Vector2Int parentDoorwayPos = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        // calculate adjustment based on doorway pos that we try to connect
        switch (doorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;
                
            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;

            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;

            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;

            case Orientation.none:
                break;

            default:
                break;
        }

        // calculate room lower and upper bounds
        room.lowerBounds = parentDoorwayPos + room.templateLowerBounds + adjustment - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        // check if room overlaps with any other room
        Room overlapRoomCheck = CheckForOverlaps(room);

        if (overlapRoomCheck == null)
        {
            // dorways are connected and unavailable
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            // we managed to place a room wuthout overlap
            return true;
        }
        else
        {
            // mark parent as unavailable so we dont try to place another room on it
            doorwayParent.isUnavailable = true;

            return false;
        }
    }

    /// <summary>
    /// Check For room Overlaps
    /// </summary>
    private Room CheckForOverlaps(Room testRoom)
    {
        // loop through all rooms
        foreach (KeyValuePair<string, Room> keyvaluepair in dungBuilderRoomDict)
        {
            Room room = keyvaluepair.Value;

            // skip if it is the same room as testROom
            if (room.id == testRoom.id || !room.isPositioned)
                continue;

            if (roomIsOverlapping(testRoom, room))
                return room;
        }

        // if no overlaps return null
        return null;
    }

    /// <summary>
    /// room Is Overlapping check
    /// </summary>
    private bool roomIsOverlapping(Room room1, Room room2)
    {
        // check if room1 is to the left of room2
        if (room1.lowerBounds.x > room2.upperBounds.x)
            return false;

        // check if room1 is to the right of room2
        if (room1.upperBounds.x < room2.lowerBounds.x)
            return false;

        // check if room1 is above room2
        if (room1.lowerBounds.y > room2.upperBounds.y)
            return false;

        // check if room1 is below room2
        if (room1.upperBounds.y < room2.lowerBounds.y)
            return false;

        // if none of the above return true
        return true;
    }

    /// <summary>
    /// GetOppositeDoorway - return opposite doorway to parent
    /// </summary>
    private Doorway GetOppositeDoorway(Doorway doorwayParent, List<Doorway> roomDoorwayList)
    {
        // check all orientations between parent and doorway we check
        foreach (Doorway doorwayCheck in roomDoorwayList)
        {
            if (doorwayParent.orientation == Orientation.east && doorwayCheck.orientation == Orientation.west)
                return doorwayCheck;
            else if (doorwayParent.orientation == Orientation.west && doorwayCheck.orientation == Orientation.east)
                return doorwayCheck;
            else if (doorwayParent.orientation == Orientation.north && doorwayCheck.orientation == Orientation.south)
                return doorwayCheck;
            else if (doorwayParent.orientation == Orientation.south && doorwayCheck.orientation == Orientation.north)
                return doorwayCheck;
        }
        return null;
    }

    /// <summary>
    /// Get random room tmplate
    /// </summary>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplatesList = new List<RoomTemplateSO>();

        // loop template list
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            // add matching templates
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplatesList.Add(roomTemplate);
            }
        }
        // return null if list is zero
        if (matchingRoomTemplatesList.Count == 0)
            return null;

        return matchingRoomTemplatesList[UnityEngine.Random.Range(0, matchingRoomTemplatesList.Count)];
    }

    /// <summary>
    /// Create a random room node graph from the list
    /// </summary>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        // Initialise room from template
        Room room = new Room();

        // load variables from template
        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.prefab = roomTemplate.prefab;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        
        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorWayList = CopyDoorwayList(roomTemplate.doorwayList);

        // Set parent ID for room for Entrance
        if (roomNode.parentRoomNodeIDList.Count == 0)
        {
            room.parentRoomID = "";
            room.isAlreadyVisited = true;
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        return room;
    }

    /// <summary>
    /// Copy string list
    /// </summary> 
    private List<string> CopyStringList(List<string> stringList)
    {
        List<string> copyStringList = new List<string>();

        foreach (string str in stringList)
        {
            copyStringList.Add(str);
        }

        return copyStringList;
    }

    /// <summary>
    /// Copy Doorway List
    /// </summary>
    private List<Doorway> CopyDoorwayList(List<Doorway> doorwayList)
    {
        List<Doorway> copyDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in doorwayList)
        {
            Doorway newDoorway = new Doorway();

            // copy all variables
            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            copyDoorwayList.Add(doorway);
        }

        return copyDoorwayList;
    }
}
