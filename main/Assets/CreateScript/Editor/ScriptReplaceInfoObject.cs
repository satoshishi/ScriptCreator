using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ScriptCreateEditor
{
    [System.Serializable]
    public class ReplaceInfo
    {
        [System.Serializable]
        public class KeyValue
        {
            public string key;
            public string value;
        }
        public KeyValue Name;

        public TextAsset ScriptTemplate;

        public List<KeyValue> ReplaceKeyValuesPair;

        public void CopyFrom(ReplaceInfo target)
        {
            Name = new KeyValue()
            {
                key = target.Name.key,
                value = target.Name.value
            };
            
            ScriptTemplate = target.ScriptTemplate;
            ReplaceKeyValuesPair = new List<ReplaceInfo.KeyValue>();
            ReplaceKeyValuesPair.AddRange(
                target.ReplaceKeyValuesPair.Select(i => new ReplaceInfo.KeyValue()
                {
                    key = i.key,
                    value = i.value
                }));
        }
    }

    [System.Serializable]
    public class MakeScriptInfo
    {
        public ReplaceInfo replaceInfo;

        public string makePath;
    }

    [CreateAssetMenu(menuName = "ScriptCreator/ReplaceInfoObject")]
    public class ScriptReplaceInfoObject : ScriptableObject
    {
        public ReplaceInfo info;
    }
}