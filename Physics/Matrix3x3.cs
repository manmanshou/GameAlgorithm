using System.Globalization;
namespace Y7Engine
{
    public struct Matrix3x3 : IEquatable<Matrix3x3>
    {
        public float A1;
        public float A2;
        public float A3;
        public float B1;
        public float B2;
        public float B3;
        public float C1;
        public float C2;
        public float C3;
        private static Matrix3x3 _identity = new Matrix3x3(1f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 1f);

        public static Matrix3x3 Identity => Matrix3x3._identity;

        public bool IsIdentity
        {
            get
            {
                float num = 0.01f;
                return (double)this.A2 <= (double)num && (double)this.A2 >= -(double)num && ((double)this.A3 <= (double)num && (double)this.A3 >= -(double)num) && ((double)this.B1 <= (double)num && (double)this.B1 >= -(double)num && ((double)this.B3 <= (double)num && (double)this.B3 >= -(double)num)) && ((double)this.C1 <= (double)num && (double)this.C1 >= -(double)num && ((double)this.C2 <= (double)num && (double)this.C2 >= -(double)num) && ((double)this.A1 <= 1.0 + (double)num && (double)this.A1 >= 1.0 - (double)num && ((double)this.B2 <= 1.0 + (double)num && (double)this.B2 >= 1.0 - (double)num))) && (double)this.C3 <= 1.0 + (double)num && (double)this.C3 >= 1.0 - (double)num;
            }
        }

        public float this[int i, int j]
        {
            get
            {
                switch (i)
                {
                    case 1:
                        switch (j)
                        {
                            case 1:
                                return this.A1;
                            case 2:
                                return this.A2;
                            case 3:
                                return this.A3;
                            default:
                                return 0.0f;
                        }
                    case 2:
                        switch (j)
                        {
                            case 1:
                                return this.B1;
                            case 2:
                                return this.B2;
                            case 3:
                                return this.B3;
                            default:
                                return 0.0f;
                        }
                    case 3:
                        switch (j)
                        {
                            case 1:
                                return this.C1;
                            case 2:
                                return this.C2;
                            case 3:
                                return this.C3;
                            default:
                                return 0.0f;
                        }
                    default:
                        return 0.0f;
                }
            }
            set
            {
                switch (i)
                {
                    case 1:
                        switch (j)
                        {
                            case 1:
                                this.A1 = value;
                                return;
                            case 2:
                                this.A2 = value;
                                return;
                            case 3:
                                this.A3 = value;
                                return;
                            default:
                                return;
                        }
                    case 2:
                        switch (j)
                        {
                            case 1:
                                this.B1 = value;
                                return;
                            case 2:
                                this.B2 = value;
                                return;
                            case 3:
                                this.B3 = value;
                                return;
                            default:
                                return;
                        }
                    case 3:
                        switch (j)
                        {
                            case 1:
                                this.C1 = value;
                                return;
                            case 2:
                                this.C2 = value;
                                return;
                            case 3:
                                this.C3 = value;
                                return;
                            default:
                                return;
                        }
                }
            }
        }

        public Matrix3x3(
          float a1,
          float a2,
          float a3,
          float b1,
          float b2,
          float b3,
          float c1,
          float c2,
          float c3)
        {
            this.A1 = a1;
            this.A2 = a2;
            this.A3 = a3;
            this.B1 = b1;
            this.B2 = b2;
            this.B3 = b3;
            this.C1 = c1;
            this.C2 = c2;
            this.C3 = c3;
        }

        public Matrix3x3(Matrix4x4 rotMatrix)
        {
            this.A1 = rotMatrix.m00;
            this.A2 = rotMatrix.m01;
            this.A3 = rotMatrix.m02;
            this.B1 = rotMatrix.m10;
            this.B2 = rotMatrix.m11;
            this.B3 = rotMatrix.m12;
            this.C1 = rotMatrix.m20;
            this.C2 = rotMatrix.m21;
            this.C3 = rotMatrix.m22;
        }

