using System.Runtime.CompilerServices;

namespace Y7Engine
{
	[Serializable]
	public struct Vector3: IEquatable<Vector3>
	{
		private const float K1OverSqrt2 = 0.7071068f;
		private const float Epsilon = 1E-05f;
		public static Vector3 Zero { get { return default; } }
		public static readonly Vector3 _one = new Vector3(1f, 1f, 1f);
		public static readonly Vector3 _up = new Vector3(0.0f, 1f, 0.0f);
		public static readonly Vector3 _down = new Vector3(0.0f, -1f, 0.0f);
		public static readonly Vector3 _right = new Vector3(1f, 0.0f, 0.0f);
		public static readonly Vector3 _left = new Vector3(-1f, 0.0f, 0.0f);
		public static readonly Vector3 _forward = new Vector3(0.0f, 0.0f, 1f);
		public static readonly Vector3 _back = new Vector3(0.0f, 0.0f, -1f);

		public static Vector3 One { get { return _one; } }
		public static Vector3 Up { get { return _up; } }
		public static Vector3 Down { get { return _down; } }
		public static Vector3 Right { get { return _right; } }
		public static Vector3 Left { get { return _left; } }
		public static Vector3 Forward { get { return _forward; } }
		public static Vector3 Back { get { return _back; } }

		public float X;
		public float Y;
		public float Z;

