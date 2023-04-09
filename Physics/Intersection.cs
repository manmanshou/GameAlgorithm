

namespace Y7Engine
{
    public static class Intersection
    {
        public static bool Intersect_AABB_OBB(AABB aabb, OBB obb)
        {
            // 将 OBB 转换为局部坐标系
            Vector3 obb_center_local = obb.center - aabb.center;
            Matrix3x3 obb_rot = obb.axes * aabb.axes.Inverse();
            Vector3 obb_extent_local = obb.half_lengths * obb_rot;

            // 判断 AABB 盒是否与 OBB 相交
            Vector3 aabb_min = aabb.center - aabb.half_lengths;
            Vector3 aabb_max = aabb.center + aabb.half_lengths;
            for (int i = 0; i < 8; i++)
            {
                Vector3 p = (i & 1) ? aabb_min : aabb_max;
                p.x *= ((i & 2) ? 1 : -1);
                p.y *= ((i & 4) ? 1 : -1);
                p = obb_center_local + p.Dot(obb_rot);
                if (p.x > obb_center_local.x + obb_extent_local.x ||
                    p.x < obb_center_local.x - obb_extent_local.x ||
                    p.y > obb_center_local.y + obb_extent_local.y ||
                    p.y < obb_center_local.y - obb_extent_local.y ||
                    p.z > obb_center_local.z + obb_extent_local.z ||
                    p.z < obb_center_local.z - obb_extent_local.z)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

