using UnityEngine;

namespace FarmVox.Scripts
{
    public class TestCommand : MonoBehaviour, ICommand
    {
        public string Name => "test";

        public void Run(string[] args)
        {
            Debug.Log("test command");
        }
    }
}