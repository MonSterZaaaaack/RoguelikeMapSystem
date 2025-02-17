using UnityEngine;
using RoguelikeMap;
public class MapGenerationTester : MonoBehaviour
{
    public RoguelikeMap.MapConfig config;
    public void OnClick()
    {
        RoguelikeMap.MapInfoManager.instance.RequestMap(config);
    }
}
