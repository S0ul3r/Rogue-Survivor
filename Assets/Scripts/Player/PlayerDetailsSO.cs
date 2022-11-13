using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetailsSO_", menuName = "ScriptableObjects/Player/PlayerDetailsSO", order = 1)]
public class PlayerDetailsSO : ScriptableObject
{
    #region Header PLAYER DETAILS
    [Space(10)]
    [Header("PLAYER DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("The player's name.")]
    #endregion
    public string playerName;

    #region Tooltip
    [Tooltip("Prefab for player gameobject.")]
    #endregion
    public GameObject playerPrefab;

    #region Tooltip
    [Tooltip("Player runtime animator controler")]
    #endregion
    public RuntimeAnimatorController playerAnimatorController;

    #region Header HEALTH
    [Space(10)]
    [Header("HEALTH")]
    #endregion
    #region Tooltip
    [Tooltip("Player's starting health.")]
    #endregion
    public int playerStartingHealth;

    #region Header OTHER
    [Space(10)]
    [Header("OTHER")]
    #endregion
    #region Tooltip
    [Tooltip("Player's icon on minimap.")]
    #endregion
    public Sprite playerMinimapIcon;

    #region Tooltip
    [Tooltip("Player's hand sprite.")]
    #endregion
    public Sprite playerHandSprite;

    // ideas for more tooltips from AI
    /*[Header("Player Details")]
    [SerializeField] private int playerLevel = 0;
    [SerializeField] private int playerExperience = 0;
    [SerializeField] private int playerGold = 0;
    [SerializeField] private int playerHealth = 0;
    [SerializeField] private int playerMaxHealth = 0;
    [SerializeField] private int playerAttack = 0;
    [SerializeField] private int playerDefense = 0;
    [SerializeField] private int playerSpeed = 0;
    [SerializeField] private int playerLuck = 0;
    [SerializeField] private int playerCriticalChance = 0;
    [SerializeField] private int playerCriticalDamage = 0;
    [SerializeField] private int playerAccuracy = 0;
    [SerializeField] private int playerEvasion = 0;
    [SerializeField] private int playerMaxWeight = 0;
    [SerializeField] private int playerCurrentWeight = 0;
    [SerializeField] private int playerMaxInventory = 0;
    [SerializeField] private int playerCurrentInventory = 0;
    [SerializeField] private int playerMaxEquipment = 0;
    [SerializeField] private int playerCurrentEquipment = 0;
    [SerializeField] private int playerMaxConsumables = 0;
    [SerializeField] private int playerCurrentConsumables = 0;
    [SerializeField] private int playerMaxQuests = 0;
    [SerializeField] private int playerCurrentQuests = 0;
    [SerializeField] private int playerMaxSkills = 0;
    [SerializeField] private int playerCurrentSkills = 0;
    [SerializeField] private int playerMaxSpells = 0;
    [SerializeField] private int playerCurrentSpells = 0;
    [SerializeField] private int playerMaxAbilities = 0;
    [SerializeField] private int playerCurrentAbilities = 0;
    [SerializeField] private int playerMaxStatusEffects = 0;
    [SerializeField] private int playerCurrentStatusEffects = 0;
    [SerializeField] private int playerMaxBuffs = 0;
    [SerializeField] private int playerCurrentBuffs = 0;
    [SerializeField] private int playerMaxDebuffs = 0;
    [SerializeField] private int playerCurrentDebuffs = 0;
    [SerializeField] private int playerMaxPassives = 0;*/

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Check for empty strings
        HelperUtilities.ValidateCheckEmptyString(this, "playerName", playerName);

        // Check for null values
        HelperUtilities.ValidateCheckNull(this, "playerPrefab", playerPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, "playerStartingHealth", playerStartingHealth, false);
        HelperUtilities.ValidateCheckNull(this, "playerAnimatorController", playerAnimatorController);
        HelperUtilities.ValidateCheckNull(this, "playerMinimapIcon", playerMinimapIcon);
        HelperUtilities.ValidateCheckNull(this, "playerHandSprite", playerHandSprite);
    }
#endif
    #endregion
}
