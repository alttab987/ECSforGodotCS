using Godot;
using System.Collections.Generic;
using Constants_Test;
namespace ECS_Test
{
    public partial class GameSystemManager : Node
    {
        //
        private readonly Dictionary<List<string>, List<int>> requirementsEntitiesDict = new();
        public IReadOnlyList<GameSystem> Systems { get => systems; }
        private List<GameSystem> systems = new();
        public override void _Ready()
        {
            var tree = GetTree();
            tree.NodeAdded += OnNodeAdded;
            tree.NodeRemoved += OnNodeRemoved;
            TreeExited += OnTreeExited;
            systems = GetSystems();
            foreach (var system in systems)
            {
                if (ECS.IsSystem(system))
                {
                    RegisterRequirements(system.Requirements);
                }
            }
            foreach (var entity in GetTree().GetNodesInGroup(Groups.Entity))
            {
                UpdateComponentGroups(entity as Entity);
            }
            foreach (var system in systems)
            {
                if (ECS.IsSystem(system) && Helpers.HasMethod(system, "System_Ready"))
                {
                    var gameSystem = system;
                    gameSystem.System_Ready();
                }
            }
        }
        private List<GameSystem> GetSystems()
        {
            List<GameSystem> systems = new();
            foreach (var child in GetChildren())
            {
                if (ECS.IsSystem(child))
                {
                    if (Engine.IsEditorHint())
                    {
                        systems.Add(child as GameSystem);
                    }
                    else if (ValidateSystem(child as GameSystem))
                    {
                        systems.Add(child as GameSystem);
                    }
                }
            }
            return systems;
        }
        private bool ValidateSystem(GameSystem system)
        {
            if (!Helpers.HasMethod(system, "System_Init"))
            {
                GD.PushError($"System {system.SystemName} does not have a System_Init method");
                return false;
            }
            if (!system.System_Init(this))
            {
                GD.PushError($"System {system.SystemName} failed to initialize");
                return false;
            }
            if (system.Requirements.Count == 0)
            {
                GD.PushError($"System {system.SystemName} attempted to register with no requirements. Skipping.");
                return false;
            }
            return true;
        }
        private void RegisterRequirements(List<string> systemRequirements)
        {
            systemRequirements.Sort();
            if (!requirementsEntitiesDict.ContainsKey(systemRequirements))
            {
                var entities = new List<int>();
                requirementsEntitiesDict[systemRequirements] = entities;
            }
        }
        private void OnNodeAdded(Node node)
        {
            if (ECS.IsComponent(node))
            {
                var entity = node.GetParent<Entity>();
                entity.RegisterComponent(node as IComponent);
                UpdateComponentGroups(entity);
            }
            else if (ECS.IsEntity(node))
            {
                UpdateComponentGroups(node as Entity);
            }
        }
        private void OnNodeRemoved(Node node)
        {
            if (ECS.IsComponent(node))
            {
                var entity = node.GetParent<Entity>();
                entity.UnregisterComponent(node as IComponent);
                UpdateComponentGroups(entity);
            }
            else if (ECS.IsEntity(node))
            {
                UpdateComponentGroups(node as Entity);
            }
        }
        private void OnTreeExited()
        {
            var tree = GetTree();
            tree.NodeAdded -= OnNodeAdded;
            tree.NodeRemoved -= OnNodeRemoved;
        }
        private void UpdateComponentGroups(Entity entity)
        {
            foreach (var componentGroup in requirementsEntitiesDict.Keys)
            {
                var entityList = requirementsEntitiesDict[componentGroup];
                if (entity.MeetRequirements(componentGroup))
                {
                    if (!entityList.Contains(entity.ID))
                    {
                        entityList.Add(entity.ID);
                    }
                }
                else
                {
                    entityList.Remove(entity.ID);
                }
            }
        }
        public override void _Process(double delta)
        {
            foreach (var system in systems)
            {
                if (ECS.IsSystem(system) && Helpers.HasMethod(system, "System_Process"))
                {
                    var gameSystem = system;
                    gameSystem.System_Process(QueryEntities(gameSystem), delta);
                }
            }
        }
        public override void _PhysicsProcess(double delta)
        {
            foreach (var system in systems)
            {
                if (ECS.IsSystem(system) && Helpers.HasMethod(system, "System_Physics_Process"))
                {
                    var gameSystem = system;
                    gameSystem.System_Physics_Process(QueryEntities(gameSystem), delta);
                }
            }
        }
        private List<int> QueryEntities(GameSystem gameSystem)
        {
            return requirementsEntitiesDict[gameSystem.Requirements];
        }
    }
}
