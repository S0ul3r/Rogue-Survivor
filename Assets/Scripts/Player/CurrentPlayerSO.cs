using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrentPlayerSO_", menuName = "ScriptableObjects/Player/CurrentPlayerSO", order = 1)]
public class CurrentPlayerSO : ScriptableObject
{
    public PlayerDetailsSO playerDetails;
    public string playerName;
}
