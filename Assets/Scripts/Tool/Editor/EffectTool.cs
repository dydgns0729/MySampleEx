using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using System.Text;

namespace MySampleEx
{
    public class EffectTool : EditorWindow
    {
        #region Variables
        //UI에 필요한 변수
        public int uiWidthLarge = 300;
        public int uiWidthMiddle = 200;
        private Vector2 sp1 = Vector2.zero;
        private Vector2 sp2 = Vector2.zero;

        private int selection = 0;              //데이터 목록중 현재 선택된 인덱스
        private GameObject effectSorce = null;  //이펙트 소스

        private static EffectData effectData;   //이펙트 데이터
        #endregion

        [MenuItem("Tool/Effect Tool")]
        static void Init()
        {
            //데이터 가져오기
            effectData = ScriptableObject.CreateInstance<EffectData>();
            effectData.LoadData();

            //툴 창 열기
            EffectTool window = GetWindow<EffectTool>(false, "Effect Tool");
            window.Show();
        }

        private void OnGUI()
        {
            if (effectData == null)
            {
                return;
            }

            EditorGUILayout.BeginVertical();
            {
                UnityObject source = effectSorce;

                //상단 레이어(Add, Copy, Remove 버튼)
                EditorHelper.EditToolTopLayer(effectData, ref selection, ref source, uiWidthMiddle);
                effectSorce = (GameObject)source;

                //이펙트 목록, 이펙트 속성창 레이어
                EditorGUILayout.BeginHorizontal();
                {
                    //이펙트 목록
                    EditorHelper.EditorToolListLayer(effectData, ref selection, ref source,
                        uiWidthLarge, ref sp1);
                    effectSorce = (GameObject)source;

                    //이펙트 속성창
                    EditorGUILayout.BeginVertical();
                    {
                        sp2 = EditorGUILayout.BeginScrollView(sp2);
                        {
                            if (effectData.GetDataCount() > 0)
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.Separator();
                                    //속성값 설정
                                    EditorGUILayout.LabelField("ID", selection.ToString(), GUILayout.Width(uiWidthLarge));
                                    effectData.names[selection] = EditorGUILayout.TextField("이름", effectData.names[selection],
                                        GUILayout.Width(uiWidthLarge * 1.5f));
                                    effectData.effect.effectClips[selection].effectType = (EffectType)EditorGUILayout.EnumPopup("이펙트 이름",
                                        effectData.effect.effectClips[selection].effectType, GUILayout.Width(uiWidthLarge));

                                    EditorGUILayout.Separator();
                                    if(effectSorce == null && effectData.effect.effectClips[selection].effectName != string.Empty)
                                    {
                                        effectData.effect.effectClips[selection].PreLoad();
                                        effectSorce = Resources.Load(effectData.effect.effectClips[selection].effectPath +
                                            effectData.effect.effectClips[selection].effectName) as GameObject;
                                    }
                                    effectSorce = (GameObject)EditorGUILayout.ObjectField("이펙트", effectSorce, typeof(GameObject),
                                        false, GUILayout.Width(uiWidthLarge * 1.5f));
                                    if (effectSorce != null)
                                    {
                                        effectData.effect.effectClips[selection].effectPath = EditorHelper.GetPath(effectSorce);
                                        effectData.effect.effectClips[selection].effectName = effectSorce.name;
                                    }
                                    else
                                    {
                                        effectData.effect.effectClips[selection].effectPath = string.Empty;
                                        effectData.effect.effectClips[selection].effectName = string.Empty;
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            //하단 레이아웃: 로드, 저장
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("Reload Settings"))
                {
                    effectData = ScriptableObject.CreateInstance<EffectData>();
                    effectData.LoadData();
                    selection = 0;
                    effectSorce = null;
                }
                if (GUILayout.Button("Save"))
                {
                    effectData.SaveData();
                    CreateEnumStructure();
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        //이름 목록 리스트 가져와서 enum 만들기
        public void CreateEnumStructure()
        {
            string enumName = "EffectList";
            StringBuilder builer = new StringBuilder();
            builer.AppendLine();
            for (int i = 0; i < effectData.names.Count; i++)
            {
                if(effectData.names[i] != string.Empty)
                {
                    builer.AppendLine("    " + effectData.names[i] + " = " + i + ",");
                }
            }
            EditorHelper.CreateEnumStructure(enumName, builer);
        }
    }
}