using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Assets.Scripts.UnitBrains.Pathfinding
{
    public class AStarNode
    {
        public Vector2Int Position;// Позиция по X/Y
        public int Cost = 10;// Стоимость перехода
        public int Estimate;// Оценка расстояния до цели
        public int Value;// Итоговое значение эвристической функции (конечной стоимости перехода)
        public AStarNode Parent;// Ссылка на ноду, "стрелочка"

        public AStarNode(Vector2Int pos)
        {
            Position = pos;
        }

        public void CalculateEstimate(Vector2Int targetPos)// Расчёт расстояния до цели
        {
            Estimate = Math.Abs(Position.x - targetPos.x) + Math.Abs(Position.x - targetPos.y);
            // Функция Math.Abs берёт только модуль числа, убирая знак -
        }

        public void CalculateValue()// Расчёт эвристической функции, исходя из стоимости и расстояния до цели)
        {
            Value = Cost + Estimate;
        }

        public override bool Equals(object? obj)// Проверка что сравнение происходит с нодой
        {
            if (obj is not AStarNode node)// Если нет- возвращает false
                return false;

            return Position.Equals(node.Position);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Position);
        }
    }
}
