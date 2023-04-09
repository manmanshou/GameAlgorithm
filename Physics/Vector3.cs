using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Y7Engine
{
	[Serializable]
	public struct Vector3: IEquatable<Vector3>
	{
		private const float k1OverSqrt2 = 0.7071068f;
		private const float epsilon = 1E-05f;
		public static Vector3 zero { get { return default; } }
		public static readonly Vector3 _one = new Vector3(1f, 1f, 1f);
		public static readonly Vector3 _up = new Vector3(0.0f, 1f, 0.0f);
		public static readonly Vector3 _down = new Vector3(0.0f, -1f, 0.0f);
		public static readonly Vector3 _right = new Vector3(1f, 0.0f, 0.0f);
		public static readonly Vector3 _left = new Vector3(-1f, 0.0f, 0.0f);
		public static readonly Vector3 _forward = new Vector3(0.0f, 0.0f, 1f);
		public static readonly Vector3 _back = new Vector3(0.0f, 0.0f, -1f);

		public static Vector3 one { get { return _one; } }
		public static Vector3 up { get { return _up; } }
		public static Vector3 down { get { return _down; } }
		public static Vector3 right { get { return _right; } }
		public static Vector3 left { get { return _left; } }
		public static Vector3 forward { get { return _forward; } }
		public static Vector3 back { get { return _back; } }

		public float x;
		public float y;
		public float z;

		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3(double x, double y, double z)
		{
			this.x = (float)x;
			this.y = (float)y;
			this.z = (float)z;
		}

		public Vector3(float value)
		{
			this.x = this.y = this.z = value;
		}

		public Vector3(Vector2 value, float z)
		{
			this.x = value.x;
			this.y = value.y;
			this.z = z;
		}

		public float this[int axis]
		{
			get
			{
				switch(axis)
				{
					case 0: return x;
					case 1: return y;
					case 2: return z;
					default: throw new Exception($"axis：{axis}超出范围");
				}
			}
		}

		public override string ToString()
		{
			//CultureInfo currentCulture = CultureInfo.CurrentCulture;
			//return string.Format("({0},{1},{2})",
			//					this.x.ToString("F5", currentCulture),
			//					this.y.ToString("F5", currentCulture),
			//					this.z.ToString("F5", currentCulture));
			return $"({x},{y},{z})";
		}

		public bool Equals(Vector3 other)
		{
			if (this.x == (double) other.x && this.y == (double) other.y)
				return this.z == (double) other.z;
			return false;
		}

		public override bool Equals(object obj)
		{
			bool flag = false;
			if (obj is Vector3)
				flag = this.Equals((Vector3) obj);
			return flag;
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() + this.y.GetHashCode() + this.z.GetHashCode();
		}

		public float Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (float)Math.Sqrt(this.x * (double)this.x + this.y * (double)this.y + this.z * (double)this.z);
			}
			set
			{
				if(value == 0)
				{
					this.x = this.y = this.z = 0;
				}                    
				else
				{
					if(this.x == 0 && this.y == 0 && this.z == 0)
						throw new Exception("设置长度时，改向量没有方向");
					float r = value / this.Length;
					this.x *= r;
					this.y *= r;
					this.z *= r;
				}
			}
		}

		public float magnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Length;
			}
		}
		
		public float sqrMagnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
			}
		}

		public static float Distance(Vector3 value1, Vector3 value2)
		{
			double num1 = value1.x - value2.x;
			double num2 = value1.y - value2.y;
			double num3 = value1.z - value2.z;
			return Mathf.Sqrt((float)(num1 * num1 + num2 * num2 + num3 * num3));
		}

		public static float DistanceSquared(Vector3 value1, Vector3 value2)
		{
			float num1 = value1.x - value2.x;
			float num2 = value1.y - value2.y;
			float num3 = value1.z - value2.z;
			return (float) (num1 * (double) num1 + num2 * (double) num2 + num3 * (double) num3);
		}

		public static float DistanceSquared(ref Vector3 value1, ref Vector3 value2)
		{
			float num1 = value1.x - value2.x;
			float num2 = value1.y - value2.y;
			float num3 = value1.z - value2.z;
			return (float)(num1 * (double)num1 + num2 * (double)num2 + num3 * (double)num3);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(Vector3 vector1, Vector3 vector2)
		{
			return (float) (vector1.x * (double) vector2.x + vector1.y * (double) vector2.y +
				vector1.z * (double) vector2.z);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(in Vector3 vector1, in Vector3 vector2)
		{
			return (float)(vector1.x * (double)vector2.x + vector1.y * (double)vector2.y +
				vector1.z * (double)vector2.z);
		}
		public static float DotUp(in Vector3 vector)
		{
			return vector.y;
		}

		public void Normalize()
		{
			float num1 = (float) (this.x * (double) this.x + this.y * (double) this.y + this.z * (double) this.z);
			if (num1 < (double) Mathf.Epsilon)
				return;
			float num2 = 1f / (float) Math.Sqrt(num1);
			this.x *= num2;
			this.y *= num2;
			this.z *= num2;
		}
		
		public Vector3 normalized
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Normalize(this);
			}
		}

		public static Vector3 Normalize(Vector3 value)
		{
			double num1 = (value.x * (double) value.x + value.y * (double) value.y + value.z * (double) value.z);
			if (num1 == 0)
				return value;
			double num2 = 1f / Math.Sqrt(num1);
			Vector3 vector3;
			vector3.x = (float)(value.x * num2);
			vector3.y = (float)(value.y * num2);
			vector3.z = (float)(value.z * num2);
			return vector3;
		}

		public static void Normalize(in Vector3 value, out Vector3 result)
		{
			double num1 = (float) (value.x * (double) value.x + value.y * (double) value.y + value.z * (double) value.z);
			if (num1 == 0)
			{
				result = value;
			}
			else
			{
				double num2 = 1.0 / Math.Sqrt(num1);
				result.x = (float)(value.x * num2);
				result.y = (float)(value.y * num2);
				result.z = (float)(value.z * num2);
			}
		}

		public static Vector3 Cross(Vector3 a, Vector3 b)
		{
			return new Vector3((float)(a.y * (double)b.z - a.z * (double)b.y), (float)(a.z * (double)b.x - a.x * (double)b.z), (float)(a.x * (double)b.y - a.y * (double)b.x));
		}

		public static void Cross(in Vector3 a, in Vector3 b, out Vector3 result)
		{
			result.x = (float) (a.y * (double) b.z - a.z * (double) b.y);
			result.y = (float) (a.z * (double) b.x - a.x * (double) b.z);
			result.z = (float) (a.x * (double) b.y - a.y * (double) b.x);
		}

		public static Vector3 Reflect(Vector3 vector, Vector3 normal)
		{
			float num =
					(float) (vector.x * (double) normal.x + vector.y * (double) normal.y + vector.z * (double) normal.z);
			Vector3 vector3;
			vector3.x = vector.x - 2f * num * normal.x;
			vector3.y = vector.y - 2f * num * normal.y;
			vector3.z = vector.z - 2f * num * normal.z;
			return vector3;
		}

		public static void Reflect(in Vector3 vector, in Vector3 normal, out Vector3 result)
		{
			float num =
					(float) (vector.x * (double) normal.x + vector.y * (double) normal.y + vector.z * (double) normal.z);
			result.x = vector.x - 2f * num * normal.x;
			result.y = vector.y - 2f * num * normal.y;
			result.z = vector.z - 2f * num * normal.z;
		}

		public static Vector3 Min(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.x = value1.x < value2.x? value1.x : value2.x;
			vector3.y = value1.y < value2.y? value1.y : value2.y;
			vector3.z = value1.z < value2.z? value1.z : value2.z;
			return vector3;
		}

		public static void Min(in Vector3 value1, in Vector3 value2, out Vector3 result)
		{
			result.x = value1.x < value2.x? value1.x : value2.x;
			result.y = value1.y < value2.y? value1.y : value2.y;
			result.z = value1.z < value2.z? value1.z : value2.z;
		}

		public static Vector3 Max(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.x = value1.x > value2.x? value1.x : value2.x;
			vector3.y = value1.y > value2.y? value1.y : value2.y;
			vector3.z = value1.z > value2.z? value1.z : value2.z;
			return vector3;
		}

		public static void Max(in Vector3 value1, in Vector3 value2, out Vector3 result)
		{
			result.x = value1.x > value2.x? value1.x : value2.x;
			result.y = value1.y > value2.y? value1.y : value2.y;
			result.z = value1.z > value2.z? value1.z : value2.z;
		}

		public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max)
		{
			float x = value1.x;
			float num1 = x > max.x? max.x : x;
			float num2 = num1 < min.x? min.x : num1;
			float y = value1.y;
			float num3 = y > max.y? max.y : y;
			float num4 = num3 < min.y? min.y : num3;
			float z = value1.z;
			float num5 = z > max.z? max.z : z;
			float num6 = num5 < min.z? min.z : num5;
			Vector3 vector3;
			vector3.x = num2;
			vector3.y = num4;
			vector3.z = num6;
			return vector3;
		}

		public static void Clamp(in Vector3 value1, in Vector3 min, in Vector3 max, out Vector3 result)
		{
			float x = value1.x;
			float num1 = x > max.x? max.x : x;
			float num2 = num1 < min.x? min.x : num1;
			float y = value1.y;
			float num3 = y > max.y? max.y : y;
			float num4 = num3 < min.y? min.y : num3;
			float z = value1.z;
			float num5 = z > max.z? max.z : z;
			float num6 = num5 < min.z? min.z : num5;
			result.x = num2;
			result.y = num4;
			result.z = num6;
		}

		public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
		{
			Vector3 vector3;
			vector3.x = value1.x + (value2.x - value1.x) * amount;
			vector3.y = value1.y + (value2.y - value1.y) * amount;
			vector3.z = value1.z + (value2.z - value1.z) * amount;
			return vector3;
		}

		public static void Lerp(in Vector3 value1, in Vector3 value2, float amount, out Vector3 result)
		{
			result.x = value1.x + (value2.x - value1.x) * amount;
			result.y = value1.y + (value2.y - value1.y) * amount;
			result.z = value1.z + (value2.z - value1.z) * amount;
		}

		public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, float amount)
		{
			amount = (double) amount > 1.0? 1f : ((double) amount < 0.0? 0.0f : amount);
			amount = (float) (amount * (double) amount * (3.0 - 2.0 * amount));
			Vector3 vector3;
			vector3.x = value1.x + (value2.x - value1.x) * amount;
			vector3.y = value1.y + (value2.y - value1.y) * amount;
			vector3.z = value1.z + (value2.z - value1.z) * amount;
			return vector3;
		}

		public static void SmoothStep(in Vector3 value1, in Vector3 value2, float amount, out Vector3 result)
		{
			amount = amount > 1.0? 1f : (amount < 0.0? 0.0f : amount);
			amount = (float) (amount * (double) amount * (3.0 - 2.0 * amount));
			result.x = value1.x + (value2.x - value1.x) * amount;
			result.y = value1.y + (value2.y - value1.y) * amount;
			result.z = value1.z + (value2.z - value1.z) * amount;
		}

		public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount)
		{
			float num1 = amount * amount;
			float num2 = amount * num1;
			float num3 = (float) (2.0 * num2 - 3.0 * num1 + 1.0);
			float num4 = (float) (-2.0 * num2 + 3.0 * num1);
			float num5 = num2 - 2f * num1 + amount;
			float num6 = num2 - num1;
			Vector3 vector3;
			vector3.x = (float) (value1.x * (double) num3 + value2.x * (double) num4 + tangent1.x * (double) num5 +
				tangent2.x * (double) num6);
			vector3.y = (float) (value1.y * (double) num3 + value2.y * (double) num4 + tangent1.y * (double) num5 +
				tangent2.y * (double) num6);
			vector3.z = (float) (value1.z * (double) num3 + value2.z * (double) num4 + tangent1.z * (double) num5 +
				tangent2.z * (double) num6);
			return vector3;
		}

		public static void Hermite(
			in Vector3 value1, in Vector3 tangent1, in Vector3 value2, in Vector3 tangent2, float amount, out Vector3 result)
		{
			float num1 = amount * amount;
			float num2 = amount * num1;
			float num3 = (float) (2.0 * num2 - 3.0 * num1 + 1.0);
			float num4 = (float) (-2.0 * num2 + 3.0 * num1);
			float num5 = num2 - 2f * num1 + amount;
			float num6 = num2 - num1;
			result.x = (float) (value1.x * (double) num3 + value2.x * (double) num4 + tangent1.x * (double) num5 +
				tangent2.x * (double) num6);
			result.y = (float) (value1.y * (double) num3 + value2.y * (double) num4 + tangent1.y * (double) num5 +
				tangent2.y * (double) num6);
			result.z = (float) (value1.z * (double) num3 + value2.z * (double) num4 + tangent1.z * (double) num5 +
				tangent2.z * (double) num6);
		}

		public static Vector3 Negate(Vector3 value)
		{
			Vector3 vector3;
			vector3.x = -value.x;
			vector3.y = -value.y;
			vector3.z = -value.z;
			return vector3;
		}

		public static void Negate(in Vector3 value, out Vector3 result)
		{
			result.x = -value.x;
			result.y = -value.y;
			result.z = -value.z;
		}

		public static Vector3 Add(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.x = value1.x + value2.x;
			vector3.y = value1.y + value2.y;
			vector3.z = value1.z + value2.z;
			return vector3;
		}

		public static void Add(in Vector3 value1, in Vector3 value2, out Vector3 result)
		{
			result.x = value1.x + value2.x;
			result.y = value1.y + value2.y;
			result.z = value1.z + value2.z;
		}

		public static Vector3 Sub(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.x = value1.x - value2.x;
			vector3.y = value1.y - value2.y;
			vector3.z = value1.z - value2.z;
			return vector3;
		}

		public static void Sub(in Vector3 value1, in Vector3 value2, out Vector3 result)
		{
			result.x = value1.x - value2.x;
			result.y = value1.y - value2.y;
			result.z = value1.z - value2.z;
		}

		public static Vector3 Multiply(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.x = value1.x * value2.x;
			vector3.y = value1.y * value2.y;
			vector3.z = value1.z * value2.z;
			return vector3;
		}

		public static void Multiply(in Vector3 value1, in Vector3 value2, out Vector3 result)
		{
			result.x = value1.x * value2.x;
			result.y = value1.y * value2.y;
			result.z = value1.z * value2.z;
		}

		public static Vector3 Multiply(Vector3 value1, float scaleFactor)
		{
			Vector3 vector3;
			vector3.x = value1.x * scaleFactor;
			vector3.y = value1.y * scaleFactor;
			vector3.z = value1.z * scaleFactor;
			return vector3;
		}

		public static void Multiply(in Vector3 value1, float scaleFactor, out Vector3 result)
		{
			result.x = value1.x * scaleFactor;
			result.y = value1.y * scaleFactor;
			result.z = value1.z * scaleFactor;
		}

		public static Vector3 Divide(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.x = value1.x / value2.x;
			vector3.y = value1.y / value2.y;
			vector3.z = value1.z / value2.z;
			return vector3;
		}

		public static void Divide(in Vector3 value1, in Vector3 value2, out Vector3 result)
		{
			result.x = value1.x / value2.x;
			result.y = value1.y / value2.y;
			result.z = value1.z / value2.z;
		}

		public static Vector3 Divide(Vector3 value1, float divider)
		{
			float num = 1f / divider;
			Vector3 vector3;
			vector3.x = value1.x * num;
			vector3.y = value1.y * num;
			vector3.z = value1.z * num;
			return vector3;
		}

		public static void Divide(in Vector3 value1, float divider, out Vector3 result)
		{
			float num = 1f / divider;
			result.x = value1.x * num;
			result.y = value1.y * num;
			result.z = value1.z * num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float magnitudeStatic(in Vector3 inV)
		{
			return (float) Math.Sqrt((float)(inV.x * (double)inV.x + inV.y * (double)inV.y + inV.z * (double)inV.z));
		}

		private static Vector3 orthoNormalVectorFast(in Vector3 n)
		{
			Vector3 vector3;
			if (Math.Abs(n.z) > (double) k1OverSqrt2)
			{
				float num = 1f / (float) Math.Sqrt(n.y * (double) n.y + n.z * (double) n.z);
				vector3.x = 0.0f;
				vector3.y = -n.z * num;
				vector3.z = n.y * num;
			}
			else
			{
				float num = 1f / (float) Math.Sqrt(n.x * (double) n.x + n.y * (double) n.y);
				vector3.x = -n.y * num;
				vector3.y = n.x * num;
				vector3.z = 0.0f;
			}
			return vector3;
		}

		public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent)
		{
			float num1 = magnitudeStatic(in normal);
			if (num1 > Mathf.Epsilon)
				normal /= num1;
			else
				normal = new Vector3(1f, 0.0f, 0.0f);
			float num2 = Dot(normal, tangent);
			tangent -= normal * num2;
			float num3 = magnitudeStatic(in tangent);
			if (num3 < Mathf.Epsilon)
				tangent = orthoNormalVectorFast(in normal);
			else
				tangent /= num3;
		}

		public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent, ref Vector3 binormal)
		{
			float num1 = magnitudeStatic(in normal);
			if (num1 > Mathf.Epsilon)
				normal /= num1;
			else
				normal = new Vector3(1f, 0.0f, 0.0f);
			float num2 = Dot(normal, tangent);
			tangent -= normal * num2;
			float num3 = magnitudeStatic(in tangent);
			if (num3 > Mathf.Epsilon)
				tangent /= num3;
			else
				tangent = orthoNormalVectorFast(in normal);
			float num4 = Dot(tangent, binormal);
			float num5 = Dot(normal, binormal);
			binormal -= normal * num5 + tangent * num4;
			float num6 = magnitudeStatic(in binormal);
			if (num6 > Mathf.Epsilon)
				binormal /= num6;
			else
				binormal = Cross(normal, tangent);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Project(Vector3 vector, Vector3 normal)
		{
			return normal * Dot(vector, normal) / Dot(normal, normal);
		}

		public static bool Project(in Vector3 vector, in Vector3 normal, out Vector3 result)
		{
			float dot = Dot(vector, normal);
			if (dot > -epsilon && dot < epsilon)
			{
				result = zero;
				return false;
			}
			result = normal * dot / Dot(normal, normal);
			return true;
		}

		/// <summary>
		/// 当参数normal模为1时，才能用这个函数，否则请用Project函数
		/// </summary>
		public static bool project(in Vector3 vector, in Vector3 normal, out Vector3 result)
		{
			float dot = Dot(vector, normal);
			if (dot > -epsilon && dot < epsilon)
			{
				result = zero;
				return false;
			}
			result = normal * dot;
			return true;
		}

		public static float Angle(Vector3 from, Vector3 to)
		{
			double num = Math.Sqrt((double)from.sqrMagnitude * to.sqrMagnitude);
			if (num < 1.00000000362749E-15)
				return 0.0f;
			//return (float)Math.Acos(MathF.Clamp(Dot(from, to) / num, -1f, 1f));
			return (float)Math.Acos(Dot(from, to) / num);
		}

		public static void Angle(in Vector3 from, in Vector3 to, out float result)
		{
			result = 0.0f;
			double num = Math.Sqrt((double) from.sqrMagnitude * to.sqrMagnitude);
			if ( num < 1.00000000362749E-15)
				return;
			Vector3.Dot(in from, in to);
			result = Mathf.Cos(Mathf.Clamp(result, -1f, 1f)) * 57.29578f;
			return;
		}
		
		#region 扩展方法添加区域  modify by James.liu
		/// <summary>
		/// Determine the signed angle between two vectors, with normal 'n'
		/// as the rotation axis.
		/// </summary>
		public static float Angle(Vector3 v1, Vector3 v2, Vector3 n)
		{
			return Mathf.Rad2Deg * Mathf.Atan2(Dot(n, Cross(v1, v2)), Dot(v1, v2));
		}

		#endregion

		public static Vector3 operator -(Vector3 value)
		{
			Vector3 vector3;
			vector3.x = -value.x;
			vector3.y = -value.y;
			vector3.z = -value.z;
			return vector3;
		}

		public static bool operator ==(Vector3 lhs, Vector3 rhs)
		{
			return (lhs - rhs).sqrMagnitude < 9.99999943962493E-11;
		}

		public static bool operator !=(Vector3 lhs, Vector3 rhs)
		{
			return !(lhs == rhs);
		}

		public static Vector3 operator +(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.x = value1.x + value2.x;
			vector3.y = value1.y + value2.y;
			vector3.z = value1.z + value2.z;
			return vector3;
		}

		public static Vector3 operator +(Vector3 value1, float offset)
		{
			value1.x = value1.x + offset;
			value1.y = value1.y + offset;
			value1.z = value1.z + offset;
			return value1;
		}

		public static Vector3 operator -(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.x = value1.x - value2.x;
			vector3.y = value1.y - value2.y;
			vector3.z = value1.z - value2.z;
			return vector3;
		}

		public static Vector3 operator -(Vector3 value1, float offset)
		{
			value1.x = value1.x - offset;
			value1.y = value1.y - offset;
			value1.z = value1.z - offset;
			return value1;
		}

		public static Vector3 operator *(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.x = value1.x * value2.x;
			vector3.y = value1.y * value2.y;
			vector3.z = value1.z * value2.z;
			return vector3;
		}

		public static Vector3 operator *(Vector3 value, float scaleFactor)
		{
			Vector3 vector3;
			vector3.x = value.x * scaleFactor;
			vector3.y = value.y * scaleFactor;
			vector3.z = value.z * scaleFactor;
			return vector3;
		}

		public static Vector3 operator *(float scaleFactor, Vector3 value)
		{
			Vector3 vector3;
			vector3.x = value.x * scaleFactor;
			vector3.y = value.y * scaleFactor;
			vector3.z = value.z * scaleFactor;
			return vector3;
		}

		public static Vector3 operator /(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.x = value1.x / value2.x;
			vector3.y = value1.y / value2.y;
			vector3.z = value1.z / value2.z;
			return vector3;
		}

		public static Vector3 operator /(Vector3 value, float divider)
		{
			float num = 1f / divider;
			Vector3 vector3;
			vector3.x = value.x * num;
			vector3.y = value.y * num;
			vector3.z = value.z * num;
			return vector3;
		}
	}
}