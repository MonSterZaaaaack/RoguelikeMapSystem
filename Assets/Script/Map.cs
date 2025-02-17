using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace RoguelikeMap
{
    [Serializable]
    public class Map
    {
        public List<List<NodeData>> mapNodes;
        public List<PathNodeData> pathNodes;
        public string mapName;
        public int mapIndex;
        public Map()
        {
            mapNodes = new List<List<NodeData>>();
            pathNodes = new List<PathNodeData>();
            mapName = "";
            mapIndex = 0;
        }
        public Map(MapConfig config) : this()
        {
            mapIndex = config.mapConfigIndex;

        }
        [JsonConstructor]
        public Map(List<List<NodeData>> mapNodes, List<PathNodeData> pathNodes, string mapName, int mapIndex)
        {
            this.mapNodes = mapNodes;
            this.pathNodes = pathNodes;
            this.mapName = mapName;
            this.mapIndex = mapIndex;
        }
    }
}


