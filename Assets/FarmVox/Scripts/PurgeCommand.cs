using System.IO;
using System.Linq;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class PurgeCommand : MonoBehaviour, ICommand
    {
        public string CommandName => "purge";

        private void Awake()
        {
            CommandManager.Instance.Add(this);
        }

        public string Run(string[] args)
        {
            var directory = new DirectoryInfo(Application.persistentDataPath);
            var files = directory.GetFiles();
            var names = files.Select(x => x.Name);

            foreach (var file in files)
            {
                file.Delete();
            }

            return $"Purged saved data: {string.Join(",", names)}";
        }
    }
}