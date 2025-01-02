using Godot;
using System.Collections.Generic;

namespace ECS_Test
{
    public partial class GameSystem : Node, IGameSystem
    {
        public virtual string SystemName { get; set; }
        public virtual List<string> Requirements { get; set; }
        protected GameSystemManager SystemManager {get; private set;}
        public bool System_Init(GameSystemManager systemManager) 
        {
            SystemManager = systemManager;
            return true;
        }
        public virtual void System_Ready()
        {

        }
        public virtual void System_Process(List<int> entities, double delta) 
        {

        }
        public virtual void System_Physics_Process(List<int> entities, double delta) 
        {

        }
    }
} 