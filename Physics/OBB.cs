
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
        public Vector3[] GetAxes()
        {
            Vector3[] axes = new Vector3[3];

            Matrix3x3 m = Quaternion.QuaternionToMatrix(Rotation);

            // 获取OBB的方向向量
            Vector3 dirX = m.GetColumn(0);
            Vector3 dirY = m.GetColumn(1);
            Vector3 dirZ = m.GetColumn(2);

            // 将方向向量归一化
            dirX.Normalize();
            dirY.Normalize();
            dirZ.Normalize();

            // 将归一化后的方向向量作为轴向量
            axes[0] = dirX;
            axes[1] = dirY;
            axes[2] = dirZ;

            return axes;
        }


    }
}