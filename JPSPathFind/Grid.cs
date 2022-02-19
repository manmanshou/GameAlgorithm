using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPSPathFind
{
    public class Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"X:{x + 1},Y:{y + 1}";
        }
    }


    public class Grid
    {
        #region Singleton
        private static Grid _instance = null;
        public static Grid Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Grid();
                return _instance;
            }
        }
        #endregion

        private int _nodeAmountX;
        private int _nodeAmountY;

        public int NodeAmountX
        {
            get
            {
                return _nodeAmountX;
            }
        }
        public int NodeAmountY
        {
            get
            {
                return _nodeAmountY;
            }
        }

        private FindPath _path = null;

        private Node[,] _grid = null;

        private Node[,] _unwalkableNodes; // 不可移动信息

        public Node[,] UnwalkableNodes
        {
            get
            {
                return _unwalkableNodes;
            }
        }

        public int GridSize
        {
            get
            {
                return _nodeAmountX * _nodeAmountY;
            }
        }

        public void InitializeGrid(int xLen, int yLen, bool[,] pathInfo)
        {
            if (_nodeAmountX != xLen || _nodeAmountY != yLen)
            {
                _unwalkableNodes = new Node[xLen, yLen];
                _grid = new Node[xLen, yLen];
            }

            _nodeAmountX = xLen;
            _nodeAmountY = yLen;

            if (_path == null)
                _path = new FindPath();

            for (var x = 0; x < xLen; x++)
            {
                for (var y = 0; y < yLen; y++)
                {
                    _unwalkableNodes[x, y] = null;

                    if (_grid[x, y] == null)
                        _grid[x, y] = new Node(Vector3.zero, x, y);
                    if (_grid[x, y].x < xLen && _grid[x, y].y < yLen)
                    {
                        _grid[x, y].Reset(Vector3.zero, x, y, 1);
                        var pathId = y * 1000 + x;
                        if (!pathInfo[x, y])
                        {
                            _unwalkableNodes[x, y] = _grid[x, y];
                        }
                    }
                }
            }
        }

        public List<Node> GetAStarNeighbours(Node currentNode)
        {
            List<Node> neighbours = new List<Node>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    if (IsWalkable(x + currentNode.x, y + currentNode.y))
                    {
                        neighbours.Add(_grid[x + currentNode.x, y + currentNode.y]);
                    }
                }
            }
            return neighbours;
        }

        public List<Node> GetPath(Point startPosition, Point targetPosition)
        {
            Node startNode = GetNodeFromPoint(startPosition.x, startPosition.y);
            Node targetNode = GetNodeFromPoint(targetPosition.x, targetPosition.y);
            var path = _path.GetPath(startNode, targetNode);
            return path;
        }

        private void Print(Point startPosition, Point targetPosition)
        {
            string filePath = Application.dataPath + "/../Temp/CShapeBuildPosInfo.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            var strBuilder = new StringBuilder();
            for (int y = _nodeAmountY - 1; y >= 0; y--)
            {
                for (int x = 0; x <= _nodeAmountX - 1; x++)
                {
                    if (x == startPosition.x && y == startPosition.y)
                    {
                        strBuilder.Append("S");
                    }
                    else if (x == targetPosition.x && y == targetPosition.y)
                    {
                        strBuilder.Append("E");
                    }
                    else
                    {
                        var model = IsWalkable(x, y) ? 1 : 0;
                        strBuilder.Append(model);
                    }
                }

                strBuilder.Append("\n");
            }

            File.WriteAllText(filePath, strBuilder.ToString());
        }

        public Node GetNodeFromPoint(int nodeX, int nodeY, bool start = false)
        {
            float percentX = nodeX / (float)_nodeAmountX;
            float percentY = nodeY / (float)_nodeAmountY;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = nodeX;
            int y = nodeY;

            x = Mathf.Clamp(x, 0, _nodeAmountX - 1);
            y = Mathf.Clamp(y, 0, _nodeAmountY - 1);

            return _grid[x, y];
        }

        public Node GetNodeFromIndex(int x, int y)
        {
            if (!IsWalkable(x, y))
                return null;
            return _grid[x, y];
        }

        public List<Node> GetNeighbours(Node currentNode)
        {
            List<Node> neighbours = new List<Node>();

            Node parentNode = currentNode.parent;
            if (parentNode == null)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0)
                            continue;

                        if (IsWalkable(x + currentNode.x, y + currentNode.y))
                        {
                            neighbours.Add(_grid[x + currentNode.x, y + currentNode.y]);
                        }
                    }
                }
            }
            else
            {
                int xDirection = Mathf.Clamp(currentNode.x - parentNode.x, -1, 1); // 确定移动方向
                int yDirection = Mathf.Clamp(currentNode.y - parentNode.y, -1, 1);

                if (xDirection != 0 && yDirection != 0)
                {
                    //assumes positive direction for variable naming
                    bool neighbourUp = IsWalkable(currentNode.x, currentNode.y + yDirection);
                    bool neighbourRight = IsWalkable(currentNode.x + xDirection, currentNode.y);
                    bool neighbourLeft = IsWalkable(currentNode.x - xDirection, currentNode.y);
                    bool neighbourDown = IsWalkable(currentNode.x, currentNode.y - yDirection);

                    if (neighbourUp)
                        neighbours.Add(_grid[currentNode.x, currentNode.y + yDirection]);

                    if (neighbourRight)
                        neighbours.Add(_grid[currentNode.x + xDirection, currentNode.y]);

                    if (neighbourUp || neighbourRight)
                        if (IsWalkable(currentNode.x + xDirection, currentNode.y + yDirection))
                            neighbours.Add(_grid[currentNode.x + xDirection, currentNode.y + yDirection]);

                    if (!neighbourLeft && neighbourUp) // 根据强制邻居概念获取跳点
                        if (IsWalkable(currentNode.x - xDirection, currentNode.y + yDirection))
                            neighbours.Add(_grid[currentNode.x - xDirection, currentNode.y + yDirection]);

                    if (!neighbourDown && neighbourRight)
                        if (IsWalkable(currentNode.x + xDirection, currentNode.y - yDirection))
                            neighbours.Add(_grid[currentNode.x + xDirection, currentNode.y - yDirection]);
                }
                else
                {
                    if (xDirection == 0)
                    {
                        if (IsWalkable(currentNode.x, currentNode.y + yDirection))
                        {
                            neighbours.Add(_grid[currentNode.x, currentNode.y + yDirection]);

                            if (!IsWalkable(currentNode.x + 1, currentNode.y))
                                if (IsWalkable(currentNode.x + 1, currentNode.y + yDirection))
                                    neighbours.Add(_grid[currentNode.x + 1, currentNode.y + yDirection]);

                            if (!IsWalkable(currentNode.x - 1, currentNode.y))
                                if (IsWalkable(currentNode.x - 1, currentNode.y + yDirection))
                                    neighbours.Add(_grid[currentNode.x - 1, currentNode.y + yDirection]);
                        }
                    }
                    else
                    {
                        if (IsWalkable(currentNode.x + xDirection, currentNode.y))
                        {
                            neighbours.Add(_grid[currentNode.x + xDirection, currentNode.y]);
                            if (!IsWalkable(currentNode.x, currentNode.y + 1))
                                neighbours.Add(_grid[currentNode.x + xDirection, currentNode.y + 1]);
                            if (!IsWalkable(currentNode.x, currentNode.y - 1))
                                neighbours.Add(_grid[currentNode.x + xDirection, currentNode.y - 1]);
                        }
                    }
                }
            }
            return neighbours;
        }

        public bool IsWalkable(int x, int y)
        {
            return (x >= 0 && x < _nodeAmountX) && (y >= 0 && y < _nodeAmountY) && (_unwalkableNodes[x, y] == null);
        }
    }
}


