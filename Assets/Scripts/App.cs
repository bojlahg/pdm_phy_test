using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
	/*private static App _instance;
	public static App instance { get { return _instance; } }*/

	public class Data
	{
		public float[] x, y, z;
	}

	public CameraController camCtl;
	public GameObject ballPrefab;
	public TextAsset[] ballTextAssets;
	public Slider velSlider;
	//
	private int followIndex = 0;
	private List<Ball> balls = new List<Ball>();

	private void Start()
    {
	//	_instance = this;

		Application.targetFrameRate = 60;

		
		foreach(TextAsset ta in ballTextAssets)
		{
			// Загружаем данные из json
			Data bd = JsonUtility.FromJson<Data>(ta.text);
			// Создаем мячи из префабов
			GameObject go = (GameObject)GameObject.Instantiate(ballPrefab);
			Ball ball = go.GetComponent<Ball>();
			if(ball)
			{
				// Передаем мячу его траекторию
				ball.Init(bd);
				balls.Add(ball);
			}
		}

		// Заставляем следить камеру за первым мячем
		if(balls.Count > 0)
		{
			camCtl.followTarget = balls[followIndex].transform;
		}

	}

	public void Prev()
	{
		// Переходим к предыдущему мячу
		balls[followIndex].velocity = 0;

		followIndex = (followIndex - 1) % balls.Count;
		if(followIndex < 0)
		{
			followIndex += balls.Count;
		}
		camCtl.followTarget = balls[followIndex].transform;

		balls[followIndex].velocity = velSlider.value * 10;
	}

	public void Next()
	{
		// Переходим к следующему мячу
		balls[followIndex].velocity = 0;

		followIndex = (followIndex + 1) % balls.Count;
		camCtl.followTarget = balls[followIndex].transform;

		balls[followIndex].velocity = velSlider.value * 10;
	}

	public void OnVelocityChanged(float val)
	{
		// Изменение скорости текущего мяча
		balls[followIndex].velocity = val * 10;
	}
}
