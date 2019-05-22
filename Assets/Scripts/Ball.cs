using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ball : MonoBehaviour
{
	public enum State
	{
		IdleAtStart, // начало
		MoveStartToFinish, // перемещаемся вперед
		IdleAtFinish, // конец
		MoveFinishToStart, // перемещаемся назад
	}

	// класс для удобного получения точек и в обратном порядке тоже
	public class PosData
	{
		public PosData(float[] x, float[] y, float[] z)
		{
			int minLen = Mathf.Min(x.Length, y.Length, z.Length);
			data = new Vector3[minLen];
			for(int i = 0; i < minLen; ++i)
			{
				data[i] = new Vector3(x[i], y[i], z[i]);
			}
		}

		private Vector3[] data;
		public bool reversed = false;

		public Vector3 this[int idx]
		{
			get
			{
				return data[reversed ? (data.Length - idx - 1) : idx];
			}
		}

		public int length
		{
			get
			{
				return data.Length;
			}
		}
	}

	public LineRenderer lineRenderer;

	[HideInInspector]
	public float velocity = 5;
	//
	private PosData posData;
	private float pos = 0, minPos = 0, maxPos = 0;
	private int pathIdx = 0;
	private State state = State.IdleAtStart;
	private float doubleClickTimer = 0;

	public void Init(App.Data data)
	{
		posData = new PosData(data.x, data.y, data.z);
		
		Vector3 p1 = posData[0], p2 = posData[1];
		maxPos = (p2 - p1).magnitude;

		// перемещакмся в стартовую точку
		transform.position = p1;
	}

	private void Update()
    {
		switch(state)
		{
		case State.MoveStartToFinish:
		case State.MoveFinishToStart:
			// движемся ли обратно?
			posData.reversed = state == State.MoveFinishToStart;
			int pi1, pi2;
			Vector3 p1, p2;
			// инкрементируем позицию
			pos += velocity * Time.deltaTime;
			if(pos <= maxPos)
			{
				// позиция между двух текущих точек
				pi1 = pathIdx;
				pi2 = pathIdx + 1;
				p1 = posData[pi1];
				p2 = posData[pi2];
			}
			else
			{
				if(pathIdx + 1 == posData.length - 1)
				{
					// закончились точки пути
					state = State.IdleAtFinish;
					p1 = posData[posData.length - 1];
					p2 = posData[posData.length - 1];
				}
				else
				{
					// ищем следующую подходящую точку
					{
						++pathIdx;
						minPos = maxPos;
						pi1 = pathIdx;
						pi2 = pathIdx + 1;
						p1 = posData[pi1];
						p2 = posData[pi2];
						maxPos += (p2 - p1).magnitude;
					}
					while(maxPos < pos && pi2 < posData.length);
				}
			}
			// положение между текущими крайними точками
			float t = (pos - minPos) / (maxPos - minPos);
			transform.position = Vector3.Lerp(p1, p2, t);

			// загружаем данные для отрисовки траектории
			lineRenderer.positionCount = pathIdx + 2;
			for(int i = 0; i < pathIdx + 1; ++i)
			{
				lineRenderer.SetPosition(i, posData[i]);
			}
			lineRenderer.SetPosition(pathIdx + 1, transform.position);

			break;
		}
		// Уменьшаем время ожидания двойного щелчка
		doubleClickTimer -= Time.unscaledDeltaTime;
	}

	public void Reset()
	{
		// Сброс
		pos = 0;
		minPos = 0;
		Vector3 p1 = posData[0], p2 = posData[1];
		maxPos = (p2 - p1).magnitude;
		pathIdx = 0;
	}

	public void Pick(BaseEventData bed)
	{
		PointerEventData ped = (PointerEventData)bed;

		if(doubleClickTimer > 0)
		{
			// Двойной щелчок
			doubleClickTimer = 0;

			Reset();
			state = State.MoveStartToFinish;
		}
		else
		{
			// Ждем двойного щелчка 0.3 сек
			doubleClickTimer = 0.3f;
			// Одинарный щелчок
			if(state == State.IdleAtStart)
			{
				// находимся в начале - едем вперед
				Reset();
				state = State.MoveStartToFinish;
			}
			else if(state == State.IdleAtFinish)
			{
				// находимся в конце - едем назад
				Reset();
				state = State.MoveFinishToStart;
			}
		}
	}

	public void PointerDrag(BaseEventData bed)
	{
		// Передаем событие камере
		CameraController.instance.PointerDrag(bed);
	}
}
