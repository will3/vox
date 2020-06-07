using System;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class TestCommand : MonoBehaviour, ICommand
    {
        public string CommandName => "test";
        public string output = "test output";

        private void Awake()
        {
            CommandManager.Instance.Add(this);
        }

        public string Run(string[] args)
        {
            return output;
        }
    }
}