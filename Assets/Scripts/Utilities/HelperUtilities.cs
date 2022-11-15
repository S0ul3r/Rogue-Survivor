using System.Collections;
using UnityEngine;

public static class HelperUtilities
{
    /// <summary>
    /// Empty string debug check
    /// </summary>
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.LogError(thisObject.name.ToString() + ": " + fieldName + " is empty and must contain a value to be used.");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check for null values
    /// </summary>
    public static bool ValidateCheckNull(Object thisObject, string fieldName, Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.LogError(thisObject.name.ToString() + ": " + fieldName + " is null and must contain a value to be used.");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check for empty list or contain null value check - returns error = true if empty or contains null
    /// </summary> 
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableChecktoObject)
    {
        bool error = false;
        int count = 0;

        if (enumerableChecktoObject == null)
        {
            Debug.Log(fieldName + " is null" + thisObject.name.ToString());
            return true;
        }

        foreach (var item in enumerableChecktoObject)
        {
            // Check for null
            if (item == null)
            {
                error = true;
                Debug.LogError(thisObject.name.ToString() + ": " + fieldName + " contains null values.");
            }
            else
            {
                count++;
            }
        }

        // Check for empty
        if (count == 0)
        {
            error = true;
            Debug.LogError(thisObject.name.ToString() + ": " + fieldName + " is empty and must contain a value to be used.");
        }

        return error;
    }

    /// <summary>
    /// check for positive values - returns error = true if negative, isZeroAllowed and param
    /// </summary>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;
        
        if (isZeroAllowed && valueToCheck < 0)
        {
            error = true;
            Debug.LogError(thisObject.name.ToString() + ": " + fieldName + " is negative and must be positive or zero.");
        }

        if (!isZeroAllowed && valueToCheck <= 0)
        {
            error = true;
            Debug.LogError(thisObject.name.ToString() + ": " + fieldName + " is zero or negative and must be greater than zero.");
        }

        return error;
    }

    /// <summary>
    /// Get neares spawn point form position
    /// </summary>
    public static Vector3 GetNearestSpawnPoint(Vector3 playerPos)
    {
        Room currRoom = GameManager.Instance.GetCurrentRoom();

        // get nearest spawn point to player
        Grid grid = currRoom.instantiatedRoom.grid;

        // initialise vector3 variable with huge number
        Vector3 nearestSpawnPoint = new Vector3(10000f, 10000f, 0f);

        // loop through all spawn points
        foreach (Vector2Int spawnPosGrid in currRoom.spawnPositionArray)
        {
            // get world position of spawn point
            Vector3 spawnPosWorld = grid.CellToWorld((Vector3Int)spawnPosGrid);

            // check if spawn point is closer than current nearest spawn point
            if (Vector3.Distance(playerPos, spawnPosWorld) < Vector3.Distance(playerPos, nearestSpawnPoint))
            {
                nearestSpawnPoint = spawnPosWorld;
            }
        }

        return nearestSpawnPoint;
    }
}
