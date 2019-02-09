using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public abstract class Command
    {
        public CommandType CommandType { get; protected set; }
        public DeployItem DeployItem { get; protected set; }
    }

    public class BuildWallCommand : Command
    {
        public BuildWallCommand()
        {
            CommandType = CommandType.DragAndDrop;
            DeployItem = DeployItem.Wall;
        }
    }

    public enum CommandType
    {
        Click,
        DragAndDrop
    }

    public enum DeployItem
    {
        Wall
    }
    
    public class TestDeploy : MonoBehaviour
    {
        public HighlightHoveredSurface Highlight;
        public CameraController CameraController;
        
        private Command _currentCommand;
        
        private bool _isDragging;
        private Vector3 _lastDragPosition;
        
        private void Start()
        {
            Debug.Assert(Highlight != null, "Highlight cannot be null");
            Debug.Assert(CameraController != null, "CameraController cannot be null");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _currentCommand = new BuildWallCommand();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _currentCommand = null;
            }
            
            CameraController.InputEnabled =
                _currentCommand == null || _currentCommand.CommandType != CommandType.DragAndDrop;
            
            UpdateCurrentCommand();
        }

        private void UpdateCurrentCommand()
        {
            if (_currentCommand == null)
            {
                return;
            }

            if (_currentCommand.CommandType == CommandType.DragAndDrop)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    _isDragging = true;
                    _lastDragPosition = Input.mousePosition;
                }

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    _isDragging = false;
                }

                if (_isDragging)
                {
                    var result = VoxelRaycast.TraceMouse(Input.mousePosition, 1 << UserLayers.Terrian);
                    if (result != null)
                    {
                        Debug.Log(result.GetCoord());
                    }
                }

                _lastDragPosition = Input.mousePosition;
            }
        }
    }
}