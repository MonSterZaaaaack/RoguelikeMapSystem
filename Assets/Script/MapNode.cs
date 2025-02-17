using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RoguelikeMap
{
    /// <summary>
    /// Base Class for all Map Nodes
    /// 
    /// </summary>
    public abstract class MapNode : MonoBehaviour
    {
        public Image nodeImage;
        public Button nodeButton;
        public Sprite lockedImage;
        public Sprite attendableImage;
        public Sprite clearedImage;
        public NodeData NodeData { get { return _nodeData; } }
        [SerializeReference] protected NodeData _nodeData;
        public abstract void SetVisited();
        /// <summary>
        /// Default behaviour when nodes become cleared;
        /// </summary>
        public virtual void OnCleared()
        {

        }
        /// <summary>
        /// Default behaviour when nodes become locked;
        /// </summary>
        public abstract void OnLocked();
        /// <summary>
        /// Default behaviour when nodes become attendable;
        /// </summary>
        public abstract void OnUnlocked();
        /// <summary>
        /// Default behaviour when nodes are clicked;
        /// </summary>
        public abstract void OnClick();
        /// <summary>
        /// Initialize Nodedata based on specific MapNode Type;
        /// </summary>
        public abstract void InitializeNodeData();
        /// <summary>
        /// Initialize MapNode data;
        /// </summary>
        public virtual void InitializeMapNode()
        {
            InitializeNodeData();

        }
        public void SetNodeData(NodeData data)
        {
            _nodeData = data;
        }
        private void OnDestroy()
        {
            Debug.Log(gameObject.name + " Has Been Destoried");
        }

    }
    [Serializable]
    public enum NodeState
    {
        Locked = 0,
        Visited = 1,
        Attandable,
    }
}

