using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FarmVox
{
    public class FileStorage
    {
        public void Save(string dir, string key, object obj)
        {
            var dirPath = Path.Combine(Application.persistentDataPath, dir);
            Directory.CreateDirectory(dirPath);
            var path = Path.Combine(dirPath, key);
            var json = JsonUtility.ToJson(obj);
            File.WriteAllText(path, json);
        }

        public T Load<T>(string dir, string key)
        {
            var dirPath = Path.Combine(Application.persistentDataPath, dir);
            var path = Path.Combine(dirPath, key);
            var json = File.ReadAllText(path);

            return JsonUtility.FromJson<T>(json);
        }

        public void Remove(string dir, string key)
        {
            var dirPath = Path.Combine(Application.persistentDataPath, dir);
            var path = Path.Combine(dirPath, key);
            File.Delete(path);
        }

        public IEnumerable<string> List(string dir)
        {
            var dirPath = new DirectoryInfo(Path.Combine(Application.persistentDataPath, dir));
            var files = dirPath.GetFiles("*.*");
            return files.Select(u => u.Name);
        }
    }
}