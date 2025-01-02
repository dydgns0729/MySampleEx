using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.UI;
using TMPro;
using System;

namespace MySampleEx
{
    /// <summary>
    /// 대화창 구현 클래스
    /// 대화 데이터 파일 읽기
    /// 대화 데이터 UI 적용
    /// </summary>
    public class DialogUI : MonoBehaviour
    {
        #region Variables
        //XML 
        public string xmlFile = "Dialog/Dialog";    //Path (.../Resources/ 가 생략되어있음)
        private XmlNodeList allNodes;

        private Queue<Dialog> dialogs;

        //UI
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI sentenceText;
        public GameObject npcImage;
        public GameObject nextButton;
        #endregion

        private void Start()
        {
            
            //xml 데이터 파일 읽기
            LoadDialogXml(xmlFile);

            dialogs = new Queue<Dialog>();
            InitDialog();

            StartDialog(1);
        }

        //Xml 데이터 읽기
        private void LoadDialogXml(string path)
        {
            TextAsset xmlFile = Resources.Load<TextAsset>(path);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlFile.text);
            allNodes = xmlDoc.SelectNodes("root/Dialog");
        }

        //초기화
        private void InitDialog()
        {
            dialogs.Clear();

            npcImage.SetActive(false);

            nameText.text = "";
            sentenceText.text = "";

            nextButton.SetActive(false);
            StopAllCoroutines();
        }


        //대화 시작하기
        public void StartDialog(int dialogIndex)
        {
            InitDialog();
            //현재 대화씬 내용을 큐에 저장
            foreach (XmlNode node in allNodes)
            {
                int num = int.Parse(node["number"].InnerText);
                if (num == dialogIndex)
                {
                    Dialog dialog = new Dialog();
                    dialog.number = num;
                    dialog.character = int.Parse(node["character"].InnerText);
                    dialog.name = node["name"].InnerText;
                    dialog.sentence = node["sentence"].InnerText;

                    dialogs.Enqueue(dialog);
                }
            }

            //첫번째 대화를 보여준다
            DrawNextDialog();
        }

        //다음 대화를 보여준다 - (큐)dialogs에서 하나 꺼내서 보여준다.
        public void DrawNextDialog()
        {
            if (dialogs.Count == 0)
            {
                EndDialog();
                return;
            }
            Dialog dialog = dialogs.Dequeue();

            if (dialog.character > 0)
            {
                npcImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Dialog/Npc/npc0" + dialog.character.ToString());
                npcImage.SetActive(true);

            }
            else //NpcImg 제거
            {
                npcImage.SetActive(false);
            }

            nextButton.SetActive(false);

            nameText.text = dialog.name;
            //sentenceText.text = dialog.sentence;
            StartCoroutine(typingSentence(dialog.sentence));
        }

        //텍스트 타이핑 연출
        IEnumerator typingSentence(string typingText)
        {
            sentenceText.text = "";
            foreach(char latter in typingText)
            {
                sentenceText.text += latter;
                yield return new WaitForSeconds(0.05f);
            }
            nextButton.SetActive(true);
        }

        // Dialog 종료
        private void EndDialog()
        {
            InitDialog();

            //대화 종료시 이벤트 처리
            //...
        }
    }
}