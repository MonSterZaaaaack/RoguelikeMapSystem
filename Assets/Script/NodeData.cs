using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Xml.Serialization;
namespace RoguelikeMap
{
    /// <summary>
    /// Node data stores all constant(will not change in Game once initialized)
    /// information about a Node itself, including its Nodetype, Node position, incoming and oncoming Node lists.
    /// </summary>
    [JsonObject(ItemTypeNameHandling = TypeNameHandling.Auto)]
    [Serializable]
    public class NodeData
    {
        public Vector2 position;
        [JsonIgnore]
        public readonly List<NodeData> incoming = new List<NodeData>();
        [JsonIgnore]
        public readonly List<NodeData> outgoing = new List<NodeData>();
        public readonly List<int> incomingNodeIndex;
        public readonly List<int> outgoingNodeIndex;
        [JsonConverter(typeof(StringEnumConverter))]
        public readonly NodeType nodeType;
        [Tooltip("This Index is Unique Per Map")]
        public int nodeIndex;
        [JsonConverter(typeof(StringEnumConverter))]
        public NodeState nodeState;
        public readonly int layerCount;
        public NodeData()
        {
            position = Vector2.zero;
            incoming = new List<NodeData>();
            outgoing = new List<NodeData>();
            nodeType = NodeType.Invalid;
            nodeState = 0;
            nodeIndex = -1;
            layerCount = -1;
        }
        [JsonConstructor]
        public NodeData(NodeType nodeType, int nodeIndex, Vector2 position, NodeState nodeState, List<int> incomingNodeIndex, List<int> outgoingNodeIndex, int layerCount)
        {
            this.nodeType = nodeType;
            this.nodeIndex = nodeIndex;
            this.position = position;
            this.nodeState = nodeState;
            this.incoming = new List<NodeData>();
            this.outgoing = new List<NodeData>();
            this.incomingNodeIndex = new List<int>();
            this.outgoingNodeIndex = new List<int>();
            if(incomingNodeIndex != null)
            {
                foreach(int index in incomingNodeIndex)
                {
                    this.incomingNodeIndex.Add(index);
                }
            }
            if (outgoingNodeIndex != null)
            {
                foreach (int index in outgoingNodeIndex)
                {
                    this.outgoingNodeIndex.Add(index);
                }
            }
            this.layerCount = layerCount;
        }
        public virtual void SetUpNodeData()
        {

        }
        public virtual void NodeLocked()
        {
            nodeState = NodeState.Locked;
        }
        public virtual void NodeUnlocked()
        {
            nodeState = NodeState.Attandable;
        }
        public virtual void NodeVisited()
        {
            nodeState = NodeState.Visited;
        }
        public virtual void NodeCleared()
        {
            nodeState = NodeState.Visited;
        }
        public void AddIncoming(NodeData p)
        {
            if (incoming.Any(element => element.Equals(p)))
                return;
            incomingNodeIndex.Add(p.nodeIndex);
            incoming.Add(p);
        }

        public void AddOutgoing(NodeData p)
        {
            if (outgoing.Any(element => element.Equals(p)))
                return;
            outgoingNodeIndex.Add(p.nodeIndex);
            outgoing.Add(p);
        }

        public void RemoveIncoming(NodeData p)
        {
            incoming.RemoveAll(element => element.Equals(p));
            incomingNodeIndex.RemoveAll(element => element == p.nodeIndex);
        }

        public void RemoveOutgoing(NodeData p)
        {
            outgoing.RemoveAll(element => element.Equals(p));
            outgoingNodeIndex.RemoveAll(element => element == p.nodeIndex);
        }

        public bool HasNoConnections()
        {
            return incoming.Count == 0 && outgoing.Count == 0;
        }

    }
    [Serializable]
    public enum NodeType
    {
        Invalid = -1,
        Minions = 0,
        Elites = 1,
        Boss,
        Shop,
        Restort,
        Event,

    }
}