        public Matrix3x3(Matrix3x3 m)
        {
            this.A1 = m.A1;
            this.A2 = m.A2;
            this.A3 = m.A3;
            this.B1 = m.B1;
            this.B2 = m.B2;
            this.B3 = m.B3;
            this.C1 = m.C1;
            this.C2 = m.C2;
            this.C3 = m.C3;        
        }

        public void Transpose()
        {
            Matrix3x3 matrix3x3 = new Matrix3x3(this);
            this.A2 = matrix3x3.B1;
            this.A3 = matrix3x3.C1;
            this.B1 = matrix3x3.A2;
            this.B3 = matrix3x3.C2;
            this.C1 = matrix3x3.A3;
            this.C2 = matrix3x3.B3;
        }

        public void Inverse()
        {
            float num1 = this.Determinant();
            if ((double)num1 == 0.0)
            {
                this.A1 = float.NaN;
                this.A2 = float.NaN;
                this.A3 = float.NaN;
                this.B1 = float.NaN;
                this.B2 = float.NaN;
                this.B3 = float.NaN;
                this.C1 = float.NaN;
                this.C2 = float.NaN;
                this.C3 = float.NaN;
            }
            float num2 = 1f / num1;
            float num3 = num2 * (float)((double)this.B2 * (double)this.C3 - (double)this.B3 * (double)this.C2);
            float num4 = (float)(-(double)num2 * ((double)this.A2 * (double)this.C3 - (double)this.A3 * (double)this.C2));
            float num5 = num2 * (float)((double)this.A2 * (double)this.B3 - (double)this.A3 * (double)this.B2);
            float num6 = (float)(-(double)num2 * ((double)this.B1 * (double)this.C3 - (double)this.B3 * (double)this.C1));
            float num7 = num2 * (float)((double)this.A1 * (double)this.C3 - (double)this.A3 * (double)this.C1);
            float num8 = (float)(-(double)num2 * ((double)this.A1 * (double)this.B3 - (double)this.A3 * (double)this.B1));
            float num9 = num2 * (float)((double)this.B1 * (double)this.C2 - (double)this.B2 * (double)this.C1);
            float num10 = (float)(-(double)num2 * ((double)this.A1 * (double)this.C2 - (double)this.A2 * (double)this.C1));
            float num11 = num2 * (float)((double)this.A1 * (double)this.B2 - (double)this.A2 * (double)this.B1);
            this.A1 = num3;
            this.A2 = num4;
            this.A3 = num5;
            this.B1 = num6;
            this.B2 = num7;
            this.B3 = num8;
            this.C1 = num9;
            this.C2 = num10;
            this.C3 = num11;
        }

        public float Determinant() => (float)((double)this.A1 * (double)this.B2 * (double)this.C3 - (double)this.A1 * (double)this.B3 * (double)this.C2 + (double)this.A2 * (double)this.B3 * (double)this.C1 - (double)this.A2 * (double)this.B1 * (double)this.C3 + (double)this.A3 * (double)this.B1 * (double)this.C2 - (double)this.A3 * (double)this.B2 * (double)this.C1);

        public static Matrix3x3 FromEulerAnglesXYZ(float x, float y, float z)
        {
            float num1 = (float)Math.Cos((double)x);
            float num2 = (float)Math.Sin((double)x);
            float num3 = (float)Math.Cos((double)y);
            float num4 = (float)Math.Sin((double)y);
            float num5 = (float)Math.Cos((double)z);
            float num6 = (float)Math.Sin((double)z);
            float num7 = num2 * num4;
            float num8 = num1 * num4;
            Matrix3x3 matrix3x3;
            matrix3x3.A1 = num3 * num5;
            matrix3x3.A2 = num3 * num6;
            matrix3x3.A3 = -num4;
            matrix3x3.B1 = (float)((double)num7 * (double)num5 - (double)num1 * (double)num6);
            matrix3x3.B2 = (float)((double)num7 * (double)num6 + (double)num1 * (double)num5);
            matrix3x3.B3 = num2 * num3;
            matrix3x3.C1 = (float)((double)num8 * (double)num5 + (double)num2 * (double)num6);
            matrix3x3.C2 = (float)((double)num8 * (double)num6 - (double)num2 * (double)num5);
            matrix3x3.C3 = num1 * num3;
            return matrix3x3;
        }
        public static Matrix3x3 LookRotationToMatrix(Vector3 forward, Vector3 up)
        {
            forward.Normalize();
            up.Normalize();

            // Cross product of the forward and up vectors gives the right vector
            Vector3 right = Vector3.Cross(forward, up);
            right.Normalize();

            // Cross product of the right and forward vectors gives the actual up vector
            Vector3 actualUp = Vector3.Cross(right, forward);

            Matrix3x3 result = new Matrix3x3();
            result.A1 = right.X;
            result.A2 = right.Y;
            result.A3 = right.Z;

            result.B1 = actualUp.X;
            result.B2 = actualUp.Y;
            result.B3 = actualUp.Z;

            result.C1 = forward.X;
            result.C2 = forward.Y;
            result.C3 = forward.Z;
            return result;
        }
        public static Matrix3x3 FromEulerAnglesXYZ(Vector3 angles) => Matrix3x3.FromEulerAnglesXYZ(angles.X, angles.Y, angles.Z);

