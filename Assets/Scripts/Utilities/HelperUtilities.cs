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
}
