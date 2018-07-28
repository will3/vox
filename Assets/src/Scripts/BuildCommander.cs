using UnityEngine;

namespace FarmVox
{
    public class BuildCommander : MonoBehaviour
    {
        enum CommandType {
            None,
            Dig
        }

        class Command {
            public readonly CommandType type;
            public Vector3Int? startCoord;
            public Vector3Int? endCoord;
            public Command(CommandType type) {
                this.type = type;
            }
        }

        Command command;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.D)) {
                command = new Command(CommandType.Dig);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                command = null;
            }

            if (command.type == CommandType.Dig) {
                if (command.startCoord == null && Input.GetKeyDown(KeyCode.Mouse0)) {
                    var result = VoxelRaycast.TraceMouse();
                    if (result != null)
                    {
                        if (command.startCoord == null) {
                            command.startCoord = result.GetCoord();
                        } else  {
                            command.endCoord = result.GetCoord();    
                        }
                    }
                }
            }
        }
    }
}