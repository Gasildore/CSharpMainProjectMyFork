using System;
using UnityEngine;

namespace Assets.Scripts.UnitBrains.Pathfinding
{
    public class AStarNode
    {
        public Vector2Int Pos;// Позиция по X/Y
        public int Cost = 10;// Стоимость перехода
        public int Estimate;// Оценка расстояния до цели
        public int Value;// Итоговое значение эвристической функции (конечной стоимости перехода)
        public AStarNode Parent;// Ссылка на ноду, "стрелочка"

        public AStarNode(Vector2Int position)
        {
            Pos = position;
        }

        public void CalculateEstimate(Vector2Int targetPos)// Расчёт расстояния до цели
        {
            Estimate = Math.Abs(Pos.x - targetPos.x) + Math.Abs(Pos.x - targetPos.y);
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

            return Pos.x == node.Pos.x && Pos.y == node.Pos.y;// Иначе - возвращает результат проверки координат
        }
    }
}
