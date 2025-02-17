using UnityEngine;
using RoguelikeMap;
[CreateAssetMenu]
public class MinionPredefinedNodeData : RoguelikeMap.PredefinedNodeData
{
    public string LevelName;
    public MinionPredefinedNodeData()
    {
        nodeType = NodeType.Minions;
        LevelName = "";
    }
}
