using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;
using System;
using System.Reflection;
using System.Linq;

namespace ScriptCreateEditor
{
    public class ScriptCreater : EditorWindow
    {
        private List<MakeScriptInfo> MakeInfos = new List<MakeScriptInfo>();
        private Vector2 ScrollPosition = Vector2.zero;
        private ScriptReplaceInfoObject ScriptableObject;

        [MenuItem("Tools/CreateScript")]
        private static void Create()
        {
            GetWindow<ScriptCreater>("Create Script");
        }

        void OnGUI()
        {
            SetScriptableObject();
            SetLastScriptInfo();
                       
            DrawScriptInfos();

            MakeScripts();
        }

        private void SetScriptableObject()
        {
            ScriptableObject = EditorGUILayout.ObjectField("ScriptableObject", ScriptableObject, typeof(ScriptReplaceInfoObject), true) as ScriptReplaceInfoObject;
            if (GUILayout.Button("↑のScriptableObjectから追加"))
            {
                if (ScriptableObject != null)
                {
                    var newReplace = new ReplaceInfo();
                    newReplace.CopyFrom(ScriptableObject.info);
                    MakeInfos.Add(new MakeScriptInfo()
                    {
                        replaceInfo = newReplace,
                        makePath = ""
                    });
                }
            }
        }

        private void SetLastScriptInfo()
        {
            if (GUILayout.Button("一番最後の設定値から追加"))
            {
                if (MakeInfos != null && MakeInfos.Count > 0)
                {
                    var newReplace = new ReplaceInfo();
                    var targetInfo = MakeInfos.Last();
                    newReplace.CopyFrom(targetInfo.replaceInfo);

                    MakeInfos.Add(new MakeScriptInfo()
                    {
                        replaceInfo = newReplace,
                        makePath = targetInfo.makePath
                    });
                }
            }
        }

        private void DrawScriptInfos()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("生成するスクリプト");

            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);

            for (int i = 0; i < MakeInfos.Count; i++)
            {
                DrawScriptInfo(MakeInfos[i], i);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawScriptInfo(MakeScriptInfo info, int index)
        {
            if (GUILayout.Button("↓この設定を消す"))
                MakeInfos.RemoveAt(index);

            info.replaceInfo.ScriptTemplate = EditorGUILayout.ObjectField("スクリプトのテンプレート(TextAsset)", info.replaceInfo.ScriptTemplate, typeof(TextAsset), true) as TextAsset;

            EditorGUILayout.BeginHorizontal();

            info.makePath = EditorGUILayout.TextField("スクリプトのpath", info.makePath);
            if (GUILayout.Button("pathを選択する"))
                info.makePath = EditorUtility.OpenFolderPanel("Choice Path", Application.dataPath, string.Empty);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            info.replaceInfo.Name.key = EditorGUILayout.TextField("名前(置換対象)", info.replaceInfo.Name.key);
            info.replaceInfo.Name.value = EditorGUILayout.TextField("名前(置換後)", info.replaceInfo.Name.value);

            EditorGUILayout.Space();

            for (int i = 0; i < info.replaceInfo.ReplaceKeyValuesPair.Count; i++)
            {
                EditorGUILayout.LabelField($"{i + 1}");
                info.replaceInfo.ReplaceKeyValuesPair[i].key = EditorGUILayout.TextField("置換対象とする文字列", info.replaceInfo.ReplaceKeyValuesPair[i].key);
                info.replaceInfo.ReplaceKeyValuesPair[i].value = EditorGUILayout.TextField("置換後の文字列", info.replaceInfo.ReplaceKeyValuesPair[i].value);
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            }
        }

        private void MakeScripts()
        {
            if (GUILayout.Button("↑のScriptを作成する"))
            {
                foreach (MakeScriptInfo info in MakeInfos)
                {
                    if (string.IsNullOrEmpty(info.makePath) || string.IsNullOrEmpty(info.replaceInfo.Name.key) || string.IsNullOrEmpty(info.replaceInfo.Name.value))
                        continue;

                    MakeScript(info);
                }
            }
        }

        private void MakeScript(MakeScriptInfo info)
        {
            var filePath = info.makePath + "/" + info.replaceInfo.Name.value + ".cs";
            var script = info.replaceInfo.ScriptTemplate.text.Replace(@$"{info.replaceInfo.Name.key}", @$"{info.replaceInfo.Name.value}");

            foreach (ReplaceInfo.KeyValue keyvalue in info.replaceInfo.ReplaceKeyValuesPair)
                script = script.Replace(@$"{keyvalue.key}", @$"{keyvalue.value}");

            File.WriteAllText(filePath, script);


            AssetDatabase.Refresh();
        }
    }
}