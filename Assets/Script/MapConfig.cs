using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeMap
{
    [CreateAssetMenu]
    public class MapConfig : ScriptableObject
    {
        /// <summary>
        /// All Type of Nodes templates will be used for this level;
        /// </summary>
        public List<NodeTemplateData> mapNodeTemplates;
        [Tooltip("Node type weight list, used for setting up node connections.")]
        public List<NodeTypeWeightData> nodeTypeWeights;
        /// <summary>
        /// Distinct index for this mapConfig;
        /// </summary>
        [Tooltip("The Index for this Config File, this Config will be used by MapGenerator to check whether the map should be generated as new or already exist in MapSave File")]
        public int mapConfigIndex;
        [Tooltip("The Width of each nodes in pixel (Nodes are default to have Square shape so Width = Height")]
        [Min(1)]
        public int nodeWidth;
        [Tooltip("The space between nodes in the same Layer, depending on the orientation settings.")]
        [Min(1)]
        public int nodeSpacing;
        [Tooltip("The Width of each layer in pixels, depending on the orientation settings.")]
        [Min(1)]
        public int layerWidth;
        [Tooltip("The position offset range for each Node.")]
        [Min(0)]
        public int nodeOffset;
        [Tooltip("Layer info for this map.")]
        public List<MapLayer> layers;


    }
    [Serializable]
    public class NodeTemplateData
    {
        public NodeType type;
        public GameObject template;
    }
    [Serializable]
    public class NodeTypeWeightData
    {
        public NodeType type;
        public int weight;
    }
}


