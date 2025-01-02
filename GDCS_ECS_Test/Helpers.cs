using Godot;
using System.Reflection;
namespace ECS_Test
{
    public partial class Helpers
    {
        public static bool HasMethod(Node node, string methodName)
        {
            var type = node.GetType();
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return method != null && method.DeclaringType != typeof(Node);
        }
    }
}