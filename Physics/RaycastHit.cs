using System;

namespace Y7Engine
{
	public struct RaycastHit
	{
		public object collider { get; set; }
		//
		// 摘要:
		//     The impact point in world space where the ray hit the collider.
		public Vector3 point { get; set; }
		//
		// 摘要:
		//     The normal of the surface the ray hit.
		public Vector3 normal { get; set; }
		//
		// 摘要:
		//     The barycentric coordinate of the triangle we hit.
		public Vector3 barycentricCoordinate { get; set; }
		//
		// 摘要:
		//     The distance from the ray's origin to the impact point.
		public float distance { get; set; }
		//
		// 摘要:
		//     The index of the triangle that was hit.
		public int triangleIndex { get; set; }
		//
		// 摘要:
		//     The uv texture coordinate at the collision location.
		public Vector2 textureCoord { get; set; }
		//
		// 摘要:
		//     The Transform of the rigidbody or collider that was hit.
		public Transform transform { get; set; }
		//
		// 摘要:
		//     The secondary uv texture coordinate at the impact point.
		public Vector2 textureCoord2 { get; set; }
		[Obsolete("Use textureCoord2 instead. (UnityUpgradable) -> textureCoord2")]
		public Vector2 textureCoord1 { get; set; }
		//
		// 摘要:
		//     The uv lightmap coordinate at the impact point.
		public Vector2 lightmapCoord { get; set; }
	}
}
