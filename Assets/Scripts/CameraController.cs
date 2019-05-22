using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
	private static CameraController _instance;
	public static CameraController instance { get { return _instance; } }

	public Transform followTarget;
	public float sensitivity = 5;
	//

	private void Start()
	{
		_instance = this;
	}

	private void LateUpdate()
    {
        if(followTarget)
		{
			// Плавно перемещаемся от мяча к мячу
			transform.position = Vector3.Lerp(transform.position, followTarget.transform.position, 10 * Time.deltaTime);
		}
    }

	public void PointerDrag(BaseEventData bed)
	{
		// Перемещение воуруг мяча
		PointerEventData ped = (PointerEventData)bed;
		transform.rotation *= Quaternion.AngleAxis(ped.delta.x * sensitivity * Time.unscaledDeltaTime, Vector3.up) * Quaternion.AngleAxis(-ped.delta.y * sensitivity * Time.unscaledDeltaTime, Vector3.right);
	}
}
