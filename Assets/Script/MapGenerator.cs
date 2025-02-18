using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Linq;
namespace RoguelikeMap
{
    public class MapGenerator : MonoBehaviour
    {
        public static MapGenerator instance { get { return _instance; } }
        private static MapGenerator _instance;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject contentPanel;
        [SerializeField] private GameObject pathNode;
        [SerializeField] private GameObject contentViewport;
        public int pathNodeLength;
        public int pathNodeWidth;
        public int pathNodeSpacing;
        private Dictionary<NodeType, GameObject> availableNodes;
        private Dictionary<NodeType, int> nodeTypeWeights;
        private int nodeCounter;
        private MapConfig curConfig;
        private int mapHeight;
        private int mapWidth;
        public Map curMap;
        private void Awake()
        {
            if(_instance != null && _instance != this)
            {
                Destroy(this);
            }
            _instance = this;
        }
        
        public Map GenerateMapData(MapConfig config)
        {
            SettingUpConfigFile(config);
            // Set Current using MapConfig;
            mapWidth = 0;
            mapHeight = 0;
            // Read Layer Data from Config;
            List<MapLayer> layers = config.layers;
            // Read Node Type Weights;
            nodeTypeWeights = new Dictionary<NodeType, int>();
            foreach(NodeTypeWeightData weight in config.nodeTypeWeights)
            {
                nodeTypeWeights[weight.type] = weight.weight;
            }
            // Reset Node Index Counter;
            nodeCounter = 0;

            // Generate Map by layers;
            Map map = new Map(config); // Map Index will be set to MapConfig Index By default, you can change it to the way you like.
            mapWidth = layers.Count * curConfig.layerWidth + 100;
            for(int i = 0; i < layers.Count;i++)
            {
                MapLayer layer = layers[i];
                List<NodeData> layerdata = null;
                // If this layer is a randomized layer.
                if (layer.randomizedLayer)
                {
                    layerdata = GenerateRandomLayer(layer,i);
                }
                // If this layer is a pre-defined layer.
                else
                {
                    layerdata = GeneratePredefindLayer(layer,i);
                }
                if(layerdata != null)
                {
                    map.mapNodes.Add(layerdata);
                }

            }

            //
            curMap = map;
            GeneratePathData();
            return map;

        }
        public void GeneratePathData()
        {
            List<List<NodeData>> nodeData = curMap.mapNodes;
            for(int i = 0; i < nodeData.Count - 1; i++)
            {
                List<NodeData> curNodeData = nodeData[i];
                if (curNodeData.Count <= nodeData[i + 1].Count)
                {
                    SettingUpConectionWithLessNodes(curNodeData, nodeData[i + 1]);
                }
                else
                {
                    SettingUpConnectionWithMoreNodes(curNodeData, nodeData[i + 1]);
                }
            }
            GeneratePathNodes();
        }
        public void GeneratePathNodes()
        {
            List<PathNodeData> pathNodes = new List<PathNodeData>();
            List<List<NodeData>> nodeData = curMap.mapNodes;
            for(int i = 0; i < nodeData.Count; i++)
            {
                List<NodeData> curNodeList = nodeData[i];
                for(int j = 0; j < curNodeList.Count; j++)
                {
                    NodeData curNode = curNodeList[j];
                    Vector2 start = curNode.position;
                    foreach(NodeData outComingNode in curNode.outgoing)
                    {
                        Vector2 end = outComingNode.position;
                        pathNodes.AddRange(CalculatePathNodePositions(start, end,pathNodeSpacing));
                    }
                }
            }
            curMap.pathNodes = pathNodes;
        }
        public List<PathNodeData> CalculatePathNodePositions(Vector2 start, Vector2 end,double maxdistance = 5)
        {
            start = new Vector2(start.x + curConfig.nodeWidth / 2, start.y);
            end = new Vector2(end.x - curConfig.nodeWidth / 2, end.y);
            List<PathNodeData> positions = new List<PathNodeData>();
            float totalDistance = Vector2.Distance(start, end);
            Vector2 direction = (end - start);
            direction = direction.normalized;
            if (totalDistance <= maxdistance)
            {
                positions.Add(new PathNodeData((start + end) / 2,direction));
                
            }
            else
            {
                int numSegments = (int)Math.Ceiling(totalDistance / maxdistance);
                float step = totalDistance / numSegments;
                direction *= step;
                for(int i = 1; i < numSegments; i++)
                {
                    positions.Add(new PathNodeData(start + i * direction,direction));
                }
            }
            return positions;


            
        }
        public void SettingUpConectionWithLessNodes(List<NodeData> curLayerList, List<NodeData> nextLayerList)
        {
            int lastConnected = 0;
            int maxWeight = 0;
            for(int i = 0; i < curLayerList.Count; i++)
            {
                NodeType curNodeType = curLayerList[i].nodeType;
                int tempWeight = nodeTypeWeights.ContainsKey(curNodeType)? nodeTypeWeights[curNodeType]: 0;
                maxWeight = Math.Max(maxWeight, tempWeight);
            }
            for(int i = 0; i < curLayerList.Count; i++)
            {
                
                NodeData curNode = curLayerList[i];
                // Setting Up Connections to Nodes in Next Layer
                int remaininngNode = nextLayerList.Count - lastConnected;
                int maxConnect = (i == curLayerList.Count - 1) ? remaininngNode : UnityEngine.Random.Range(1, remaininngNode + 1);
                int curWeight = nodeTypeWeights.ContainsKey(curNode.nodeType) ? nodeTypeWeights[curNode.nodeType] : 0;
                double weightFactor = (double) curWeight / maxWeight;
                int allocatedConnections = (i == curLayerList.Count - 1) ? remaininngNode : Math.Max(1,(int)weightFactor*maxConnect);
                allocatedConnections = Math.Min(allocatedConnections, remaininngNode);
                for(int j = 0; j < allocatedConnections; j++)
                {
                    curNode.AddOutgoing(nextLayerList[j+lastConnected]);
                    nextLayerList[j+ lastConnected].AddIncoming(curNode);
                }
                lastConnected += allocatedConnections - 1;
                // Setting Up Connections to Nodes in Second next Layer;

            }
        }
        public void SettingUpConnectionWithMoreNodes(List<NodeData> curLayerList, List<NodeData> nextLayerList)
        {
            int lastConnected = 0;
            int maxWeight = 0;
            for (int i = 0; i < nextLayerList.Count; i++)
            {
                NodeType curNodeType = nextLayerList[i].nodeType;
                int tempWeight = nodeTypeWeights.ContainsKey(curNodeType) ? nodeTypeWeights[curNodeType] : 0;
                maxWeight = Math.Max(maxWeight, tempWeight);
            }
            for (int i = 0; i < nextLayerList.Count; i++)
            {
                NodeData curNodeInNextLayer = nextLayerList[i];
                int remaininngNode = curLayerList.Count - lastConnected;
                int maxConnect = (i == nextLayerList.Count - 1) ? remaininngNode : UnityEngine.Random.Range(1, remaininngNode + 1);
                int curWeight = nodeTypeWeights.ContainsKey(curNodeInNextLayer.nodeType) ? nodeTypeWeights[curNodeInNextLayer.nodeType] : 0;
                double inverseWeightFactor =  1 - (double)curWeight / maxWeight;

                int allocatedConnections = (i == nextLayerList.Count - 1) ? remaininngNode :Math.Max(1, (int)(inverseWeightFactor * maxConnect));
                allocatedConnections = Math.Min(allocatedConnections, remaininngNode);
                for (int j = 0; j < allocatedConnections; j++)
                {
                    curNodeInNextLayer.AddIncoming(curLayerList[j + lastConnected]);
                    
                    curLayerList[j + lastConnected].AddOutgoing(curNodeInNextLayer);
                }
                lastConnected += allocatedConnections - 1;
            }
        }
        public GameObject PopulateMap(Map mapdata)
        {
            GameObject curPanel = Instantiate(contentPanel, contentViewport.transform);

            RectTransform panelTransform = curPanel.GetComponent<RectTransform>();
            CalculateMapSize(mapdata);
            panelTransform.sizeDelta = new Vector2(mapWidth, mapHeight);
            panelTransform.anchorMax = new Vector2(0f, 0.5f);
            panelTransform.anchorMin = new Vector2(0f, 0.5f);
            panelTransform.pivot = new Vector2(0f, 0.5f);
            scrollRect.content = panelTransform;
            PopulateNodes(mapdata,curPanel);
            PopulatePaths(mapdata,curPanel);
            MapPopulateComplete();
            return curPanel;
        }
        public void CalculateMapSize(Map mapdata)
        {
            mapWidth =mapdata.mapNodes.Count * curConfig.layerWidth + 100;
            int maxNodeSize = mapdata.mapNodes.Max(nodeList => nodeList.Count);
            mapHeight = maxNodeSize * curConfig.nodeWidth + (maxNodeSize - 1) * curConfig.nodeSpacing + 100;
        }
        public void UnlockFirstLayerNodes(Map curMap)
        {
            foreach(NodeData node in curMap.mapNodes[0])
            {
                node.NodeUnlocked();
            }
        }
        /// <summary>
        /// Reset Everything When Map generation completes;
        /// </summary>
        private void MapPopulateComplete()
        {
            curConfig = null;
            curMap = null;
        }
        public void PopulateNodes(Map curMap, GameObject curPanel)
        {
            if(curMap != null)
            {
                for(int i = 0; i < curMap.mapNodes.Count; i++)
                {
                    List<NodeData> curNodeList = curMap.mapNodes[i];
                    for(int j = 0; j < curNodeList.Count; j++)
                    {
                        NodeData curNodeData = curNodeList[j];
                        GameObject nodeTemplate = availableNodes[curNodeData.nodeType];
                        GameObject curNode = Instantiate(nodeTemplate, curPanel.transform);
                        MapNode mapNode = curNode.GetComponent<MapNode>();
                        if(mapNode != null)
                        {
                            RectTransform nodeTransform = curNode.GetComponent<RectTransform>();
                            nodeTransform.sizeDelta = new Vector2(curConfig.nodeWidth, curConfig.nodeWidth);
                            nodeTransform.anchorMax = new Vector2(0.5f, 0.5f);
                            nodeTransform.anchorMin = new Vector2(0.5f, 0.5f);
                            nodeTransform.pivot = new Vector2(0.5f, 0.5f);
                            nodeTransform.anchoredPosition = curNodeData.position;
                            mapNode.SetNodeData(curNodeData);
                            mapNode.InitializeMapNode();
                            MapInfoManager.instance.curMapNode[mapNode.NodeData] = mapNode;
                            switch (curNodeData.nodeState)
                            {
                                case NodeState.Locked:
                                    mapNode.OnLocked();
                                    break;
                                case NodeState.Visited:
                                    mapNode.SetVisited();
                                    break;
                                case NodeState.Attandable:
                                    mapNode.OnUnlocked();
                                    break;
                                default:
                                    mapNode.OnLocked();
                                    break;

                            }

                        }


                    }
                }
            }
          
        }
        public void PopulatePaths(Map curMap, GameObject curPanel)
        {
            for (int j = 0; j < curMap.pathNodes.Count; j++)
            {
                GameObject curPathNode = Instantiate(pathNode, curPanel.transform);
                RectTransform nodeTransform = curPathNode.GetComponent<RectTransform>();
                nodeTransform.sizeDelta = new Vector2(pathNodeLength, pathNodeWidth);
                nodeTransform.anchorMax = new Vector2(0.5f, 0.5f);
                nodeTransform.anchorMin = new Vector2(0.5f, 0.5f);
                nodeTransform.pivot = new Vector2(0.5f, 0.5f);
                nodeTransform.anchoredPosition = curMap.pathNodes[j].pathNodePosition;
                nodeTransform.up = -curMap.pathNodes[j].pathNodeDirection;

            }
        }
        private List<NodeData> GenerateRandomLayer(MapLayer layer,int curLayerCount)
        {
            List<NodeData> layerData = new List<NodeData>();
            // Get Current Layer Node Size
            int nodeSize = UnityEngine.Random.Range(layer.minNodeSize, layer.maxNodeSize+1);
            // Calculate Current Layer Height Based On Node Size
            int curLayerHeight = nodeSize * curConfig.nodeWidth + (nodeSize - 1) * curConfig.nodeSpacing + 100;
            mapHeight = mapHeight > curLayerHeight ? mapHeight : curLayerHeight;
            // Get Weighted Node Type List;
            List<NodeTypeWeightData> weightedTypes = layer.weightedPotentialNodeTypes;
            // Get Node Position List;
            List<Vector2> nodePosition = GenerateNodePositons(curLayerCount, nodeSize);
            // Populate Node data 
            for(int i = 0; i < nodeSize; i++)
            {
                NodeData curNodeData = GenerateNodes(weightedTypes, nodePosition[i],curLayerCount);
                layerData.Add(curNodeData);
            }
            return layerData;
        }
        private NodeType GetWeightedRandomNodeType(List<NodeTypeWeightData> typeList)
        {
            int totalWeight = 0;
            foreach(NodeTypeWeightData type in typeList)
            {
                // Weight must be a positive Int;
                totalWeight += type.weight > 0 ? type.weight : 0;
            }
            int randomWeight = UnityEngine.Random.Range(0, totalWeight);
            int cumulativeWeight = 0;
            for(int i = 0; i < typeList.Count; i++)
            {
                cumulativeWeight += typeList[i].weight;
                if(randomWeight < cumulativeWeight)
                {
                    return typeList[i].type;
                }
            }
            return NodeType.Invalid;
        }
        private List<NodeData> GeneratePredefindLayer(MapLayer layer,int curLayerCount)
        {
            List<NodeData> layerData = new List<NodeData>();
            // Get Current Layer Node Size
            int nodeSize = layer.layerNodes.Count;
            // Calculate Current Layer Height Based On Node Size
            int curLayerHeight = nodeSize * curConfig.nodeWidth + (nodeSize - 1) * curConfig.nodeSpacing + 100;
            mapHeight = mapHeight > curLayerHeight ? mapHeight : curLayerHeight;
            // Get Node Position List;
            List<Vector2> nodePosition = GenerateNodePositons(curLayerCount, nodeSize);
            // Populate Node data 
            for (int i = 0; i < nodeSize; i++)
            {
                NodeData curNodeData = null;
                switch (layer.layerNodes[i].nodeType)
                {
                    case NodeType.Minions:
                        {
                            MinionPredefinedNodeData data = (MinionPredefinedNodeData)layer.layerNodes[i];
                            curNodeData = new MinionNodeData(NodeType.Minions, nodeCounter++, nodePosition[i], NodeState.Locked, new List<int>(), new List<int>(), curLayerCount, data.LevelName,new List<string>());
                            break;
                        }

                    case NodeType.Elites:
                        {
                            ElitePredefinedNodeData data = (ElitePredefinedNodeData)layer.layerNodes[i];
                            curNodeData = new EliteNodeData(NodeType.Elites, nodeCounter++, nodePosition[i], NodeState.Locked, new List<int>(), new List<int>(), curLayerCount, data.LevelName,new List<string>());
                            break;
                        }

                }
                if(curNodeData != null)
                {
                    layerData.Add(curNodeData);
                }
                
            }
            return layerData;
        }
        private NodeData GenerateNodes(List<NodeTypeWeightData> availableTypes,Vector2 position,int curLayerCount)
        {
            NodeType curNodeType = GetWeightedRandomNodeType(availableTypes);
            int curNodeIndex = nodeCounter++;
            NodeData curNodeData= new NodeData(curNodeType, curNodeIndex, new Vector2(position.x, position.y), NodeState.Locked, new List<int>(), new List<int>(), curLayerCount);

            switch (curNodeType)
            {
                case NodeType.Minions:
                    curNodeData = new MinionNodeData(curNodeData);
                    break;
                case NodeType.Elites:
                    curNodeData = new EliteNodeData(curNodeData);
                    break;
                default:
                    
                    break;
            }
            curNodeData.SetUpNodeData();
            return curNodeData;

        }
        private List<Vector2> GenerateNodePositons(int layerCount,int nodeSize)
        {
            int layerWidth = curConfig.layerWidth;
            // Node Width have to be smaller or equal to Layer Width;
            int nodeWidth = curConfig.nodeWidth > layerWidth ? layerWidth : curConfig.nodeWidth;
            int offsetLimit = (layerWidth - nodeWidth) / 2;
            // Central Axis Position, for Left to Right Orientation, this is the X position for each Node of Current Layer.
            // For Top Down Orientation, this is  the Y position for each Node of Current Layer.
            int totalWidth = curConfig.layerWidth * curConfig.layers.Count;
            int centralPosition = -(totalWidth - curConfig.layerWidth) / 2 + layerCount * curConfig.layerWidth ;
            // For Left to Right Orientation, Fixed Positon are the Y position for each Node;
            // For Top Down Orientation, this is the X position for each Node.
            // Nodes will be evenly distributed on the two sides of the Layer from the Mid Point.
            int[] fixedPosition = new int[nodeSize];
            if(nodeSize % 2 == 1)
            {
                int midPosition = (nodeSize - 1) / 2;
                fixedPosition[midPosition] = 0;
                for(int i = 1; i <= midPosition; i++)
                {
                    fixedPosition[midPosition + i] = -(nodeWidth + curConfig.nodeSpacing) * i;
                    fixedPosition[midPosition - i] = (nodeWidth + curConfig.nodeSpacing) * i;
                }
            }
            else
            {
                int midPosition = nodeSize / 2 - 1;
                for(int i = midPosition; i >= 0; i--)
                {
                    fixedPosition[i] = (midPosition - i) * (nodeWidth + curConfig.nodeSpacing) + (nodeWidth + curConfig.nodeSpacing)/2;
                    fixedPosition[nodeSize - 1 - i] = fixedPosition[i] * -1;
                }
            }
            // Fill Node Position List Using Calculated Positons; 
            List<Vector2> Res = new List<Vector2>();
            for(int i = 0; i < fixedPosition.Length; i++)
            {

                int positionOffset = UnityEngine.Random.Range(-curConfig.nodeOffset, curConfig.nodeOffset);
                if (positionOffset > 0)
                {
                    positionOffset = positionOffset > offsetLimit ? offsetLimit : positionOffset;
                }
                else
                {
                    positionOffset = positionOffset < -offsetLimit ? -offsetLimit : positionOffset;
                }
                Res.Add(new Vector2((centralPosition + positionOffset), fixedPosition[i]));
            }
            return Res;

        }
        public Map PopulateMapTest(MapConfig config)
        {
            Map testMap = GenerateMapData(config);
            PopulateMap(testMap);
            return testMap;
        }
        public void SettingUpConfigFile(MapConfig config)
        {
            curConfig = config;
            // Read available Nodes templates;
            availableNodes = new Dictionary<NodeType, GameObject>();
            foreach (NodeTemplateData template in config.mapNodeTemplates)
            {
                availableNodes[template.type] = template.template;
            }

        }
        public void ResetNodeConnections(Map map)
        {
            Dictionary<int, NodeData> mapNodes = new Dictionary<int, NodeData>();
            for(int i = 0; i < map.mapNodes.Count; i++)
            {
                List<NodeData> curLayerData = map.mapNodes[i];
                for(int j = 0; j < curLayerData.Count; j++)
                {
                    mapNodes[curLayerData[j].nodeIndex] = curLayerData[j];
                }
            }
            for (int i = 0; i < map.mapNodes.Count; i++)
            {
                List<NodeData> curLayerData = map.mapNodes[i];
                for (int j = 0; j < curLayerData.Count; j++)
                {
                    NodeData curNode = curLayerData[j];
                    curNode.incoming.Clear();
                    foreach(int nodeIndex in curNode.incomingNodeIndex)
                    {
                        curNode.incoming.Add(mapNodes[nodeIndex]);
                    }
                    curNode.outgoing.Clear();
                    foreach (int nodeIndex in curNode.outgoingNodeIndex)
                    {
                        curNode.outgoing.Add(mapNodes[nodeIndex]);
                    }
                }
            }

        }
    }
    [Serializable]
    public class PathNodeData
    {
        public Vector2 pathNodePosition;
        public Vector2 pathNodeDirection;
        [JsonConstructor]
        public PathNodeData(Vector2 pathNodePosition,Vector2 pathNodeDirection)
        {
            this.pathNodePosition = pathNodePosition;
            this.pathNodeDirection = pathNodeDirection;
        }
    }

}

