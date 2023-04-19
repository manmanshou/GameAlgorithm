
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
        public static OBB TransformToAABB(OBB obb, AABB aabb)
        {
            Vector3 center = aabb.Center;
            Vector3 extents = aabb.Extents;
            Vector3[] axes = obb.GetAxes();

            // 在 AABB 坐标系下计算 OBB 的半长轴
            var halfLengths = new float[3];
            for (int i = 0; i < 3; i++)
            {
                halfLengths[i] = Mathf.Abs(Vector3.Dot(axes[i], obb.Extents));
            }

            return new OBB(center, new Vector3(halfLengths[0], halfLengths[1], halfLengths[2]), Quaternion.identity);
        }

    }
}