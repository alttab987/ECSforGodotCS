using Godot;
using Constants_Test;

namespace ECS_Test
{
    public partial class ECS : Node
    {
        public static bool IsSystem(Node node)
        {
            if (node is IGameSystem gameSystem)
            {
                return gameSystem.Requirements.Count > 0;
            }
            return false;
        }
        public static bool IsComponent(Node node)
        {
            return node is IComponent;
        }
        public static bool IsEntity(Node node)
        {
            return node.IsInGroup(Groups.Entity);
        }
    }
} 