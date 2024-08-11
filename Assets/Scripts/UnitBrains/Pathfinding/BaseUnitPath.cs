using System.Collections.Generic;
using Model;
using UnityEngine;

namespace UnitBrains.Pathfinding
{
    public abstract class BaseUnitPath
    {
        public Vector2Int StartPoint => startPoint;
        public Vector2Int EndPoint => endPoint;
        
        protected readonly IReadOnlyRuntimeModel runtimeModel;// Проверяет, является ли клетка проходимой
        protected readonly Vector2Int startPoint;// Стартовая точка
        protected readonly Vector2Int endPoint;// Конечная точка
        protected Vector2Int[] path = null;// путь в виде массива клеток

        protected abstract void Calculate();
        
        public IEnumerable<Vector2Int> GetPath()
        {
            if (path == null)
                Calculate();
            
            return path;
        }

        public Vector2Int GetNextStepFrom(Vector2Int unitPos)
        {
            var found = false;
            foreach (var cell in GetPath())
            {
                if (found)
                    return cell;

                found = cell == unitPos;
            }

            Debug.LogError($"Unit {unitPos} is not on the path");
            return unitPos;
        }

        protected BaseUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint)
        {
            this.runtimeModel = runtimeModel;
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }
    }
}