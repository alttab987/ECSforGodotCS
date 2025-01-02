using Godot;
using System.Collections.Generic;

namespace ECS_Test
{
    public interface IGameSystem
    {
        public string SystemName { get; set; }
        public List<string> Requirements { get; set; }
        
    }
} 