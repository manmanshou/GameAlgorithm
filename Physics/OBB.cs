
namespace Y7Engine
{
    public class OBB
    {
        public Vector3 Center;
        public Quaternion Rotation;
        public Vector3 Extents;

        public OBB(Vector3 center, Vector3 extents, Quaternion rotation)
        {
            Center = center;
            Extents = extents;
            Rotation = rotation;
        }
    }
}