        public static Matrix3x3 FromRotationX(float radians)
        {
            Matrix3x3 identity = Matrix3x3.Identity;
            identity.B2 = identity.C3 = (float)Math.Cos((double)radians);
            identity.C2 = (float)Math.Sin((double)radians);
            identity.B3 = -identity.C2;
            return identity;
        }

        public static Matrix3x3 FromRotationY(float radians)
        {
            Matrix3x3 identity = Matrix3x3.Identity;
            identity.A1 = identity.C3 = (float)Math.Cos((double)radians);
            identity.A3 = (float)Math.Sin((double)radians);
            identity.C1 = -identity.A3;
            return identity;
        }

        public static Matrix3x3 FromRotationZ(float radians)
        {
            Matrix3x3 identity = Matrix3x3.Identity;
            identity.A1 = identity.B2 = (float)Math.Cos((double)radians);
            identity.B1 = (float)Math.Sin((double)radians);
            identity.A2 = -identity.B1;
            return identity;
        }

        public static Matrix3x3 FromAngleAxis(float radians, Vector3 axis)
        {
            float x = axis.X;
            float y = axis.Y;
            float z = axis.Z;
            float num1 = (float)Math.Sin((double)radians);
            float num2 = (float)Math.Cos((double)radians);
            float num3 = x * x;
            float num4 = y * y;
            float num5 = z * z;
            float num6 = x * y;
            float num7 = x * z;
            float num8 = y * z;
            Matrix3x3 matrix3x3;
            matrix3x3.A1 = num3 + num2 * (1f - num3);
            matrix3x3.B1 = (float)((double)num6 - (double)num2 * (double)num6 + (double)num1 * (double)z);
            matrix3x3.C1 = (float)((double)num7 - (double)num2 * (double)num7 - (double)num1 * (double)y);
            matrix3x3.A2 = (float)((double)num6 - (double)num2 * (double)num6 - (double)num1 * (double)z);
            matrix3x3.B2 = num4 + num2 * (1f - num4);
            matrix3x3.C2 = (float)((double)num8 - (double)num2 * (double)num8 + (double)num1 * (double)x);
            matrix3x3.A3 = (float)((double)num7 - (double)num2 * (double)num7 + (double)num1 * (double)y);
            matrix3x3.B3 = (float)((double)num8 - (double)num2 * (double)num8 - (double)num1 * (double)x);
            matrix3x3.C3 = num5 + num2 * (1f - num5);
            return matrix3x3;
        }

        public static Matrix3x3 FromScaling(Vector3 scaling)
        {
            Matrix3x3 identity = Matrix3x3.Identity;
            identity.A1 = scaling.X;
            identity.B2 = scaling.Y;
            identity.C3 = scaling.Z;
            return identity;
        }

