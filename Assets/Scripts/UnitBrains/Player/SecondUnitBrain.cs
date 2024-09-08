using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UnitBrains.Pathfinding;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;
using static UnityEngine.GraphicsBuffer;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        private List<Vector2Int> _priorityTargets = new List<Vector2Int>();

        public static int unitСounter = 0;
        private int unitNumber;
        private const int maxTargetsCount = 3;

        public SecondUnitBrain()
        {
            unitNumber = unitСounter++;
        }

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////
            if (GetTemperature() >= overheatTemperature)
                return;
            IncreaseTemperature();
            for (int i = 0; i <= GetTemperature(); i++)
                {
                    var projectile = CreateProjectile(forTarget);
                    AddProjectileToList(projectile, intoList);
                }
            ///////////////////////////////////////
        }

        //public override Vector2Int GetNextStep()
        //{
        //    if (targets.Count > 0)
        //    {
        //        if (IsTargetInRange(targets[0]))
        //        {
        //            return unit.Pos;
        //        }
        //        var path = new AStarUnitPath(runtimeModel, unit.Pos, targets[0]);
        //        return path.GetNextStepFrom(unit.Pos);
        //    }
        //    else
        //    {
        //        return unit.Pos;
        //    }
        //}


        //protected override List<Vector2Int> SelectTargets()
        //{
        //    List<Vector2Int> result = new List<Vector2Int>();

        //    targets.Clear();

        //    foreach (Vector2Int target in GetAllTargets())
        //    {
        //        targets.Add(target);
        //    }
        //    if (targets.Count == 0)
        //    {
        //        if (IsPlayerUnitBrain)
        //        {
        //            targets.Add(runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId]);
        //        }
        //        else
        //        {
        //            targets.Add(runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);
        //        }
        //    }
        //    else
        //    {
        //        targets.Sort((x, y) => DistanceToOwnBase(x).CompareTo(DistanceToOwnBase(y)));

        //        for (int i = 0; i < maxTargets && i < targets.Count; i++)
        //        {
        //            int targetIndex = (unitNumber + i) % targets.Count;

        //            if (IsTargetInRange(targets[targetIndex]))
        //            {
        //                result.Add(targets[targetIndex]);
        //            }
        //        }
        //    }
        //    return result;
        //}
        public override Vector2Int GetNextStep()
        {
            base.GetNextStep();
            Vector2Int targetPosition;
            targetPosition = _priorityTargets.Count > 0 ? _priorityTargets[0] : unit.Pos;
            return IsTargetInRange(targetPosition) ? unit.Pos : unit.Pos.CalcNextStepTowards(targetPosition);
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////


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
            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}