		public Vector3(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public Vector3(double x, double y, double z)
		{
			this.X = (float)x;
			this.Y = (float)y;
			this.Z = (float)z;
		}

		public Vector3(float value)
		{
			this.X = this.Y = this.Z = value;
		}

		public Vector3(Vector2 value, float z)
		{
			this.X = value.X;
			this.Y = value.Y;
			this.Z = z;
		}

		public float this[int axis]
		{
			get
			{
				switch(axis)
				{
					case 0: return X;
					case 1: return Y;
					case 2: return Z;
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
			return $"({X},{Y},{Z})";
		}

		public bool Equals(Vector3 other)
		{
			if (this.X == (double) other.X && this.Y == (double) other.Y)
				return this.Z == (double) other.Z;
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
			return this.X.GetHashCode() + this.Y.GetHashCode() + this.Z.GetHashCode();
		}

		public float Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (float)Math.Sqrt(this.X * (double)this.X + this.Y * (double)this.Y + this.Z * (double)this.Z);
			}
			set
			{
				if(value == 0)
				{
					this.X = this.Y = this.Z = 0;
				}                    
				else
				{
					if(this.X == 0 && this.Y == 0 && this.Z == 0)
						throw new Exception("设置长度时，改向量没有方向");
					float r = value / this.Length;
					this.X *= r;
					this.Y *= r;
					this.Z *= r;
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
				return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
			}
		}

		public static float Distance(Vector3 value1, Vector3 value2)
		{
			double num1 = value1.X - value2.X;
			double num2 = value1.Y - value2.Y;
			double num3 = value1.Z - value2.Z;
			return Mathf.Sqrt((float)(num1 * num1 + num2 * num2 + num3 * num3));
		}

		public static float DistanceSquared(Vector3 value1, Vector3 value2)
		{
			float num1 = value1.X - value2.X;
			float num2 = value1.Y - value2.Y;
			float num3 = value1.Z - value2.Z;
			return (float) (num1 * (double) num1 + num2 * (double) num2 + num3 * (double) num3);
		}

		public static float DistanceSquared(ref Vector3 value1, ref Vector3 value2)
		{
			float num1 = value1.X - value2.X;
			float num2 = value1.Y - value2.Y;
			float num3 = value1.Z - value2.Z;
			return (float)(num1 * (double)num1 + num2 * (double)num2 + num3 * (double)num3);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(Vector3 vector1, Vector3 vector2)
		{
			return (float) (vector1.X * (double) vector2.X + vector1.Y * (double) vector2.Y +
				vector1.Z * (double) vector2.Z);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(in Vector3 vector1, in Vector3 vector2)
		{
			return (float)(vector1.X * (double)vector2.X + vector1.Y * (double)vector2.Y +
				vector1.Z * (double)vector2.Z);
		}
		public static float DotUp(in Vector3 vector)
		{
			return vector.Y;
		}

		public void Normalize()
		{
			float num1 = (float) (this.X * (double) this.X + this.Y * (double) this.Y + this.Z * (double) this.Z);
			if (num1 < (double) Mathf.Epsilon)
				return;
			float num2 = 1f / (float) Math.Sqrt(num1);
			this.X *= num2;
			this.Y *= num2;
			this.Z *= num2;
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
			double num1 = (value.X * (double) value.X + value.Y * (double) value.Y + value.Z * (double) value.Z);
			if (num1 == 0)
				return value;
			double num2 = 1f / Math.Sqrt(num1);
			Vector3 vector3;
			vector3.X = (float)(value.X * num2);
			vector3.Y = (float)(value.Y * num2);
			vector3.Z = (float)(value.Z * num2);
			return vector3;
		}

		public static void Normalize(in Vector3 value, out Vector3 result)
		{
			double num1 = (float) (value.X * (double) value.X + value.Y * (double) value.Y + value.Z * (double) value.Z);
			if (num1 == 0)
			{
				result = value;
			}
			else
			{
				double num2 = 1.0 / Math.Sqrt(num1);
				result.X = (float)(value.X * num2);
				result.Y = (float)(value.Y * num2);
				result.Z = (float)(value.Z * num2);
			}
		}

		public static Vector3 Cross(Vector3 a, Vector3 b)
		{
			return new Vector3((float)(a.Y * (double)b.Z - a.Z * (double)b.Y), (float)(a.Z * (double)b.X - a.X * (double)b.Z), (float)(a.X * (double)b.Y - a.Y * (double)b.X));
		}

		public static void Cross(in Vector3 a, in Vector3 b, out Vector3 result)
		{
			result.X = (float) (a.Y * (double) b.Z - a.Z * (double) b.Y);
			result.Y = (float) (a.Z * (double) b.X - a.X * (double) b.Z);
			result.Z = (float) (a.X * (double) b.Y - a.Y * (double) b.X);
		}

		public static Vector3 Reflect(Vector3 vector, Vector3 normal)
		{
			float num =
					(float) (vector.X * (double) normal.X + vector.Y * (double) normal.Y + vector.Z * (double) normal.Z);
			Vector3 vector3;
			vector3.X = vector.X - 2f * num * normal.X;
			vector3.Y = vector.Y - 2f * num * normal.Y;
			vector3.Z = vector.Z - 2f * num * normal.Z;
			return vector3;
		}

		public static void Reflect(in Vector3 vector, in Vector3 normal, out Vector3 result)
		{
			float num =
					(float) (vector.X * (double) normal.X + vector.Y * (double) normal.Y + vector.Z * (double) normal.Z);
			result.X = vector.X - 2f * num * normal.X;
			result.Y = vector.Y - 2f * num * normal.Y;
			result.Z = vector.Z - 2f * num * normal.Z;
		}

		public static Vector3 Min(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.X = value1.X < value2.X? value1.X : value2.X;
			vector3.Y = value1.Y < value2.Y? value1.Y : value2.Y;
			vector3.Z = value1.Z < value2.Z? value1.Z : value2.Z;
			return vector3;
		}

		public static void Min(in Vector3 value1, in Vector3 value2, out Vector3 result)
		{
			result.X = value1.X < value2.X? value1.X : value2.X;
			result.Y = value1.Y < value2.Y? value1.Y : value2.Y;
			result.Z = value1.Z < value2.Z? value1.Z : value2.Z;
		}

		public static Vector3 Max(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.X = value1.X > value2.X? value1.X : value2.X;
			vector3.Y = value1.Y > value2.Y? value1.Y : value2.Y;
			vector3.Z = value1.Z > value2.Z? value1.Z : value2.Z;
			return vector3;
		}

		public static void Max(in Vector3 value1, in Vector3 value2, out Vector3 result)
		{
			result.X = value1.X > value2.X? value1.X : value2.X;
			result.Y = value1.Y > value2.Y? value1.Y : value2.Y;
			result.Z = value1.Z > value2.Z? value1.Z : value2.Z;
		}

		public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max)
		{
			float x = value1.X;
			float num1 = x > max.X? max.X : x;
			float num2 = num1 < min.X? min.X : num1;
			float y = value1.Y;
			float num3 = y > max.Y? max.Y : y;
			float num4 = num3 < min.Y? min.Y : num3;
			float z = value1.Z;
			float num5 = z > max.Z? max.Z : z;
			float num6 = num5 < min.Z? min.Z : num5;
			Vector3 vector3;
			vector3.X = num2;
			vector3.Y = num4;
			vector3.Z = num6;
			return vector3;
		}

		public static void Clamp(in Vector3 value1, in Vector3 min, in Vector3 max, out Vector3 result)
		{
			float x = value1.X;
			float num1 = x > max.X? max.X : x;
			float num2 = num1 < min.X? min.X : num1;
			float y = value1.Y;
			float num3 = y > max.Y? max.Y : y;
			float num4 = num3 < min.Y? min.Y : num3;
			float z = value1.Z;
			float num5 = z > max.Z? max.Z : z;
			float num6 = num5 < min.Z? min.Z : num5;
			result.X = num2;
			result.Y = num4;
			result.Z = num6;
		}

		public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
		{
			Vector3 vector3;
			vector3.X = value1.X + (value2.X - value1.X) * amount;
			vector3.Y = value1.Y + (value2.Y - value1.Y) * amount;
			vector3.Z = value1.Z + (value2.Z - value1.Z) * amount;
			return vector3;
		}

		public static void Lerp(in Vector3 value1, in Vector3 value2, float amount, out Vector3 result)
		{
			result.X = value1.X + (value2.X - value1.X) * amount;
			result.Y = value1.Y + (value2.Y - value1.Y) * amount;
			result.Z = value1.Z + (value2.Z - value1.Z) * amount;
		}

		public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, float amount)
		{
			amount = (double) amount > 1.0? 1f : ((double) amount < 0.0? 0.0f : amount);
			amount = (float) (amount * (double) amount * (3.0 - 2.0 * amount));
			Vector3 vector3;
			vector3.X = value1.X + (value2.X - value1.X) * amount;
			vector3.Y = value1.Y + (value2.Y - value1.Y) * amount;
			vector3.Z = value1.Z + (value2.Z - value1.Z) * amount;
			return vector3;
		}

		public static void SmoothStep(in Vector3 value1, in Vector3 value2, float amount, out Vector3 result)
		{
			amount = amount > 1.0? 1f : (amount < 0.0? 0.0f : amount);
			amount = (float) (amount * (double) amount * (3.0 - 2.0 * amount));
			result.X = value1.X + (value2.X - value1.X) * amount;
			result.Y = value1.Y + (value2.Y - value1.Y) * amount;
			result.Z = value1.Z + (value2.Z - value1.Z) * amount;
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
			vector3.X = (float) (value1.X * (double) num3 + value2.X * (double) num4 + tangent1.X * (double) num5 +
				tangent2.X * (double) num6);
			vector3.Y = (float) (value1.Y * (double) num3 + value2.Y * (double) num4 + tangent1.Y * (double) num5 +
				tangent2.Y * (double) num6);
			vector3.Z = (float) (value1.Z * (double) num3 + value2.Z * (double) num4 + tangent1.Z * (double) num5 +
				tangent2.Z * (double) num6);
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
			result.X = (float) (value1.X * (double) num3 + value2.X * (double) num4 + tangent1.X * (double) num5 +
				tangent2.X * (double) num6);
			result.Y = (float) (value1.Y * (double) num3 + value2.Y * (double) num4 + tangent1.Y * (double) num5 +
				tangent2.Y * (double) num6);
			result.Z = (float) (value1.Z * (double) num3 + value2.Z * (double) num4 + tangent1.Z * (double) num5 +
				tangent2.Z * (double) num6);
		}

		public static Vector3 Negate(Vector3 value)
		{
			Vector3 vector3;
			vector3.X = -value.X;
			vector3.Y = -value.Y;
			vector3.Z = -value.Z;
			return vector3;
		}

		public static void Negate(in Vector3 value, out Vector3 result)
		{
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
		}

		public static Vector3 Add(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.X = value1.X + value2.X;
			vector3.Y = value1.Y + value2.Y;
			vector3.Z = value1.Z + value2.Z;
			return vector3;
		}

		public static void Add(in Vector3 value1, in Vector3 value2, out Vector3 result)
		{
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
			result.Z = value1.Z + value2.Z;
		}

		public static Vector3 Sub(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.X = value1.X - value2.X;
			vector3.Y = value1.Y - value2.Y;
			vector3.Z = value1.Z - value2.Z;
			return vector3;
		}

		public static void Sub(in Vector3 value1, in Vector3 value2, out Vector3 result)
		{
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
			result.Z = value1.Z - value2.Z;
		}

		public static Vector3 Multiply(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.X = value1.X * value2.X;
			vector3.Y = value1.Y * value2.Y;
			vector3.Z = value1.Z * value2.Z;
			return vector3;
		}

		public static void Multiply(in Vector3 value1, in Vector3 value2, out Vector3 result)
		{
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
			result.Z = value1.Z * value2.Z;
		}

		public static Vector3 Multiply(Vector3 value1, float scaleFactor)
		{
			Vector3 vector3;
			vector3.X = value1.X * scaleFactor;
			vector3.Y = value1.Y * scaleFactor;
			vector3.Z = value1.Z * scaleFactor;
			return vector3;
		}

		public static void Multiply(in Vector3 value1, float scaleFactor, out Vector3 result)
		{
			result.X = value1.X * scaleFactor;
			result.Y = value1.Y * scaleFactor;
			result.Z = value1.Z * scaleFactor;
		}

		public static Vector3 Divide(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.X = value1.X / value2.X;
			vector3.Y = value1.Y / value2.Y;
			vector3.Z = value1.Z / value2.Z;
			return vector3;
		}

		public static void Divide(in Vector3 value1, in Vector3 value2, out Vector3 result)
		{
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
			result.Z = value1.Z / value2.Z;
		}

		public static Vector3 Divide(Vector3 value1, float divider)
		{
			float num = 1f / divider;
			Vector3 vector3;
			vector3.X = value1.X * num;
			vector3.Y = value1.Y * num;
			vector3.Z = value1.Z * num;
			return vector3;
		}

		public static void Divide(in Vector3 value1, float divider, out Vector3 result)
		{
			float num = 1f / divider;
			result.X = value1.X * num;
			result.Y = value1.Y * num;
			result.Z = value1.Z * num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float magnitudeStatic(in Vector3 inV)
		{
			return (float) Math.Sqrt((float)(inV.X * (double)inV.X + inV.Y * (double)inV.Y + inV.Z * (double)inV.Z));
		}

		private static Vector3 orthoNormalVectorFast(in Vector3 n)
		{
			Vector3 vector3;
			if (Math.Abs(n.Z) > (double) K1OverSqrt2)
			{
				float num = 1f / (float) Math.Sqrt(n.Y * (double) n.Y + n.Z * (double) n.Z);
				vector3.X = 0.0f;
				vector3.Y = -n.Z * num;
				vector3.Z = n.Y * num;
			}
			else
			{
				float num = 1f / (float) Math.Sqrt(n.X * (double) n.X + n.Y * (double) n.Y);
				vector3.X = -n.Y * num;
				vector3.Y = n.X * num;
				vector3.Z = 0.0f;
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
			if (dot > -Epsilon && dot < Epsilon)
			{
				result = Zero;
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
			if (dot > -Epsilon && dot < Epsilon)
			{
				result = Zero;
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
			vector3.X = -value.X;
			vector3.Y = -value.Y;
			vector3.Z = -value.Z;
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
			vector3.X = value1.X + value2.X;
			vector3.Y = value1.Y + value2.Y;
			vector3.Z = value1.Z + value2.Z;
			return vector3;
		}

		public static Vector3 operator +(Vector3 value1, float offset)
		{
			value1.X = value1.X + offset;
			value1.Y = value1.Y + offset;
			value1.Z = value1.Z + offset;
			return value1;
		}

		public static Vector3 operator -(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.X = value1.X - value2.X;
			vector3.Y = value1.Y - value2.Y;
			vector3.Z = value1.Z - value2.Z;
			return vector3;
		}

		public static Vector3 operator -(Vector3 value1, float offset)
		{
			value1.X = value1.X - offset;
			value1.Y = value1.Y - offset;
			value1.Z = value1.Z - offset;
			return value1;
		}

		public static Vector3 operator *(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.X = value1.X * value2.X;
			vector3.Y = value1.Y * value2.Y;
			vector3.Z = value1.Z * value2.Z;
			return vector3;
		}

		public static Vector3 operator *(Vector3 value, float scaleFactor)
		{
			Vector3 vector3;
			vector3.X = value.X * scaleFactor;
			vector3.Y = value.Y * scaleFactor;
			vector3.Z = value.Z * scaleFactor;
			return vector3;
		}

		public static Vector3 operator *(float scaleFactor, Vector3 value)
		{
			Vector3 vector3;
			vector3.X = value.X * scaleFactor;
			vector3.Y = value.Y * scaleFactor;
			vector3.Z = value.Z * scaleFactor;
			return vector3;
		}

		public static Vector3 operator /(Vector3 value1, Vector3 value2)
		{
			Vector3 vector3;
			vector3.X = value1.X / value2.X;
			vector3.Y = value1.Y / value2.Y;
			vector3.Z = value1.Z / value2.Z;
			return vector3;
		}

		public static Vector3 operator /(Vector3 value, float divider)
		{
			float num = 1f / divider;
			Vector3 vector3;
			vector3.X = value.X * num;
			vector3.Y = value.Y * num;
			vector3.Z = value.Z * num;
			return vector3;
		}

		public static Vector3 TransformNormal(Vector3 normal, Matrix4x4 matrix)
		{
			float num1 = (float) ((double) normal.X * (double) matrix.m00 + (double) normal.Y * (double) matrix.m01 + (double) normal.Z * (double) matrix.m02);
			float num2 = (float) ((double) normal.X * (double) matrix.m10 + (double) normal.Y * (double) matrix.m11 + (double) normal.Z * (double) matrix.m12);
			float num3 = (float) ((double) normal.X * (double) matrix.m20 + (double) normal.Y * (double) matrix.m21 + (double) normal.Z * (double) matrix.m22);
			return new Vector3(num1, num2, num3);
		}
	}
}