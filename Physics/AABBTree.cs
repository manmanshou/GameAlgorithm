using System.Collections.Generic;


namespace Y7Engine
{
    public class AABBTreeNode
    {
        public int LeftNodeIndex;
        public int RightNodeIndex;
        public AABB? BoundingBox;
    }

    public class AABBTree
    {
        private List<AABBTreeNode> nodes;

        public AABBTree()
        {
            nodes = new List<AABBTreeNode>();
        }

        public void AddNode(AABBTreeNode node)
        {
            nodes.Add(node);
        }

        public void Build()
        {
            
        }
    }

}


