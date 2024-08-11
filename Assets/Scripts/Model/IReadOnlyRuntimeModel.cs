using System.Collections.Generic;
using Model.Runtime;
using Model.Runtime.ReadOnly;
using UnityEngine;

namespace Model
{
    public interface IReadOnlyRuntimeModel
    {
        IReadOnlyMap RoMap { get; } // Карта игры
        RuntimeModel.GameStage Stage { get; }// Текущее состояние игры
        public int Level { get; } // Какой уровень
        public IReadOnlyDictionary<int, int> RoMoney { get; } // Деньги
        public IEnumerable<IReadOnlyUnit> RoUnits { get; } // Все юниты
        public IEnumerable<IReadOnlyProjectile> RoProjectiles { get; } // Проджектайлы
        
        public IEnumerable<IReadOnlyUnit> RoPlayerUnits { get; } // Юниты игрока
        public IEnumerable<IReadOnlyUnit> RoBotUnits { get; } // Юниты противника
        public IReadOnlyList<IReadOnlyBase> RoBases { get; } // Все базы

        public bool IsTileWalkable(Vector2Int pos); // Определяет является ли ячейка карты проходимой
    }
}