        public static Matrix3x3 FromToMatrix(Vector3 from, Vector3 to)
        {
            float num1 = Vector3.Dot(from, to);
            float num2 = (double)num1 < 0.0 ? -num1 : num1;
            Matrix3x3 identity = Matrix3x3.Identity;
            if ((double)num2 > 0.999989986419678)
            {
                Vector3 vector3D1;
                vector3D1.X = (double)from.X > 0.0 ? from.X : -from.X;
                vector3D1.Y = (double)from.Y > 0.0 ? from.Y : -from.Y;
                vector3D1.Z = (double)from.Z > 0.0 ? from.Z : -from.Z;
                if ((double)vector3D1.X < (double)vector3D1.Y)
                {
                    if ((double)vector3D1.X < (double)vector3D1.Z)
                    {
                        vector3D1.X = 1f;
                        vector3D1.Y = 0.0f;
                        vector3D1.Z = 0.0f;
                    }
                    else
                    {
                        vector3D1.X = 0.0f;
                        vector3D1.Y = 0.0f;
                        vector3D1.Z = 1f;
                    }
                }
                else if ((double)vector3D1.Y < (double)vector3D1.Z)
                {
                    vector3D1.X = 0.0f;
                    vector3D1.Y = 1f;
                    vector3D1.Z = 0.0f;
                }
                else
                {
                    vector3D1.X = 0.0f;
                    vector3D1.Y = 0.0f;
                    vector3D1.Z = 1f;
                }
                Vector3 vector3D2;
                vector3D2.X = vector3D1.X - from.X;
                vector3D2.Y = vector3D1.Y - from.Y;
                vector3D2.Z = vector3D1.Z - from.Z;
                Vector3 vector3D3;
                vector3D3.X = vector3D1.X - to.X;
                vector3D3.Y = vector3D1.Y - to.Y;
                vector3D3.Z = vector3D1.Z - to.Z;
                float num3 = 2f / Vector3.Dot(vector3D2, vector3D2);
                float num4 = 2f / Vector3.Dot(vector3D3, vector3D3);
                float num5 = num3 * num4 * Vector3.Dot(vector3D2, vector3D3);
                for (int index = 1; index < 4; ++index)
                {
                    for (int j = 1; j < 4; ++j)
                        identity[index, j] = (float)(-(double)num3 * (double)vector3D2[index - 1] * (double)vector3D2[j - 1] - (double)num4 * (double)vector3D3[index - 1] * (double)vector3D3[j - 1] + (double)num5 * (double)vector3D3[index - 1] * (double)vector3D2[j - 1]);
                    ++identity[index, index];
                }
            }
            else
            {
                Vector3 vector3D = Vector3.Cross(from, to);
                float num3 = (float)(1.0 / (1.0 + (double)num1));
                float num4 = num3 * vector3D.X;
                float num5 = num3 * vector3D.Z;
                float num6 = num4 * vector3D.Y;
                float num7 = num4 * vector3D.Z;
                float num8 = num5 * vector3D.Y;
                identity.A1 = num1 + num4 * vector3D.X;
                identity.A2 = num6 - vector3D.Z;
                identity.A3 = num7 + vector3D.Y;
                identity.B1 = num6 + vector3D.Z;
                identity.B2 = num1 + num3 * vector3D.Y * vector3D.Y;
                identity.B3 = num8 - vector3D.X;
                identity.C1 = num7 - vector3D.Y;
                identity.C2 = num8 + vector3D.X;
                identity.C3 = num1 + num5 * vector3D.Z;
            }
            return identity;
        }

        public static bool operator ==(Matrix3x3 a, Matrix3x3 b) => (double)a.A1 == (double)b.A1 && (double)a.A2 == (double)b.A2 && ((double)a.A3 == (double)b.A3 && (double)a.B1 == (double)b.B1) && ((double)a.B2 == (double)b.B2 && (double)a.B3 == (double)b.B3) && ((double)a.C1 == (double)b.C1 && (double)a.C2 == (double)b.C2 && (double)a.C3 == (double)b.C3);

        public static bool operator !=(Matrix3x3 a, Matrix3x3 b) => (double)a.A1 != (double)b.A1 || (double)a.A2 != (double)b.A2 || ((double)a.A3 != (double)b.A3 || (double)a.B1 != (double)b.B1) || ((double)a.B2 != (double)b.B2 || (double)a.B3 != (double)b.B3) || ((double)a.C1 != (double)b.C1 || (double)a.C2 != (double)b.C2 || (double)a.C3 != (double)b.C3);

