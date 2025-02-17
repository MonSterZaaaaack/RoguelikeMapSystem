using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
namespace RoguelikeMap
{
    public class MapInfoManager : MonoBehaviour
    {
        public static MapInfoManager instance { get { return _instance; } }
        private static MapInfoManager _instance;
        public Dictionary<int, Map> mapData;
        public Dictionary<NodeData, MapNode> curMapNode;
        public string mapDataSavePath;
        public GameObject curPanel;
        public Map curMap;
        public void RequestMap(MapConfig config)
        {
            GameObject oldPanel = curPanel;
            Destroy(oldPanel);
            curMapNode = new Dictionary<NodeData, MapNode>();
            if (mapData.ContainsKey(config.mapConfigIndex))
            {
                curMap = mapData[config.mapConfigIndex];
                MapGenerator.instance.SettingUpConfigFile(config);
                MapGenerator.instance.ResetNodeConnections(curMap);
                curPanel = MapGenerator.instance.PopulateMap(curMap);

            }
            else
            {
                curMap = MapGenerator.instance.GenerateMapData(config);
                MapGenerator.instance.UnlockFirstLayerNodes(curMap);
                mapData[config.mapConfigIndex] = curMap;

                curPanel = MapGenerator.instance.PopulateMap(curMap);
            }
            //curMap = mapData[config.mapConfigIndex];

        }
        private void Awake()
        {
            if(_instance != null && _instance != this)
            {
                Destroy(this);
            }
            _instance = this;
            curPanel = null;
            curMap = null;
            LoadMapInfo();

        }
        public void SaveMapInfo()
        {
            var settings = new JsonSerializerSettings { 
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string saveData = JsonConvert.SerializeObject(mapData,settings);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(Application.persistentDataPath, mapDataSavePath));
            bf.Serialize(file, saveData);
            file.Close();
            Debug.Log("Saved To : " + Application.persistentDataPath + mapDataSavePath);
        }
        public void LoadMapInfo()
        {
            if (File.Exists(string.Concat(Application.persistentDataPath, mapDataSavePath)))
            {
                Debug.Log("Load From : " + Application.persistentDataPath + mapDataSavePath);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(string.Concat(Application.persistentDataPath, mapDataSavePath), FileMode.Open);
                var settings = new JsonSerializerSettings { 
                    TypeNameHandling = TypeNameHandling.Auto
                };
                mapData = JsonConvert.DeserializeObject< Dictionary<int, Map> >(bf.Deserialize(file).ToString(),settings);
                file.Close();
            }
            else
            {
                mapData = new Dictionary<int, Map>();
            }
        }

    }

}

