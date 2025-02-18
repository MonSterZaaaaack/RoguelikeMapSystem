using UnityEngine;
using RoguelikeMap;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
public class MinionNodeData : NodeData
{
    [JsonProperty]
    private string minionName;
    [JsonProperty]
    private List<string> minionIds;
    [JsonConstructor]
    public MinionNodeData(NodeType nodeType, int nodeIndex, Vector2 point, NodeState nodeState, List<int> incoming, List<int> outgoing, int layerCount, string levelName, List<string> minionIds) : base(nodeType, nodeIndex, point, nodeState, incoming, outgoing, layerCount)
    {
        this.minionName = "MinionLevel: " + levelName;
        this.minionIds = minionIds;
    }
    /// <summary>
    /// Copy constructor From Base Data
    /// </summary>
    /// <param name="data"></param>
    public MinionNodeData(NodeData data) : base(data.nodeType, data.nodeIndex, data.position, data.nodeState, data.incomingNodeIndex, data.outgoingNodeIndex, data.layerCount)
    {
        this.minionName = "MinionLevel : None";
        minionIds = new List<string>();
    }
    public void SetLevelName(string levelName)
    {
        minionName = "MinionLevel: " + levelName;
    }
    public void SetMinionIds(List<string> ids)
    {
        minionIds = ids;
    }
    public List<string> GetMinionIds()
    {
        return minionIds;
    }
    public override void SetUpNodeData()
    {
        SetLevelName(nodeIndex.ToString());
        base.SetUpNodeData();
        for (int i = 0; i < 5; i++)
        {
            minionIds.Add("MinionMapId: " + UnityEngine.Random.Range(0, 10));
        }

    }
    public string GetLevelName()
    {
        return minionName;
    }
}
