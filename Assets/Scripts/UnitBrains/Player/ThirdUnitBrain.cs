using Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitBrains.Player;
using UnityEditor.Graphs;
using UnityEngine;

enum UnitMode
{
    IsShooting,
    IsRiding
}

public class ThirdUnitBrain : DefaultPlayerUnitBrain
{
    public override string TargetUnitName => "Ironclad Behemoth";
    private UnitMode _mode = UnitMode.IsRiding;
    private bool _changingMode;
    private float _modeChangeDelay = 1f;

    private List<Vector2Int> _priorityTargets = new List<Vector2Int>();

    public static int unitСounter = 0;
    private int unitNumber;
    private const int maxTargetsCount = 3;

    public ThirdUnitBrain()
    {
        unitNumber = unitСounter;
        unitСounter++;
    }

    public override void Update(float deltaTime, float time)
    {
        if (_changingMode)
        {
            _modeChangeDelay -= deltaTime;
            if (_modeChangeDelay <= 0f)
            {
                _changingMode = false;
                _modeChangeDelay = 1f;
            }
        }
        ChangeUnitMode();
        base.Update(deltaTime, time);
    }

    public override Vector2Int GetNextStep()
    {
        return _changingMode ? unit.Pos : base.GetNextStep();
    }

    protected override List<Vector2Int> SelectTargets()// Дублирует логику выбора цели у второго юнита
    {
        var iD = IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.BotPlayerId;
        var baseCoords = runtimeModel.RoMap.Bases[iD];

        _priorityTargets.Clear();
        List<Vector2Int> allTargets = GetAllTargets().ToList();
        List<Vector2Int> reachableTargets = GetReachableTargets();
        List<Vector2Int> closestTargets = new List<Vector2Int>();

        SortByDistanceToOwnBase(allTargets);

        var closestCount = maxTargetsCount > allTargets.Count ? allTargets.Count : maxTargetsCount;
        closestTargets.AddRange(allTargets.GetRange(0, closestCount));

        var targetIndex = unitNumber % maxTargetsCount;
        var indexIsExist = targetIndex < closestTargets.Count && targetIndex > 0;
        if (indexIsExist)
        {
            _priorityTargets.Add(closestTargets[targetIndex]);
        }
        else if (closestTargets.Count > 0)
        {
            _priorityTargets.Add(closestTargets[0]);
        }
        else
        {
            _priorityTargets.Add(baseCoords);
        }

        return reachableTargets.Contains(_priorityTargets.LastOrDefault()) ? _priorityTargets : reachableTargets;
    }
        
    private void checkCurrentUnitMode(UnitMode mode)
    {
        _changingMode = mode != _mode ? true : false;
    }
      
    private void ChangeUnitMode()
    {
        var currentPosition = base.GetNextStep();
        if (currentPosition == unit.Pos)
        {
            checkCurrentUnitMode(UnitMode.IsShooting);
            _mode = UnitMode.IsShooting;
        }
        else
        {
            checkCurrentUnitMode(UnitMode.IsRiding);
            _mode = UnitMode.IsRiding;
        }
    }
}