using System;
using System.Collections.Generic;

namespace Y7Engine
{
    public static class Mathf
    {
        public const float PI = 3.1415927f;
        public const float PI2 = PI * 2;
        public const float PI_2 = PI / 2;
        public const float Epsilon = 0.00001F;
        public const double Epsilon2 = 0.0000001F;
        /// <summary>
        ///   <para>Returns the sine of angle f.</para>
        /// </summary>
        /// <param name="f">The input angle, in radians.</param>
        /// <returns>
        ///   <para>The return value between -1 and +1.</para>
        /// </returns>
        public static float Sin(float f)
        {
            return (float) Math.Sin((double) f);
        }

        /// <summary>
        ///   <para>Returns the cosine of angle f.</para>
        /// </summary>
        /// <param name="f">The input angle, in radians.</param>
        /// <returns>
        ///   <para>The return value between -1 and 1.</para>
        /// </returns>
        public static float Cos(float f)
        {
            return (float) Math.Cos((double) f);
        }

        /// <summary>
        ///   <para>Returns the tangent of angle f in radians.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Tan(float f)
        {
            return (float) Math.Tan((double) f);
        }

        /// <summary>
        ///   <para>Returns the arc-sine of f - the angle in radians whose sine is f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Asin(float f)
        {
            return (float) Math.Asin((double) f);
        }

        /// <summary>
        ///   <para>Returns the arc-cosine of f - the angle in radians whose cosine is f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Acos(float f)
        {
            return (float) Math.Acos((double) f);
        }

        /// <summary>
        ///   <para>Returns the arc-tangent of f - the angle in radians whose tangent is f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Atan(float f)
        {
            return (float) Math.Atan((double) f);
        }

        /// <summary>
        ///   <para>Returns the angle in radians whose Tan is y/x.</para>
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        public static float Atan2(float y, float x)
        {
            return (float) Math.Atan2((double) y, (double) x);
        }

        /// <summary>
        ///   <para>Returns square root of f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Sqrt(float f)
        {
            return (float) Math.Sqrt((double) f);
        }

        /// <summary>
        ///   <para>Returns the absolute value of f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Abs(float f)
        {
            return Math.Abs(f);
        }

        /// <summary>
        ///   <para>Returns the absolute value of value.</para>
        /// </summary>
        /// <param name="value"></param>
        public static int Abs(int value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        ///   <para>Returns the smallest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static float Min(float a, float b)
        {
            return (double) a >= (double) b? b : a;
        }

        /// <summary>
        ///   <para>Returns the smallest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static float Min(params float[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0.0f;
            float num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if ((double) values[index] < (double) num)
                    num = values[index];
            }

            return num;
        }

        /// <summary>
        ///   <para>Returns the smallest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static int Min(int a, int b)
        {
            return a >= b? b : a;
        }

