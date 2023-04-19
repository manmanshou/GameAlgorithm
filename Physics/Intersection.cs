

namespace Y7Engine
{
    public static class Intersection
    {
        public bool Intersect_Ray_AABB(Ray ray, AABB aabb, out float dist)
        {
            Vector3 aabbMin = aabb.Center - aabb.Extents;
            Vector3 aabbMax = aabb.Center + aabb.Extents;
            dist = 0.0f;
            float tMin = float.MinValue;
            float tMax = float.MaxValue;
            for (int i = 0; i < 3; ++i)
            {
                if (Math.Abs(ray.Direction[i]) < float.Epsilon)
                {
                    // 如果射线平行这个轴，那么只要起点在包围盒范围外则不相交
                    if (ray.Origin[i] < aabbMin[i] || ray.Origin[i] > aabbMax[i])
                        return false;
                }
                else
                {
                    // 计算射线与该轴对齐的最小AABB距离以及最大AABB的距离
                    float invRayDir = 1.0f / ray.Direction[i];
                    float tNear = (aabbMin[i] - ray.Origin[i]) * invRayDir;
                    float tFar = (aabbMax[i] - ray.Origin[i]) * invRayDir;

                    // 近平面距离大于远平面距离时，交换一下
                    if (tNear > tFar)
                    {
                        float temp = tNear;
                        tNear = tFar;
                        tFar = temp;
                    }

                    tMax = Math.Min(tMax, tFar);
                    tMin = Math.Max(tMin, tNear);

                    if (tMin > tMax)
                        return false;
                }
            }

            dist = tMin;
            return true;
        }
        public bool Intersect_Ray_OBB(Ray ray, OBB obb, out float dist)
        {
            var obbWorldMatrix = Matrix4x4.CreateFromQuaternion(obb.Rotation) * Matrix4x4.CreateTranslation(obb.Center);
            var obbInverseMatrix = Matrix4x4.Invert(obbWorldMatrix);

            // 把射线转换到OBB的本地空间
            var newRay = new Ray();
            newRay.Origin = Vector3.Transform(ray.Origin, obbInverseMatrix);
            newRay.Direction = Vector3.TransformNormal(ray.Direction, obbInverseMatrix);
            var aabb = new AABB();
            aabb.Center = Vector3.zero;
            aabb.Extents = obb.Extents;
            return Intersect_Ray_AABB(newRay, aabb, dist);
        }
        public static bool Intersect_AABB_AABB(AABB a, AABB b)
        {
            var aMax = a.Center + a.Extents;
            var aMin = a.Center - a.Extents;
            var bMax = b.Center + b.Extents;
            var bMin = b.Center - b.Extents;
            // 判断两个AABB包围盒在X、Y、Z三个轴上是否有重叠部分
            if (aMax.x < bMin.x || aMin.x > bMax.x) return false;
            if (aMax.y < bMin.y || aMin.y > bMax.y) return false;
            if (aMax.z < bMin.z || aMin.z > bMax.z) return false;

            // 若三个轴都有重叠部分，则说明两个包围盒相交
            return true;
        }
        public static bool Intersect_AABB_OBB(AABB aabb, OBB obb)
        {
            OBB obb = new OBB(aabb.Center, aabb.Extents, Quaternion.identity);
            return true;
        }
        public static bool Intersect_OBB_OBB(OBB obb1, OBB obb2)
        {
            // 将obb2转换到obb1的本地空间，然后使用分离轴定理进行相交测试。
            Matrix4x4 toLocal = Matrix4x4.TRS(-obb1.Center, obb1.Rotation, Vector3.one).inverse;
            Vector3 diff = obb2.Center - obb1.Center;
            Vector3 center = toLocal.MultiplyPoint3x4(diff);
            Quaternion rotation = Quaternion.Inverse(obb1.Rotation) * obb2.Rotation;
            Vector3[] axes = GetAxes(rotation);
            return TestOverlap(axes, center, obb1.Extents, obb2.Extents);
        }
        private bool TestOverlap(Vector3[] axes, Vector3 center, Vector3 extents1, Vector3 extents2)
        {
            // 使用分离轴定理进行相交测试。
            for (int i = 0; i < axes.Length; i++)
            {
                Vector3 axis = axes[i];
                float proj1 = Vector3.Dot(center, axis);
                float proj2 = Vector3.Dot(extents1.x * Mathf.Abs(Vector3.Dot(axis, GetAxes(Rotation)[0])) +
                                              extents1.y * Mathf.Abs(Vector3.Dot(axis, GetAxes(Rotation)[1])) +
                                              extents1.z * Mathf.Abs(Vector3.Dot(axis, GetAxes(Rotation)[2])), axis);
                float proj3 = Vector3.Dot(extents2.x * Mathf.Abs(Vector3.Dot(axis, GetAxes(Rotation)[0])) +
                                              extents2.y * Mathf.Abs(Vector3.Dot(axis, GetAxes(Rotation)[1])) +
                                              extents2.z * Mathf.Abs(Vector3.Dot(axis, GetAxes(Rotation)[2])), axis);
                if (Mathf.Abs(proj1 - (proj2 + proj3) / 2f) > Mathf.Abs(proj2 + proj3) / 2f)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

