using FarmVox.Scripts;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Commands
{
    public class Commander : MonoBehaviour
    {
        public HighlightHoveredSurface Highlight;
        public CameraController CameraController;
        
        private CommandBase _currentCommandBase;
        
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
                _currentCommandBase = new BuildWallCommandBase();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _currentCommandBase = null;
            }
            
            CameraController.InputEnabled =
                _currentCommandBase == null || _currentCommandBase.CommandType != CommandType.DragAndDrop;
            
            UpdateCurrentCommand();
        }

        private void UpdateCurrentCommand()
        {
            if (_currentCommandBase == null)
            {
                return;
            }

            if (_currentCommandBase.CommandType == CommandType.DragAndDrop)
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