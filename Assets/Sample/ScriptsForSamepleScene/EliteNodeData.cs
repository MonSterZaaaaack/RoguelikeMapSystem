using UnityEngine;
using RoguelikeMap;
using Newtonsoft.Json;
using System.Collections.Generic;
public class EliteNodeData : NodeData
{
    [JsonProperty]
    private string eliteName;
    [JsonProperty]
    private List<string> idList;
    [JsonConstructor]
    public EliteNodeData(NodeType nodeType, int nodeIndex, Vector2 point, NodeState nodeState, List<int> incoming, List<int> outgoing, int layerCount,string levelName, List<string> idList) : base(nodeType, nodeIndex, point, nodeState, incoming, outgoing, layerCount)
    {
        this.eliteName = "EliteLevel: " + levelName;
        this.idList = idList;
    }
    /// <summary>
    /// Copy constructor From Base Data
    /// </summary>
    /// <param name="data"></param>
    public EliteNodeData(NodeData data) : base(data.nodeType, data.nodeIndex, data.position, data.nodeState, data.incomingNodeIndex, data.outgoingNodeIndex, data.layerCount)
    {
        this.eliteName = "EliteLevel : None";
        this.idList = new List<string>();
    }
    public void SetLevelName(string levelName)
    {
        eliteName = "EliteLevel: " + levelName;
    }
    public void SetIdList(List<string> idList)
    {
        this.idList = idList;
    }
    public List<string> GetIdList()
    {
        return idList;
    }
    public override void SetUpNodeData()
    {
        base.SetUpNodeData();
        SetLevelName(nodeIndex.ToString());
        for(int i = 0; i < 5; i++)
        {
            idList.Add("EliteMapId: " + UnityEngine.Random.Range(0, 10));
        }
    }
    public string GetLevelName()
    {
        return eliteName;
    }
}
