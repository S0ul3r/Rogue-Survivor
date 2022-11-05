using UnityEngine;
[System.Serializable]

public class Doorway
{
    public Vector2Int position;
    public Direction direction;
    public GameObject doorPrefab;

    #region Header
    [Header("Upper left pos of doorway to start copying from")]
    #endregion

    public Vector2Int doorwayStartCopyPos;
    
    #region Header
    [Header("Width of doorway to copy over")]
    #endregion
    
    public int doorwayCopyWidth;
    
    #region Header
    [Header("Height of doorway to copy over")]
    #endregion
    
    public int doorwayCopyHeight;
    
    [HideInInspector]
    public bool isConnected = false;

    [HideInInspector]
    public bool isUnavailable = false;
}
