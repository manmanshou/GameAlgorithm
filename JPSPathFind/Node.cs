using System.Collections;
using System;

namespace JPSPathFind
{
    public class Node : IHeapItem<Node>
    {
        public Node parent;
        public int x;
        public int y;
        private int _heapIndex = 0;
        public int HeapIndex
        {
            get
            {
                return _heapIndex;
            }
            set
            {
                _heapIndex = value;
            }
        }

        public int gCost;
        public int hCost;
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public Node(Vector3 worldPoint, int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public void Reset(Vector3 worldPoint, int _x, int _y)
        {
            x = _x;
            y = _y;
            gCost = 0;
            hCost = 0;
            _heapIndex = 0;
            parent = null;
        }

        public int CompareTo(Node nodeToCompare)
        {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare;
        }

        public override string ToString()
        {
            return x.ToString() + " , " + y.ToString();
        }
    }
}