        /// <summary>
        ///   <para>Returns the smallest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static int Min(params int[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0;
            int num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] < num)
                    num = values[index];
            }

            return num;
        }

        /// <summary>
        ///   <para>Returns largest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static float Max(float a, float b)
        {
            return (double) a <= (double) b? b : a;
        }

        /// <summary>
        ///   <para>Returns largest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static float Max(params float[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0.0f;
            float num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if ((double) values[index] > (double) num)
                    num = values[index];
            }

            return num;
        }

        /// <summary>
        ///   <para>Returns the largest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static int Max(int a, int b)
        {
            return a <= b? b : a;
        }

        /// <summary>
        ///   <para>Returns the largest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static int Max(params int[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0;
            int num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] > num)
                    num = values[index];
            }

            return num;
        }

        /// <summary>
        ///   <para>Returns f raised to power p.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <param name="p"></param>
        public static float Pow(float f, float p)
        {
            return (float) Math.Pow((double) f, (double) p);
        }

        /// <summary>
        ///   <para>Returns e raised to the specified power.</para>
        /// </summary>
        /// <param name="power"></param>
        public static float Exp(float power)
        {
            return (float) Math.Exp((double) power);
        }

        /// <summary>
        ///   <para>Returns the logarithm of a specified number in a specified base.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <param name="p"></param>
        public static float Log(float f, float p)
        {
            return (float) Math.Log((double) f, (double) p);
        }

        /// <summary>
        ///   <para>Returns the natural (base e) logarithm of a specified number.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Log(float f)
        {
            return (float) Math.Log((double) f);
        }

        /// <summary>
        ///   <para>Returns the base 10 logarithm of a specified number.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Log10(float f)
        {
            return (float) Math.Log10((double) f);
        }

        /// <summary>
        ///   <para>Returns the smallest integer greater to or equal to f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Ceil(float f)
        {
            return (float) Math.Ceiling((double) f);
        }

        /// <summary>
        ///   <para>Returns the largest integer smaller to or equal to f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Floor(float f)
        {
            return (float) Math.Floor(f);
        }

        /// <summary>
        ///   <para>Returns f rounded to the nearest integer.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Round(float f)
        {
            return (float) Math.Round(f);
        }

        /// <summary>
        ///   <para>Returns the smallest integer greater to or equal to f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static int CeilToInt(float f)
        {
            return (int) Math.Ceiling(f);
        }

        /// <summary>
        ///   <para>Returns the largest integer smaller to or equal to f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static int FloorToInt(float f)
        {
            return (int) Math.Floor(f);
        }

        /// <summary>
        ///   <para>Returns f rounded to the nearest integer.</para>
        /// </summary>
        /// <param name="f"></param>
        public static int RoundToInt(float f)
        {
            return (int) Math.Round(f);
        }

        /// <summary>
        ///   <para>Returns the sign of f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Sign(float f)
        {
            return f < 0.0? -1f : 1f;
        }

        /// <summary>
        ///   <para>Clamps a value between a minimum float and maximum float value.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static float Clamp(float value, float min, float max)
        {
            if ( value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        /// <summary>
        ///   <para>Clamps value between min and max and returns value.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        /// <summary>
        ///   <para>Clamps value between 0 and 1 and returns value.</para>
        /// </summary>
        /// <param name="value"></param>
        public static float Clamp01(float value)
        {
            if (value < 0.0)
                return 0.0f;
            if (value > 1.0)
                return 1f;
            return value;
        }

        /// <summary>
        ///   <para>Linearly interpolates between a and b by t.</para>
        /// </summary>
        /// <param name="a">The start value.</param>
        /// <param name="b">The end value.</param>
        /// <param name="t">The interpolation value between the two floats.</param>
        /// <returns>
        ///   <para>The interpolated float result between the two float values.</para>
        /// </returns>
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }

        /// <summary>
        ///   <para>Linearly interpolates between a and b by t with no limit to t.</para>
        /// </summary>
        /// <param name="a">The start value.</param>
        /// <param name="b">The end value.</param>
        /// <param name="t">The interpolation between the two floats.</param>
        /// <returns>
        ///   <para>The float value as a result from the linear interpolation.</para>
        /// </returns>
        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        /// <summary>
        ///   <para>Same as Lerp but makes sure the values interpolate correctly when they wrap around 360 degrees.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static float LerpAngle(float a, float b, float t)
        {
            float num = Mathf.Repeat(b - a, 360f);
            if ((double) num > 180.0)
                num -= 360f;
            return a + num * Mathf.Clamp01(t);
        }

        /// <summary>
        ///   <para>Moves a value current towards target.</para>
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="target">The value to move towards.</param>
        /// <param name="maxDelta">The maximum change that should be applied to the value.</param>
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (Abs(target - current) <= maxDelta)
                return target;
            return current + Mathf.Sign(target - current) * maxDelta;
        }

        /// <summary>
        ///   <para>Same as MoveTowards but makes sure the values interpolate correctly when they wrap around 360 degrees.</para>
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxDelta"></param>
        public static float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            float num = Mathf.DeltaAngle(current, target);
            if (-maxDelta < num && num < maxDelta)
                return target;
            target = current + num;
            return Mathf.MoveTowards(current, target, maxDelta);
        }

        /// <summary>
        ///   <para>Interpolates between min and max with smoothing at the limits.</para>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        public static float SmoothStep(float from, float to, float t)
        {
            t = Mathf.Clamp01(t);
            t = (float) (-2.0 * t * t * t + 3.0 * t * t);
            return (float) ((double) to * t + from * (1.0 - t));
        }

        public static float Gamma(float value, float absmax, float gamma)
        {
            bool flag = false;
            if (value < 0.0)
                flag = true;
            float num1 = Mathf.Abs(value);
            if (num1 > absmax)
                return !flag? num1 : -num1;
            float num2 = Mathf.Pow(num1 / absmax, gamma) * absmax;
            return !flag? num2 : -num2;
        }

        /// <summary>
        ///   <para>Loops the value t, so that it is never larger than length and never smaller than 0.</para>
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        public static float Repeat(float t, float length)
        {
            return Mathf.Clamp(t - Mathf.Floor(t / length) * length, 0.0f, length);
        }

        /// <summary>
        ///   <para>PingPongs the value t, so that it is never larger than length and never smaller than 0.</para>
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        public static float PingPong(float t, float length)
        {
            t = Mathf.Repeat(t, length * 2f);
            return length - Mathf.Abs(t - length);
        }

        /// <summary>
        ///   <para>Calculates the linear parameter t that produces the interpolant value within the range [a, b].</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="value"></param>
        public static float InverseLerp(float a, float b, float value)
        {
            if (a != b)
                return Mathf.Clamp01((float) (((double) value - a) / ((double) b - a)));
            return 0.0f;
        }

        /// <summary>
        ///   <para>Calculates the shortest difference between two given angles given in degrees.</para>
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        public static float DeltaAngle(float current, float target)
        {
            float num = Mathf.Repeat(target - current, 360f);
            if (num > 180.0)
                num -= 360f;
            return num;
        }

        internal static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
        {
            float num1 = p2.X - p1.X;
            float num2 = p2.Y - p1.Y;
            float num3 = p4.X - p3.X;
            float num4 = p4.Y - p3.Y;
            float num5 = (float) ((double) num1 * (double) num4 - (double) num2 * (double) num3);
            if ((double) num5 == 0.0)
                return false;
            float num6 = p3.X - p1.X;
            float num7 = p3.Y - p1.Y;
            float num8 = (float) ((double) num6 * (double) num4 - (double) num7 * (double) num3) / num5;
            result = new Vector2(p1.X + num8 * num1, p1.Y + num8 * num2);
            return true;
        }

        internal static bool LineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
        {
            float num1 = p2.X - p1.X;
            float num2 = p2.Y - p1.Y;
            float num3 = p4.X - p3.X;
            float num4 = p4.Y - p3.Y;
            float num5 = (float) ((double) num1 * (double) num4 - (double) num2 * (double) num3);
            if ((double) num5 == 0.0)
                return false;
            float num6 = p3.X - p1.X;
            float num7 = p3.Y - p1.Y;
            float num8 = (float) ((double) num6 * (double) num4 - (double) num7 * (double) num3) / num5;
            if ((double) num8 < 0.0 || (double) num8 > 1.0)
                return false;
            float num9 = (float) ((double) num6 * (double) num2 - (double) num7 * (double) num1) / num5;
            if ((double) num9 < 0.0 || (double) num9 > 1.0)
                return false;
            result = new Vector2(p1.X + num8 * num1, p1.Y + num8 * num2);
            return true;
        }

        internal static long RandomToLong(System.Random r)
        {
            byte[] buffer = new byte[8];
            r.NextBytes(buffer);
            return (long) BitConverter.ToUInt64(buffer, 0) & long.MaxValue;
        }

        public const float Rad2Deg = (float)(180.0 / System.Math.PI);

        public static float Deg2Rad = (float)(System.Math.PI / 180.0);

        //public static Vector3 Rad2Deg(Vector3 radians)
        //{
        //    return new Vector3(
        //                       (float)(radians.x * 180 / System.Math.PI),
        //                       (float)(radians.y * 180 / System.Math.PI),
        //                       (float)(radians.z * 180 / System.Math.PI));
        //}
        //public static Vector3 Deg2Rad(Vector3 degrees)
        //{
        //    return new Vector3(
        //                       (float)(degrees.x * System.Math.PI / 180),
        //                       (float)(degrees.y * System.Math.PI / 180),
        //                       (float)(degrees.z * System.Math.PI / 180));
        //}
        
        public const float CosAngle20 = 0.9396926208f;
        public const float CompareEpsilon = 0.000001f;
        
        public static bool CompareApproximate(float f0, float f1, float epsilon = float.Epsilon)
        {
            return System.Math.Abs(f0 - f1) <= epsilon;
        }
        public static bool CompareApproximate(double f0, double f1, float epsilon = float.Epsilon)
        {
            return System.Math.Abs(f0 - f1) <= epsilon;
        }
        /// <summary>
        /// 点到线的距离
        /// </summary>
        /// <param name="point1">直线上点1</param>
        /// <param name="point2">直线上点2</param>
        /// <param name="point">目标点</param>
        /// <returns>目标点到直线的距离</returns>
        public static float DisPointToLine(Vector3 point, Vector2 point1, Vector2 point2)//point1和point2为线的两个端点
        {
            float a, b, c;
            a = Vector2.Distance(point1, point2);// 线段的长度
            b = Vector2.Distance(point1, point);// position到点point1的距离
            c = Vector2.Distance(point2, point);// position到point2点的距离
            if (c <= 0.000001 || b <= 0.000001)
                return 0;
            if (a <= 0.000001)
                return b;
            if (c * c >= a * a + b * b)
                return b;
            if (b * b >= a * a + c * c)
                return c;
            float p = (a + b + c) / 2;// 半周长
            float s = Sqrt(p * (p - a) * (p - b) * (p - c));// 海伦公式求面积
            return 2 * s / a;
        }

        /// <summary>
        /// 点到直线距离的平方
        /// </summary>
        /// <param name="point">目标点</param>
        /// <param name="linePoint1">直线上一个点</param>
        /// <param name="linePoint2">直线上另一个点</param>
        /// <returns></returns>
        public static float DisPointToLineSquared(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            Vector3 vec1 = point - linePoint1;
            Vector3 vec2 = linePoint2 - linePoint1;
            Vector3 vecProj = Vector3.Project(vec1, vec2);
            return Pow(vec1.magnitude, 2) - Pow(vecProj.magnitude, 2);
        }

        /// <summary>
        /// 点到平面的距离 自行推演函数
        /// </summary>
        /// <param name="point"></param>
        /// <param name="surfacePoint1"></param>
        /// <param name="surfacePoint2"></param>
        /// <param name="surfacePoint3"></param>
        /// <returns></returns>
        public static float DisPoint2Surface(Vector3 point, Vector3 surfacePoint1, Vector3 surfacePoint2, Vector3 surfacePoint3)
        {
            //空间直线一般式方程 Ax + By + Cz + D = 0;
            //假定 A = 1 ，推演B C D用A来表示，约去A，可得方程
            float BNumerator = (surfacePoint1.X - surfacePoint2.X) * (surfacePoint2.Z - surfacePoint3.Z) - (surfacePoint2.X - surfacePoint3.X) * (surfacePoint1.Z - surfacePoint2.Z);
            float BDenominator = (surfacePoint2.Y - surfacePoint3.Y) * (surfacePoint1.Z - surfacePoint2.Z) - (surfacePoint1.Y - surfacePoint2.Y) * (surfacePoint2.Z - surfacePoint3.Z);
            float B = BNumerator / BDenominator;
            float C = (B * (surfacePoint1.Y - surfacePoint2.Y) + (surfacePoint1.X - surfacePoint2.X)) / (surfacePoint2.Z - surfacePoint1.Z);
            float D = -surfacePoint1.X - B * surfacePoint1.Y - C * surfacePoint1.Z;

            return DisPoint2Surface(point, 1f, B, C, D);
        }

        public static float DisPoint2Surface(Vector3 point, float FactorA, float FactorB, float FactorC, float FactorD)
        {
            //点到平面的距离公式 d = |Ax + By + Cz + D|/sqrt(A2 + B2 + C2);
            float numerator = Mathf.Abs(FactorA * point.X + FactorB * point.Y + FactorC * point.Z + FactorD);
            float denominator = Mathf.Sqrt(Mathf.Pow(FactorA, 2) + Mathf.Pow(FactorB, 2) + Mathf.Pow(FactorC, 2));
            float dis = numerator / denominator;
            return dis;
        }

        //求点到直线的垂足
        public static Vector3 GetFootOfPerpendicular(Vector3 pt, Vector3 begin, Vector3 end)
        {
            float dx = begin.X - end.X;
            float dy = begin.Y - end.Y;
            float dz = begin.Z - end.Z;
            if (Abs(dx) < Epsilon && Abs(dy) < Epsilon && Abs(dz) < Epsilon)
                return begin;

            float u = (pt.X - begin.X) * (begin.X - end.X) + (pt.Y - begin.Y) * (begin.Y - end.Y) + (pt.Z - begin.Z) * (begin.Z - end.Z);
            u = u / ((dx * dx) + (dy * dy) + (dz * dz));
            Vector3 retVal;
            retVal.X = begin.X + u * dx;
            retVal.Y = begin.Y + u * dy;
            retVal.Z = begin.Z + u * dz;
            return retVal;
        }

        public static float CalcAttackAngle(Vector3 attackerPos, Vector3 attackerFaceDir, Vector3 defenderPos)
        {
            float dx = defenderPos.X - attackerPos.X;
            float dz = defenderPos.Z - attackerPos.Z;
            float attack_dir_rad = Mathf.Atan2(dz, dx);//水平攻击方向
            float face_dir_rad = Mathf.Atan2(attackerFaceDir.Z, attackerFaceDir.X);
            return attack_dir_rad - face_dir_rad;
        }

        public static void TurnYTo(ref Vector3 direction, float angleY)
        {
            if(angleY > 89.99f && angleY < 90.01f)
			{
                direction = direction.normalized;
                direction.Y = 1000f;
                return;
			}
            float verAngle = Mathf.Deg2Rad * angleY;
            float xz = (float)Math.Sqrt(direction.X * direction.X + direction.Z * direction.Z);
            direction.Y = xz * (float)Math.Tan(verAngle);
        }

        public unsafe static int RawFloatToInt(float value)
		{
            return *(int*)(&value);
		}

        public unsafe static float RawIntToFloat(int value)
		{
            return *(float*)(&value);
		}
    }
}