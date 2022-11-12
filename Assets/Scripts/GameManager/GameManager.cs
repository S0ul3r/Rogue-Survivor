using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region Header DUNGEON LEVELS
    [Space(10)]
    [Header("DUNGEON LEVELS")]
    #endregion Header DUNGEON LEVELS

    #region Tooltip
    [Tooltip("Populate list with dungeon level scriptable objects")]
    #endregion Tooltip

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip
    [Tooltip("Which level is currently played, first level is 0")]
    #endregion Tooltip

    [SerializeField] private int currentLevelListIndex = 0;

    [HideInInspector] public GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.gameStart;
    }

    // Update is called once per frame
    void Update()
    {
        HandleGameState();

        // check if R is pressed (for testing purposes), del later
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStart;
        }
    }

    /// <summary>
    /// Handles the game state
    /// </summary>
    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.gameStart:
                // play intro
                PlayDungeonLevel(currentLevelListIndex);
                // set game state to playing
                gameState = GameState.playing;
                break;
            case GameState.gameWon:
                break;
            case GameState.gameOver:
                break;
            case GameState.gamePaused:
                break;
            case GameState.playing:
                break;
            case GameState.engagingEnemies:
                break;
            case GameState.engagingBoss:
                break;
            case GameState.levelCompleted:
                break;
            case GameState.dungeonOverviewMap:
                break;
            case GameState.restartGame:
                break;
            case GameState.end:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Play dungeon level for intro
    /// </summary>
    private void PlayDungeonLevel(int currentLevelListIndex)
    {
        // build dung level
        bool dungeonBuiltComplete = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[currentLevelListIndex]);

        if (!dungeonBuiltComplete)
        {
            Debug.LogError("Dungeon level not built correctly, check spcified rooms and node graphs");
        }
    }

    /// <summary>
    /// Validation to check if dungeon level list is populated
    /// </summary>
    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (dungeonLevelList.Count == 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
        }
    }
#endif
    #endregion Validationn
}
