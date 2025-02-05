using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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
        private Queue<Dialog> dialogs;

        //UI
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI sentenceText;
        public GameObject npcImage;
        public GameObject nextButton;

        //대화 종료시 실행된 이벤트
        public Action OnCloseDialog;
        #endregion

        private void OnEnable()
        {
            dialogs = new Queue<Dialog>();
            InitDialog();
        }

        private void OnDisable()
        {
            InitDialog();
            dialogs = null;
            OnCloseDialog = null;
        }

        //초기화
        private void InitDialog()
        {
            dialogs.Clear();

            npcImage.SetActive(false);
            nameText.text = "";
            sentenceText.text = "";

            nextButton.SetActive(false);
        }

        //대화 시작하기
        public void StartDialog(int dialogIndex)
        {
            //현재 대화씬(dialogIndex) 내용을 큐에 입력
            foreach(var dialog in DataManager.GetDialogData().Dialogs.dialogs)
            {
                if(dialog.number == dialogIndex)
                {
                    dialogs.Enqueue(dialog);
                }
            }

            //첫번째 대화를 보여준다
            DrawNextDialog();
        }

        //다음 대화를 보여준다 - (큐)dialogs에서 하나 꺼내서 보여준다
        public void DrawNextDialog()
        {
            //dialogs 체크
            if(dialogs.Count == 0)
            {
                EndDialog();
                return;
            }

            //dialogs에서 하나 꺼내온다
            Dialog dialog = dialogs.Dequeue();

            /*if(dialog.character > 0)
            {
                npcImage.SetActive(true);
                npcImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(
                    "Dialog/Npc/npc0" + dialog.character.ToString());
            }
            else //dialog.character <= 0
            {
                npcImage.SetActive(false);
            }*/

            nextButton.SetActive(false);

            nameText.text = dialog.name;
            StartCoroutine(typingSentence(dialog.sentence));
        }

        //텍스트 타이핑 연출
        IEnumerator typingSentence(string typingText)
        {
            sentenceText.text = "";

            foreach (char latter in typingText)
            {
                sentenceText.text += latter;
                yield return new WaitForSeconds(0.03f);
            }

            nextButton.SetActive(true);
        }

        //대화 종료
        private void EndDialog()
        {
            //대화 종료시 이벤트 처리
            OnCloseDialog?.Invoke();            
        }
    }
}