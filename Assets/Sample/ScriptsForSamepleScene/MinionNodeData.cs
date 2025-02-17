using UnityEngine;
using RoguelikeMap;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
public class MinionNodeData : NodeData
{
    private string minionName;
    [JsonConstructor]
    public MinionNodeData(NodeType nodeType, int nodeIndex, Vector2 point, NodeState nodeState, List<int> incoming, List<int> outgoing, int layerCount, string levelName) : base(nodeType, nodeIndex, point, nodeState, incoming, outgoing, layerCount)
    {
        this.minionName = "MinionLevel: " + levelName;
    }
    /// <summary>
    /// Copy constructor From Base Data
    /// </summary>
    /// <param name="data"></param>
    public MinionNodeData(NodeData data) : base(data.nodeType, data.nodeIndex, data.position, data.nodeState, data.incomingNodeIndex, data.outgoingNodeIndex, data.layerCount)
    {
        this.minionName = "MinionLevel : None";
    }
    public void SetLevelName(string levelName)
    {
        minionName = "MinionLevel: " + levelName;
    }
    public override void SetUpNodeData()
    {
        SetLevelName(nodeIndex.ToString());
        base.SetUpNodeData();

    }
    public string GetLevelName()
    {
        return minionName;
    }
}
