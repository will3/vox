using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class FileStorage
    {
        public void Save(string key, object obj)
        {
            var path = Path.Combine(Application.persistentDataPath, key);
            var json = JsonUtility.ToJson(obj);
            File.WriteAllText(path, json);
        }

        public T Load<T>(string key)
        {
            var path = Path.Combine(Application.persistentDataPath, key);

            try
            {
                var json = File.ReadAllText(path);

                return JsonUtility.FromJson<T>(json);
            }
            catch (DirectoryNotFoundException)
            {
                return default;
            }
            catch (FileNotFoundException)
            {
                return default;
            }
        }

        public void Remove(string key)
        {
            var path = Path.Combine(Application.persistentDataPath, key);
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