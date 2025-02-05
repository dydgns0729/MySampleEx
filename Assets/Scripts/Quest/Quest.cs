using UnityEngine;
using System;
using System.Collections.Generic;

namespace MySampleEx
{
    /// <summary>
    /// 퀘스트 타입
    /// </summary>
    public enum QuestType
    {
        None = -1,
        Kill,
        Collect,
    }

    /// <summary>
    /// 퀘스트 상태
    /// </summary>
    public enum QuestState
    {
        None = -1,
        Ready,          //퀘스트 수락 이전 상태(진행 이전)
        Accept,         //퀘스트 수락한 상태 (진행중)
        Complete,       //퀘스트 목표 완료한 상태 (아직 보상 안받은 상태)
        Rewarded,       //퀘스트 보상 받은 상태
    }

    /// <summary>
    /// 퀘스트 데이터 리스트 클래스
    /// </summary>
    [Serializable]
    public class Quests
    {
        public List<Quest> quests { get; set; }
    }

    /// <summary>
    /// 퀘스트 데이터 클래스
    /// </summary>
    [Serializable]
    public class Quest
    {
        public int number { get; set; }         //퀘스트 인덱스
        public int npcNumber { get; set; }      //퀘스트를 가지고 있는 NPC
        public string name { get; set; }        //퀘스트 이름
        public string description { get; set; } //퀘스트 내용
        public int dialogIndex { get; set; }    //퀘스트 대화내용-의뢰,진행중,완료
        public int level { get; set; }          //퀘스트 레벨 제한
        public QuestType questType { get; set; }    //퀘스트 타입
        public int goalIndex { get; set; }          //퀘스트 목표 아이템 타입, enemy타입
        public int goalAmount { get; set; }         //퀘스트 목표 수량
        public int rewardGold { get; set; }         //보상 골드
        public int rewardExp { get; set; }          //보상 경험치
        public int rewardItem { get; set; }         //보상 아이템 아이디
    }



}