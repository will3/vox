using System.Collections;
using System.Collections.Generic;
using FarmVox;
using FarmVox.Threading;
using UnityEngine;
using UnityEngine.AI;
using Terrian = FarmVox.Terrain.Terrian;

namespace FarmVox.Scripts
{
	public class GameController : MonoBehaviour
	{
		public bool drawRoutes = false;

		public Terrian Terrian { get; private set; }

		private readonly WorkerQueue _queue = new WorkerQueue();

		public WorkerQueue Queue
		{
			get { return _queue; }
		}

		// Use this for initialization
		void Start()
		{
			Terrian = new Terrian();

			Terrian.InitColumns();

			Terrian.Start();

			StartCoroutine(Terrian.UpdateMeshesLoop());
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

		void OnDestroy()
		{
			if (Terrian != null) Terrian.Dispose();
		}

		void OnDrawGizmosSelected()
		{
			//TaskMap.Instance.OnDrawGizmosSelected();	
		}
	}
}