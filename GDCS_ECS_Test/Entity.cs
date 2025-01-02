using Godot;
using System.Collections.Generic;
using Constants_Test;

namespace ECS_Test
{
    public partial class Entity : Node
    {
        public IReadOnlyList<IComponent> Components { get => components; }
        private readonly List<IComponent> components = new();
        public int ID { get => (int)GetInstanceId(); }

        public override void _Ready()
        {
            AddToGroup(Groups.Entity);
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            foreach (var child in GetChildren())
            {
                RegisterComponent(child as IComponent);
            }
        }
        public void AddChildNode(Node node, bool legibleUniqueName = false, int @internal = 0)
        {
            if (ECS.IsComponent(node) && legibleUniqueName)
            {
                node.Name = (node as IComponent).ComponentName;
            }
            base.AddChild(node, legibleUniqueName, (Node.InternalMode)@internal);
        }
        public T GetComponent<T>(string name) where T : Node, IComponent
        {
            foreach (var component in components)
            {
                if (component is T && component.ComponentName == name)
                {
                    return (T)component;
                }
            }
            return null;
        }
        public List<T> GetComponents<T>(string name) where T : Node, IComponent
        {
            List<T> components = new();
            foreach (var component in components)
            {
                if (component is T && component.ComponentName == name)
                {
                    components.Add(component);
                }
            }
            return components;
        }
        public void RegisterComponent(IComponent component)
        {
            if (component is not null && !components.Contains(component))
            {
                components.Add(component);
            }
        }

        public void UnregisterComponent(IComponent component)
        {
            if (component is not null && components.Contains(component))
            {
                components.Remove(component);
            }
        }

        internal bool MeetRequirements(List<string> componentGroup)
        {
            int matchingRequirements = 0;
            foreach (var component in components)
            {
                if (componentGroup.Contains($"!{component.ComponentName}"))
                {
                    return false;
                }
                if (componentGroup.Contains(component.ComponentName))
                {
                    matchingRequirements++;
                }
            }
            return matchingRequirements >= componentGroup.Count;
        }
    }
}