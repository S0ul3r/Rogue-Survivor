using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public Bounds roomColliderBounds;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        // Save room collider bounds
        roomColliderBounds = boxCollider2D.bounds;

    }

    /// <summary>
    /// Initialise The Instantiated Room
    /// </summary>
    public void Initialise(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);

        BlockUnconnectedDoorways();

        DisableCollisionTilemapRenderer();

    }

    /// <summary>
    /// Populate the tilemap and grid memeber variables.
    /// </summary>
    private void PopulateTilemapMemberVariables(GameObject roomGameobject)
    {
        // Get the grid component.
        grid = roomGameobject.GetComponentInChildren<Grid>();

        // Get tilemaps in children.
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.tag == "groundTilemap")
            {
                groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration1Tilemap")
            {
                decoration1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration2Tilemap")
            {
                decoration2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "frontTilemap")
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "collisionTilemap")
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "minimapTilemap")
            {
                minimapTilemap = tilemap;
            }

        }

    }

    /// <summary>
    /// Block doorways that dont connect to any room
    /// </summary>
    private void BlockUnconnectedDoorways()
    {
        // Loop through each doorway
        foreach (Doorway doorway in room.doorWayList)
        {
            // If the doorway is not connected to a room
            if (doorway.isConnected)
                continue;
            
            // Block unconnected doorways with tiles on tilemaps
            if (collisionTilemap != null)
            {
                BlockDoorwayOnTilemap(collisionTilemap, doorway);
            }
            if (groundTilemap != null)
            {
                BlockDoorwayOnTilemap(groundTilemap, doorway);
            }
            if (minimapTilemap != null)
            {
                BlockDoorwayOnTilemap(minimapTilemap, doorway);
            }
            if (decoration1Tilemap != null)
            {
                BlockDoorwayOnTilemap(decoration1Tilemap, doorway);
            }
            if (decoration2Tilemap != null)
            {
                BlockDoorwayOnTilemap(decoration2Tilemap, doorway);
            }
            if (frontTilemap != null)
            {
                BlockDoorwayOnTilemap(frontTilemap, doorway);
            }
        }
    }

    /// <summary>
    /// Block Doorway on tilemap layer
    /// </summary>
    private void BlockDoorwayOnTilemap(Tilemap tilemap, Doorway doorway)
    {
        // switch depending on orientation of a door call the correct function to block doorway
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayOnTilemapHorizontal(tilemap, doorway);
                break;

            case Orientation.east:
            case Orientation.west:
                BlockDoorwayOnTilemapVertical(tilemap, doorway);
                break;

            case Orientation.none:
                break;
        }
    }

    /// <summary>
    /// Block Doorway on tilemap layer Horizontal
    /// </summary>
    private void BlockDoorwayOnTilemapHorizontal(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPos = doorway.doorwayStartCopyPosition;

        // loop all tiles to copy
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                // get rotation of tile to copy
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPos.x + xPos, startPos.y - yPos, 0));

                // Coppy tile
                tilemap.SetTile(new Vector3Int(startPos.x + 1 + xPos, startPos.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPos.x + xPos, startPos.y - yPos, 0)));

                // set rotation of copied tile
                tilemap.SetTransformMatrix(new Vector3Int(startPos.x + 1 + xPos, startPos.y - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Block Doorway on tilemap layer Vertical
    /// </summary>
    private void BlockDoorwayOnTilemapVertical(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPos = doorway.doorwayStartCopyPosition;

        // loop all tiles to copy
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                // get rotation of tile to copy
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPos.x + xPos, startPos.y - yPos, 0));

                // Coppy tile
                tilemap.SetTile(new Vector3Int(startPos.x + xPos, startPos.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPos.x + xPos, startPos.y - yPos, 0)));

                // set rotation of copied tile
                tilemap.SetTransformMatrix(new Vector3Int(startPos.x + xPos, startPos.y - 1 - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Disable collision tilemap renderer
    /// </summary>
    private void DisableCollisionTilemapRenderer()
    {
        // Disable collision tilemap renderer
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;

    }

}