        public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b) => new Matrix3x3((float)((double)a.A1 * (double)b.A1 + (double)a.B1 * (double)b.A2 + (double)a.C1 * (double)b.A3), (float)((double)a.A2 * (double)b.A1 + (double)a.B2 * (double)b.A2 + (double)a.C2 * (double)b.A3), (float)((double)a.A3 * (double)b.A1 + (double)a.B3 * (double)b.A2 + (double)a.C3 * (double)b.A3), (float)((double)a.A1 * (double)b.B1 + (double)a.B1 * (double)b.B2 + (double)a.C1 * (double)b.B3), (float)((double)a.A2 * (double)b.B1 + (double)a.B2 * (double)b.B2 + (double)a.C2 * (double)b.B3), (float)((double)a.A3 * (double)b.B1 + (double)a.B3 * (double)b.B2 + (double)a.C3 * (double)b.B3), (float)((double)a.A1 * (double)b.C1 + (double)a.B1 * (double)b.C2 + (double)a.C1 * (double)b.C3), (float)((double)a.A2 * (double)b.C1 + (double)a.B2 * (double)b.C2 + (double)a.C2 * (double)b.C3), (float)((double)a.A3 * (double)b.C1 + (double)a.B3 * (double)b.C2 + (double)a.C3 * (double)b.C3));

        public static implicit operator Matrix3x3(Matrix4x4 mat)
        {
            Matrix3x3 matrix3x3;
            matrix3x3.A1 = mat.m00;
            matrix3x3.A2 = mat.m01;
            matrix3x3.A3 = mat.m02;
            matrix3x3.B1 = mat.m10;
            matrix3x3.B2 = mat.m11;
            matrix3x3.B3 = mat.m12;
            matrix3x3.C1 = mat.m20;
            matrix3x3.C2 = mat.m21;
            matrix3x3.C3 = mat.m22;
            return matrix3x3;
        }
        public Vector3 GetColumn(int index)
        {
            Vector3 vector3;
            vector3.X = this[0, index];
            vector3.Y = this[1, index];
            vector3.Z = this[2, index];
            return vector3;
        }        

        public bool Equals(Matrix3x3 other) => (double)this.A1 == (double)other.A1 && (double)this.A2 == (double)other.A2 && ((double)this.A3 == (double)other.A3 && (double)this.B1 == (double)other.B1) && ((double)this.B2 == (double)other.B2 && (double)this.B3 == (double)other.B3) && ((double)this.C1 == (double)other.C1 && (double)this.C2 == (double)other.C2 && (double)this.C3 == (double)other.C3);

        public override bool Equals(object obj) => obj is Matrix3x3 other && this.Equals(other);

        public override int GetHashCode() => this.A1.GetHashCode() + this.A2.GetHashCode() + this.A3.GetHashCode() + this.B1.GetHashCode() + this.B2.GetHashCode() + this.B3.GetHashCode() + this.C1.GetHashCode() + this.C2.GetHashCode() + this.C3.GetHashCode();

        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            object[] objArray = new object[9]
            {
                (object) this.A1.ToString((IFormatProvider) currentCulture),
                (object) this.A2.ToString((IFormatProvider) currentCulture),
                (object) this.A3.ToString((IFormatProvider) currentCulture),
                (object) this.B1.ToString((IFormatProvider) currentCulture),
                (object) this.B2.ToString((IFormatProvider) currentCulture),
                (object) this.B3.ToString((IFormatProvider) currentCulture),
                (object) this.C1.ToString((IFormatProvider) currentCulture),
                (object) this.C2.ToString((IFormatProvider) currentCulture),
                (object) this.C3.ToString((IFormatProvider) currentCulture)
            };
            return string.Format((IFormatProvider)currentCulture, "{{[A1:{0} A2:{1} A3:{2}] [B1:{3} B2:{4} B3:{5}] [C1:{6} C2:{7} C3:{8}]}}", objArray);
        }
    }
}