using UnityEngine;

public class MobaMainCamera : MonoBehaviour
{
	void Awake()
	{
		MainCameraCtrl = this;
		MainCamera = GetComponent<Camera>();
	}

	void LateUpdate()
	{
		if (target == null)
			return;

		// θ、φ、r的取值范围;
		if (angleX < 0.0f)
		{
			angleX = 360.0f + (angleX % 360.0f);
		}
		if (angleX > 360.0f)
		{
			angleX = angleX % 360.0f;
		}
		angleY = Mathf.Clamp(angleY, minAngleY, maxAngleY);
		radius = Mathf.Clamp(radius, minRadius, maxRadius);

		// 球坐标系转笛卡尔坐标系;
		float theta = Mathf.Deg2Rad * (90.0f - angleY);
		float phi = Mathf.Deg2Rad * angleX;
		float x = radius * Mathf.Sin(theta) * Mathf.Cos(phi);
		float y = radius * Mathf.Cos(theta);
		float z = radius * Mathf.Sin(theta) * Mathf.Sin(phi);

		transform.position = target.position + new Vector3(x, y, z);
		transform.LookAt(target, Vector3.up);
	}

	public static Camera MainCamera = null;
	public static MobaMainCamera MainCameraCtrl = null;

	public Transform target = null;

	public float minAngleY = 25.0f;
	public float maxAngleY = 75.0f;

	public float minRadius = 0.0f;
	public float maxRadius = 20.0f;

	public float angleX = 180.0f;
	public float angleY = 55.0f;
	public float radius = 10.0f;
}