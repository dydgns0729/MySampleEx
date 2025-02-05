using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace MySampleEx
{
    public class DialogData : ScriptableObject
    {
        #region Variables
        public Dialogs Dialogs;         //대화 다이알 로그 데이터베이스

        private string dataPath = "Data/DialogData";
        #endregion

        //생성자
        public DialogData() { }

        //데이터 읽기
        public void LoadData()
        {
            TextAsset asset = (TextAsset)ResourcesManager.Load(dataPath);
            if (asset == null || asset.text == null)
            {
                return;
            }

            using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
            {
                var xs = new XmlSerializer(typeof(Dialogs));
                Dialogs = (Dialogs)xs.Deserialize(reader);
            }
        }
    }
}