using UnityEngine;
using RoguelikeMap;
[CreateAssetMenu]
public class ElitePredefinedNodeData : RoguelikeMap.PredefinedNodeData
{
    public string LevelName;
    public ElitePredefinedNodeData()
    {
        nodeType = NodeType.Elites;
        LevelName = "";
    }
}
