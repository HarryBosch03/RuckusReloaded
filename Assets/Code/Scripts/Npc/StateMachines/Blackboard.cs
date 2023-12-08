using System.Collections.Generic;
using System.Text;

namespace RuckusReloaded.Runtime.Npc.StateMachines
{
    public class Blackboard
    {
        private Dictionary<string, object> data = new();

        public void Set(string key, object element)
        {
            if (data.ContainsKey(key)) data[key] = element;
            else data.Add(key, element);
        }

        public object Get(string key, object fallback = null)
        {
            return data.ContainsKey(key) ? data[key] : fallback;
        }

        public T Get<T>(string key, T fallback = default) => (T)Get(key, (object)fallback);

        public bool HasKey(string key) => data.ContainsKey(key);
        public void Delete(string key) => data.Remove(key);

        public string Dump()
        {
            var sb = new StringBuilder();

            sb.AppendLine("--- Blackboard Dump ---");
            foreach (var e in data)
            {
                sb.AppendLine($"- [{e.Key}, {e.Value}]");
            }
            sb.AppendLine("---     End Dump    ---");
            
            return sb.ToString();
        }
    }
}