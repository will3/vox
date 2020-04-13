using UnityEngine;

namespace FarmVox.Scripts
{
    public class TestCommand : MonoBehaviour, ICommand
    {
        public string Name => "test";
        public string output = "test output";

        public string Run(string[] args)
        {
            return output;
        }
    }
}