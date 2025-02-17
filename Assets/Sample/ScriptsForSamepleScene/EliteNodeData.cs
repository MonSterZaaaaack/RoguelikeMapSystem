using UnityEngine;
using RoguelikeMap;
using Newtonsoft.Json;
using System.Collections.Generic;
public class EliteNodeData : NodeData
{
    private string eliteName;
    [JsonConstructor]
    public EliteNodeData(NodeType nodeType, int nodeIndex, Vector2 point, NodeState nodeState, List<int> incoming, List<int> outgoing, int layerCount,string levelName) : base(nodeType, nodeIndex, point, nodeState, incoming, outgoing, layerCount)
    {
        this.eliteName = "EliteLevel: " + levelName;
    }
    /// <summary>
    /// Copy constructor From Base Data
    /// </summary>
    /// <param name="data"></param>
    public EliteNodeData(NodeData data) : base(data.nodeType, data.nodeIndex, data.position, data.nodeState, data.incomingNodeIndex, data.outgoingNodeIndex, data.layerCount)
    {
        this.eliteName = "EliteLevel : None";
    }
    public void SetLevelName(string levelName)
    {
        eliteName = "EliteLevel: " + levelName;
    }
    public override void SetUpNodeData()
    {
        base.SetUpNodeData();
        SetLevelName(nodeIndex.ToString());
    }
    public string GetLevelName()
    {
        return eliteName;
    }
}
