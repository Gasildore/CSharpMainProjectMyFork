using Model;
using Model.Runtime.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnitBrains;
using UnitBrains.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.UnitBrains.Pathfinding
{
    public partial class AStarUnitPath : BaseUnitPath
    {
        private int[] dx = { -1, 0, 1, 0 };// Массивы обозначают смещения по координатам для смены позиции врага {налево, вверх, направо, вниз}
        private int[] dy = { 0, 1, 0, -1 };

        private bool _TargetIsFound;// Обнаружена ли цель
        private bool _EnemyIsReachable;// Достижим ли враг
        private AStarNode _nextToEnemyUnit;//??????????????????????????????????

        public AStarUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) :
            base(runtimeModel, startPoint, endPoint)
        {

        }

        //public override void Update()
        //{
        //    List<AStarNode> path = Calculate();// Путь равняется результатом работы метода

        //    if (path == null)// Если поиск пути невозможен, возвращается
        //        return;

        //    AStarNode nextPosition = path[1];//Так как позиция юнита это 0, следующую позицию выбираем 1
        //    TryChangePosition(nextPosition.X, nextPosition.Y, _map);
        //    // Пытается изменить позицию, и возвращает false если движение невозможно
        //}

        protected override void Calculate()// Метод поиска пути
        {
            AStarNode startNode = new AStarNode(startPoint);// Задаёт стартовые координаты
            AStarNode targetNode = new AStarNode(endPoint);// Задаёт координаты цели

            List<AStarNode> openList = new List<AStarNode> { startNode };
            // В список вносятся вершины в которые можно пойти, начиная со стартовой ноды

            List<AStarNode> closedList = new List<AStarNode>();
            // В список вносятся пройденные вершины, которые не участвуют в вычислениях

            while (openList.Count > 0) // Цикл выполняется пока в openList ещё есть ноды
            {
                AStarNode currentNode = openList[0];// Выбирается первая нода из списка (индексация начинается с 0)

                foreach (var node in openList)
                {
                    if (node.Value < currentNode.Value)// Перебирает ноды в списке и ищет с наименьшим значением эвристической функции
                        currentNode = node;// Делает такую ноду текущей
                }

                openList.Remove(currentNode);//Раз эта нода пройдена, то она исключается из открытого списка
                closedList.Add(currentNode);// И зачисляется в закрытый

                if (_TargetIsFound)
                {
                    // Если цель обнаружена на текущем ноде, то прокладывает путь до него
                    path = FindPath(currentNode);
                    return;
                }

                for (int i = 0; i < dx.Length; i++)
                {
                    int newX = currentNode.Pos.x + dx[i];
                    int newY = currentNode.Pos.y + dy[i];
                    // Складывают координату текущей ноды и её смещение по оси, и выдает новую координату по Х и Y соответственно
                    Vector2Int newPos = new Vector2Int(newX, newY);

                    if (newPos == targetNode.Pos)// Если новые координаты равны позиции цели- цель обнаружена
                        _TargetIsFound = true;

                    if (runtimeModel.IsTileWalkable(newPos))// Если клетка доступна для хода
                    {
                        AStarNode neighbor = new AStarNode(newPos);// Для неё создаётся нода

                        if (closedList.Contains(neighbor))// Проверяем, что этой ноды нет в закрытом списке
                            continue;

                        neighbor.Parent = currentNode;// Указываем в направлении текущую ноду
                        neighbor.CalculateEstimate(targetNode.Pos);// Рассчитываем расстояние
                        neighbor.CalculateValue();// И стоимость эвристической функции

                        openList.Add(neighbor);// Добавляем ноду в открытый список
                    }

                    if (CheckEncounter(newPos) && !_EnemyIsReachable)//Проверка столкновения с противником
                    {
                        _EnemyIsReachable = true;
                        _nextToEnemyUnit = currentNode;
                    }
                }
            }

            if (_EnemyIsReachable)
            {
                path = FindPath(_nextToEnemyUnit);// Прокладывает путь до противника
                return;
            }

            path = new Vector2Int[] { startNode.Pos };
        }
        private Vector2Int[] FindPath(AStarNode node)
        {
            List<Vector2Int> path = new();// Создаёт список пролагаемого пути

            while (node != null)// Цикл движется в обратном порядке, пока currentNode имеет значение
            {
                path.Add(node.Pos);// Помещает текущую ноду в список "путь"
                node = node.Parent;// Подставляет под текущую ноду следующую из Parent
            }
            path.Reverse();// Разворачивает список в обратном (правильном) порядке
            return path.ToArray();// Возвращает список пути
        }

        private bool CheckEncounter(Vector2Int newPos)
        {
            var botUnitPositions = runtimeModel.RoBotUnits.Select(u => u.Pos).Where(u => u == newPos);
            return botUnitPositions.Any();
        }        
    }
}
