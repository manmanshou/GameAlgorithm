

namespace Y7Engine
{
    public static class Intersection
    {
        public static bool Intersect_Ray_AABB(Ray ray, AABB aabb, out float dist)
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
        public static bool Intersect_Ray_OBB(Ray ray, OBB obb, out float dist)
        {
            var obbWorldMatrix = Matrix4x4.CreateFromQuaternion(obb.Rotation) * Matrix4x4.CreateTranslation(obb.Center);
            var obbInverseMatrix = obbWorldMatrix.Inverse;

            // 把射线转换到OBB的本地空间
            var newRay = new Ray();
            newRay.Origin = obbInverseMatrix.Transform(ray.Origin);
            newRay.Direction = Vector3.TransformNormal(ray.Direction, obbInverseMatrix);
            var aabb = new AABB();
            aabb.Center = Vector3.Zero;
            aabb.Extents = obb.Extents;
            return Intersect_Ray_AABB(newRay, aabb, out dist);
        }
        public static bool Intersect_AABB_AABB(AABB a, AABB b)
        {
            var aMax = a.Center + a.Extents;
            var aMin = a.Center - a.Extents;
            var bMax = b.Center + b.Extents;
            var bMin = b.Center - b.Extents;
            // 判断两个AABB包围盒在X、Y、Z三个轴上是否有重叠部分
            if (aMax.X < bMin.X || aMin.X > bMax.X) return false;
            if (aMax.Y < bMin.Y || aMin.Y > bMax.Y) return false;
            if (aMax.Z < bMin.Z || aMin.Z > bMax.Z) return false;

            // 若三个轴都有重叠部分，则说明两个包围盒相交
            return true;
        }
        public static bool Intersect_AABB_OBB(AABB aabb, OBB obb)
        {
            // 将 OBB 的顶点转换到 AABB 坐标系下
            var obb2 = OBB.TransformToAABB(obb, aabb);
            var obb1 = new OBB(Vector3.Zero, obb.Extents, obb.Rotation);
            return Intersect_OBB_OBB(obb1, obb2);
        }
        public static bool Intersect_OBB_OBB(OBB obb1, OBB obb2)
        {
            // 获取两个 OBB 包围盒的轴向
            Vector3[] obb1Axes = obb1.GetAxes();
            Vector3[] obb2Axes = obb2.GetAxes();

            // 将轴向合并
            Vector3[] axes = new Vector3[15];
            for (int i = 0; i < 3; i++)
            {
                axes[i] = obb1Axes[i];
                axes[3 + i] = obb2Axes[i];
            }

            // 添加基于两个 OBB 包围盒轴向的组合轴向
            int index = 6;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    axes[index] = Vector3.Cross(obb1Axes[i], obb2Axes[j]);
                    index++;
                }
            }

            // 对每个轴向进行分离轴测试
            for (int i = 0; i < axes.Length; i++)
            {
                // 获取 OBB 包围盒在当前轴向上的投影区间
                float obb1Min = Vector3.Dot(obb1.Center, axes[i]);
                float obb1Max = obb1Min + Vector3.Dot(obb1.Extents, new Vector3(Mathf.Abs(axes[i].X), Mathf.Abs(axes[i].Y), Mathf.Abs(axes[i].Z)));
                float obb2Min = Vector3.Dot(obb2.Center, axes[i]);
                float obb2Max = obb2Min + Vector3.Dot(obb2.Extents, new Vector3(Mathf.Abs(axes[i].X), Mathf.Abs(axes[i].Y), Mathf.Abs(axes[i].Z)));

                // 如果区间没有重叠，则两个 OBB 包围盒一定不相交
                if (obb1Max < obb2Min || obb2Max < obb1Min)
                {
                    return false;
                }
            }

            // 如果没有找到分离轴，则两个 OBB 包围盒相交
            return true;
        }
        public static bool Intersect_Ray_Sphere(Ray ray, Sphere sphere)
        {
            Vector3 offset = sphere.Center - ray.Origin;
            float tca = Vector3.Dot(offset, ray.Direction);
            float d2 = Vector3.Dot(offset, offset) - tca * tca;
            if (d2 > sphere.Radius * sphere.Radius)
            {
                return false;
            }
            float thc = (float)Math.Sqrt(sphere.Radius * sphere.Radius - d2);
            float t0 = tca - thc;
            float t1 = tca + thc;
            if (t0 < 0 && t1 < 0)
            {
                return false;
            }
            return true;
        }
        public static bool Intersect_Sphere_AABB(Sphere sphere, AABB aabb)
        {
            var aabbMin = aabb.Center - aabb.Extents;
            var aabbMax = aabb.Center + aabb.Extents;
            // 计算AABB中心到球心的向量
            Vector3 closestPoint = Vector3.Clamp(sphere.Center, aabbMin, aabbMax);

            // 计算球心到最近点的距离
            float distanceSquared = Vector3.DistanceSquared(closestPoint, sphere.Center);

            // 判断是否相交
            return distanceSquared <= (sphere.Radius * sphere.Radius);
        }
        public static bool Intersect_Sphere_OBB(Sphere sphere, OBB obb)
        {
            // 把球转换为OBB的局部坐标系下
            Vector3 localSphereCenter = Quaternion.Inverse(obb.Rotation) * (sphere.Center - obb.Center);

            // 剩余就当和AABB碰撞处理
            float x = Mathf.Clamp(localSphereCenter.X, -obb.Extents.X, obb.Extents.X);
            float y = Mathf.Clamp(localSphereCenter.Y, -obb.Extents.Y, obb.Extents.Y);
            float z = Mathf.Clamp(localSphereCenter.Z, -obb.Extents.Z, obb.Extents.Z);
            Vector3 closestPoint = new Vector3(x, y, z);

            float distanceSquared = (closestPoint - localSphereCenter).SqrMagnitude;
            return distanceSquared <= sphere.Radius * sphere.Radius;
        }
    }
}

