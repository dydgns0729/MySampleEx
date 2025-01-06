using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using System.Text;
using System.IO;

namespace MySampleEx
{
    /// <summary>
    /// 툴과 관련된 공통 기능 구현
    /// 경로 얻어오기, 이름 목록 리스트를 enum 으로 만들기
    /// </summary>
    public class EditorHelper
    {
        //매개변수로 받은 UnityObject의 위치 경로 얻어오기
        public static string GetPath(UnityObject p_clip)
        {
            string retString = string.Empty;

            //p_clip 클립의 전체 경로 : Assets/ResourcesData/Resources/EffectData/
            retString = AssetDatabase.GetAssetPath(p_clip);
            string[] path_node = retString.Split('/');
            bool findResources = false;
            for (int i = 0; i < path_node.Length; i++)
            {
                if (!findResources)
                {
                    if (path_node[i] == "Resources")
                    {
                        findResources = true;
                        retString = string.Empty;
                    }
                }
                else
                {
                    retString += path_node[i] + "/";
                }
            }
            return retString;
        }

        //이름 목록 리스트를 enum 으로 만들기
        public static void CreateEnumStructure(string enumName, StringBuilder data)
        {
            string templateFilePath = "Assets/Editor/EnumTemplate.txt";
            string entittyTemplate = File.ReadAllText(templateFilePath);

            entittyTemplate = entittyTemplate.Replace("$ENUM$", enumName);
            entittyTemplate = entittyTemplate.Replace("$DATA$", data.ToString());

            string folderPath = "Assets/Scripts/GameData/";
            if (!Directory.Exists(folderPath))
            {
                Debug.LogWarning($"{folderPath}을 찾을 수 없습니다");
                Directory.CreateDirectory(folderPath);
                Debug.Log("해당 경로에 폴더를 만들었습니다");
            }
            string filePath = folderPath + enumName + ".cs";
            //해당파일이 존재하면 삭제한다
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.WriteAllText(filePath, entittyTemplate);
        }
    }
}