using System.Collections.Generic;
using Assets.Scripts.UnitBrains.Pathfinding;
using Assets.Scripts.UnitBrains.Player;
using Model;
using Model.Runtime.Projectiles;
using UnitBrains.Pathfinding;
using UnityEngine;
using System;

namespace UnitBrains.Player
{
    public class DefaultPlayerUnitBrain : BaseUnitBrain
    {
        private BaseUnitPath _activePath;
        private int _coordinatorRange = 2;

        public override Vector2Int GetNextStep()
        {
            var recommendedTarget = PlayerUnitCoordinator.GetInstance().RecommendedTarget;
            var recommendedPosition = PlayerUnitCoordinator.GetInstance().RecommendedPosition;

            _activePath = new AStarUnitPath(runtimeModel, unit.Pos, recommendedPosition);

            if (IsTargetInCoordinationRange(recommendedTarget))
            {
                if (IsTargetInRange(recommendedTarget))
                    return unit.Pos;

                _activePath = new AStarUnitPath(runtimeModel, unit.Pos, recommendedTarget);
            }

            return _activePath.GetNextStepFrom(unit.Pos);
        }

        private bool IsTargetInCoordinationRange(Vector2Int targetPosition)
        {
            var detectionRange = _coordinatorRange * unit.Config.AttackRange * unit.Config.AttackRange;
            var v = targetPosition - unit.Pos;

            return v.sqrMagnitude <= detectionRange;
        }

        protected override List<Vector2Int> SelectTargets()
        {
            var result = new List<Vector2Int>();

            var targets = GetReachableTargets();
            var recommendedTarget = PlayerUnitCoordinator.GetInstance().RecommendedTarget;

            if (targets.Contains(recommendedTarget))
                result.Add(recommendedTarget);

            return result;

        }

        protected float DistanceToOwnBase(Vector2Int fromPos) =>
            Vector2Int.Distance(fromPos, runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);

        protected void SortByDistanceToOwnBase(List<Vector2Int> list)
        {
            list.Sort(CompareByDistanceToOwnBase);
        }

        private int CompareByDistanceToOwnBase(Vector2Int a, Vector2Int b)
        {
            var distanceA = DistanceToOwnBase(a);
            var distanceB = DistanceToOwnBase(b);
            return distanceA.CompareTo(distanceB);
        }
    }
}