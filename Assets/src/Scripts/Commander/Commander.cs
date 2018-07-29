using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public partial class Commander : MonoBehaviour
    {
        enum DesignationType {
            Dig    
        }

        public Material designationMaterial;
        public Material wireframeMaterial;

        Command currentCommand;
        readonly List<Designation> designationList = new List<Designation>();
        GameObject boxObject;

        void Start()
		{
            boxObject = new GameObject("box");
            boxObject.AddComponent<MeshFilter>().sharedMesh = Cube.CubeMesh;
            boxObject.AddComponent<MeshRenderer>().sharedMaterial = designationMaterial;
            boxObject.SetActive(false);
		}

		void Update()
        {
            if (Input.GetKeyDown(KeyCode.G)) {
                currentCommand = new Command(CommandType.Dig);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                currentCommand = null;
            }

            ProcessCommand();
        }

        public HashSet<Vector3Int> GetNonEmptyCoords()
        {
            if (currentCommand.Bounds == null) {
                return new HashSet<Vector3Int>();
            }

            var bounds = currentCommand.Bounds.Value;
            var layer = Finder.FindTerrian().DefaultLayer;

            var coords = new HashSet<Vector3Int>();
            for (var x = bounds.min.x; x <= bounds.max.x; x++)
            {
                for (var y = bounds.min.y; y <= bounds.max.y; y++)
                {
                    for (var z = bounds.min.z; z <= bounds.max.z; z++)
                    {
                        var coord = new Vector3Int(
                            Mathf.FloorToInt(x),
                            Mathf.FloorToInt(y),
                            Mathf.FloorToInt(z));
                        if (layer.Get(coord) > 0)
                        {
                            coords.Add(coord);
                        }
                    }
                }
            }

            return coords;
        }

        void ProcessCommand() {
            if (currentCommand == null)
            {
                return;
            }

            if (currentCommand.type == CommandType.Dig)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    var result = VoxelRaycast.TraceMouse();
                    if (result != null)
                    {
                        if (currentCommand.startCoord == null)
                        {
                            currentCommand.startCoord = result.GetCoord();
                        } else {
                            currentCommand.endCoord = result.GetCoord();

                            boxObject.SetActive(true);
                            boxObject.transform.position = currentCommand.Bounds.Value.min;
                            boxObject.transform.localScale = currentCommand.Bounds.Value.size;
                            var padding = 0.01f;
                            boxObject.transform.localScale += new Vector3(padding * 2, padding * 2, padding * 2);
                            boxObject.transform.position -= new Vector3(padding, padding, padding);
                        }
                    }
                }

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    boxObject.SetActive(false);

                    var coords = GetNonEmptyCoords();

                    foreach (var coord in coords) {
                        if (map.ContainsKey(coord)) {
                            continue;
                        }

                        map[coord] = new Designation(coord);
                        map[coord].AddObject(transform, wireframeMaterial);
                    }

                    currentCommand.startCoord = null;
                    currentCommand.endCoord = null;
                    currentCommand = null;
                }
            }
        }

        Dictionary<Vector3Int, Designation> map = new Dictionary<Vector3Int, Designation>();
    }
}