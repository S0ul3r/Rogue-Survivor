using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemaMachineTarget : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;
    public CinemachineTargetGroup.Target target;

    private void Awake()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCMTargetGroup();
    }

    /// <summary>
    /// Set CM Target Group
    /// </summary>
    private void SetCMTargetGroup()
    {
        // create target group
        CinemachineTargetGroup.Target cinemachineGroupTarget_plater = new CinemachineTargetGroup.Target { weight = 1f, radius = 1f, 
            target = GameManager.Instance.GetPlayer().transform };

        // add target to array
        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] { cinemachineGroupTarget_plater };

        // set target group
        targetGroup.m_Targets = cinemachineTargetArray;
    }
}
