using System;
using System.Collections;
using System.Collections.Generic;
using FarmVox;
using FarmVox.Terrain;
using FarmVox.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace FarmVox.Scripts
{
	public class GameController : MonoBehaviour
	{
		public static GameController Instance;
		
		public bool drawRoutes = false;

		private readonly WorkerQueue _queue = new WorkerQueue();

		public WorkerQueue Queue
		{
			get { return _queue; }
		}

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				throw new Exception("Only one GameController allowed");
			}
		}

		// Use this for initialization
		void Start()
		{
			StartCoroutine(_queue.DoWork());
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Mouse1))
			{
				//RaycastHit hit;
				//var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				//if (Physics.Raycast(ray, out hit))
				//{
				//    foreach (var actor in actors) {
				//        actor.SetFormationDestination(hit.point);
				//    }
				//}
			}
		}

		void OnDrawGizmosSelected()
		{
			//TaskMap.Instance.OnDrawGizmosSelected();	
		}
	}
}