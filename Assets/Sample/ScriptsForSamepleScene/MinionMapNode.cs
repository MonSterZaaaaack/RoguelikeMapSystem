using System.Collections.Generic;
using UnityEngine;
using RoguelikeMap;
public class MinionMapNode : MapNode
{
    public override void InitializeNodeData()
    {
    }

    public override void OnCleared()
    {

        MinionNodeData data = (MinionNodeData)_nodeData;
        Debug.Log(_nodeData.nodeType.ToString() + " Node : " + data.GetLevelName() + " Cleared;");
        List<NodeData> curLayerData = MapInfoManager.instance.curMap.mapNodes[_nodeData.layerCount];
        foreach(NodeData nodeData in curLayerData)
        {
            MapNode mapNodeData = MapInfoManager.instance.curMapNode[nodeData];
            mapNodeData.OnLocked();
        }
        SetVisited();
        List<NodeData> outgoing = _nodeData.outgoing;
        foreach (NodeData nodeData in outgoing)
        {
            MapNode mapNodeData = MapInfoManager.instance.curMapNode[nodeData];
            mapNodeData.OnUnlocked();
        }
    }
    public override void SetVisited()
    {
        if (nodeImage == null)
        {
            Debug.Log("Node " + gameObject.name + " Image Component Missing!");
        }
        nodeImage.sprite = clearedImage;
        //nodeButton.interactable = false;
        _nodeData.NodeVisited();
    }
    public override void OnClick()
    {
        if (_nodeData.nodeState == NodeState.Attandable)
        {
            OnCleared();
        }
        MinionNodeData nodeData = (MinionNodeData)_nodeData;
        string info = "MinionNode: " + nodeData.GetLevelName();
        List<string> ids = nodeData.GetMinionIds();
        for (int i = 0; i < ids.Count; i++)
        {
            string id = " " + ids[i];
            info += id;
        }
        Debug.Log(info);

    }

    public override void OnLocked()
    {
        if (nodeImage == null)
        {
            Debug.Log("Node " + gameObject.name + " Image Component Missing!");
        }
        nodeImage.sprite = lockedImage;
        //nodeButton.interactable = false;
        _nodeData.NodeLocked();
    }

    public override void OnUnlocked()
    {

        if (nodeImage == null)
        {
            Debug.Log("Node " + gameObject.name + " Image Component Missing!");
        }
        nodeImage.sprite = attendableImage;
        nodeButton.interactable = true;
        _nodeData.NodeUnlocked();
    }
    private void OnDestroy()
    {
    }
}
