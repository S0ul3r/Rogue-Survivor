using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonoBehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungBuilderRoomDict = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDict = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomBlockTypeListSO roomNodeTypeList;
    private bool dungBuildComplete;

    protected override void Awake()
    {
        base.Awake();

        // Load the room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;

        // Set dimmed material to fully visible
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
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
            RoomBlockGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungLevel.roomNodeGraphList);

            int dungRebuildAttemptsForRoomGraph = 0;
            dungBuildComplete = false;

            // Build dungeon untill dungBuildComplete is true or maxDungRebuildAttemptsForRoomGraph is reached
            while (!dungBuildComplete && dungRebuildAttemptsForRoomGraph <= Settings.maxDungRebuildAttemptsForRoomGraph)
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
                InstantiateRoomGameobjects();
            }
        }

        return dungBuildComplete;
    }

    /// <summary>
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
    private bool TryToBuildRandomDung(RoomBlockGraphSO roomNodeGraph)
    {

        // Create Queue for room nodes
        Queue<RoomBlockSO> roomNodeQueue = new Queue<RoomBlockSO>();

        // Add Entrance Node To Room Node Queue From Room Node Graph
        RoomBlockSO entranceNode = roomNodeGraph.GetRoomNodeByType(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null)
        {
            roomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("No Entrance Node");
            return false;  // Dungeon Not Built
        }

        // Start with no room overlaps
        bool noRoomOverlaps = true;


        // Process open room nodes queue
        noRoomOverlaps = ProcessRoomsInRoomNodeQueue(roomNodeGraph, roomNodeQueue, noRoomOverlaps);

        // If all the room nodes have been processed and there hasn't been a room overlap then return true
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
    private bool ProcessRoomsInRoomNodeQueue(RoomBlockGraphSO roomNodeGraph, Queue<RoomBlockSO> roomNodeQueue, bool noRoomOverlaps)
    {
        // lopp when queue is filled and no overlaps detected
        while (roomNodeQueue.Count > 0 && noRoomOverlaps)
        {
            // Get next room node from queue
            RoomBlockSO roomNode = roomNodeQueue.Dequeue();

            // add all child room nodes from the list
            foreach (RoomBlockSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
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
    private bool CanBePlacedWithoutOverlaps(RoomBlockSO roomNode, Room parentRoom)
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

        return true;  // no room overlaps

    }

    /// <summary>
    /// Get random room template for chosen room node that fits to parent
    /// </summary>
    private RoomTemplateSO GetRandomTemplateForRoomFittingToParent(RoomBlockSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomTemplate = null;

        // if room node is a corridor then get random corridor template
        if (roomNode.roomNodeType.isCorridor)
        {
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
        // get random room template for room node
        else
        {
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomTemplate;
    }


    /// <summary>
    /// Place room - true if no overlap
    /// </summary>
    private bool PlaceRoom(Room parentRoom, Doorway doorwayParent, Room room)
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

        Room overlapRoomCheck = CheckForRoomOverlap(room);

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
    private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
    {

        foreach (Doorway doorwayToCheck in doorwayList)
        {
            if (parentDoorway.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
            {
                return doorwayToCheck;
            }
        }

        return null;

    }


    /// <summary>
    /// Check for rooms that overlap the upper and lower bounds parameters, and if there are overlapping rooms then return room else return null
    /// </summary>
    private Room CheckForRoomOverlap(Room testRoom)
    {
        // Iterate through all rooms
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
    /// Check if 2 rooms overlap each other - return true if they overlap or false if they don't overlap
    /// </summary>
    private bool roomIsOverlapping(Room room1, Room room2)
    {
        bool isOverlappingX = IsOverLappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);

        bool isOverlappingY = IsOverLappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

        if (isOverlappingX && isOverlappingY)
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    /// <summary>
    /// Check if interval 1 overlaps interval 2 - this method is used by the roomIsOverlapping method
    /// </summary>
    private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Get a random room template from the roomtemplatelist that matches the roomType and return it
    /// (return null if no matching room templates found).
    /// </summary>
    private RoomTemplateSO GetRandomRoomTemplate(RoomBlockTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        // loop template list
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            // add matching templates
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        // Return null if list is zero
        if (matchingRoomTemplateList.Count == 0)
            return null;

        // Select random room template from list and return
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];

    }


    /// <summary>
    /// Get unconnected doorways
    /// </summary>
    private IEnumerable<Doorway> GetAvailableDoorways(List<Doorway> roomDoorwayList)
    {
        // Loop through doorway list
        foreach (Doorway doorway in roomDoorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
                yield return doorway;
        }
    }


    /// <summary>
    /// Create room based on roomTemplate and layoutNode, and return the created room
    /// </summary>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomBlockSO roomNode)
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

            // set entrance in game manager
            GameManager.Instance.SetCurrentRoom(room);
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
    private RoomBlockGraphSO SelectRandomRoomNodeGraph(List<RoomBlockGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        }
        else
        {
            Debug.Log("No room node graphs in list");
            return null;
        }
    }


    /// <summary>
    /// Create deep copy of doorway list
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

            copyDoorwayList.Add(newDoorway);
        }

        return copyDoorwayList;
    }


    /// <summary>
    /// Create deep copy of string list
    /// </summary>
    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }

        return newStringList;
    }

    /// <summary>
    /// Instantiate the dungeon room gameobjects from the prefabs
    /// </summary>
    private void InstantiateRoomGameobjects()
    {
        // Iterate through all dungeon rooms.
        foreach (KeyValuePair<string, Room> keyvaluepair in dungBuilderRoomDict)
        {
            Room room = keyvaluepair.Value;

            // Calculate room position (remember the room instantiatation position needs to be adjusted by the room template lower bounds)
            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

            // Instantiate room
            GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            // Get instantiated room component from instantiated prefab.
            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;

            // Initialise The Instantiated Room
            instantiatedRoom.Initialise(roomGameobject);

            // Save gameobject reference.
            room.instantiatedRoom = instantiatedRoom;
        }
    }


    /// <summary>
    /// Get a room template by room template ID, returns null if ID doesn't exist
    /// </summary>
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if (roomTemplateDict.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get room by roomID, if no room exists with that ID return null
    /// </summary>
    public Room GetRoomByRoomID(string roomID)
    {
        if (dungBuilderRoomDict.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    /// Clear dungeon room gameobjects and dungeon room dictionary
    /// </summary>
    private void ClearDungeon()
    {
        // Destroy instantiated dungeon gameobjects and clear dungeon manager room dictionary
        if (dungBuilderRoomDict.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyvaluepair in dungBuilderRoomDict)
            {
                Room room = keyvaluepair.Value;

                if (room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }

            dungBuilderRoomDict.Clear();
        }
    }
}