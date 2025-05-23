using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

namespace galaxy890624
{
    /// <summary>
    /// 所有的技能列表
    /// </summary>
    public class SkillManager : MonoBehaviour
    {
        public static SkillManager Instance; // 宣告單例模式 Singleton pattern
        [Header("點擊到的技能按鈕")]
        public SkillData ActivateSkill; // Mouse Information

        [Header("UI")]
        public Image SkillImage;
        public TextMeshProUGUI SkillNameText; // 技能名稱文字
        public TextMeshProUGUI SkillInfoText; // 技能資訊文字

        public SkillButton[] SkillButtons;

        [Header("PlayerData")]
        public PlayerData Data;
        public GameObject Player;

        [Header("GetAward 必須按照寶石等級排序")]
        public GetAward[] GetAwardDatas;

        [Header("升級成功的音效")]
        public AudioSource UpgradeSuccessfulSound;

        [Header("升級失敗的音效")]
        public AudioSource UpgradeFailedSound;

        /// <summary>
        /// Design pattern - 單例模式 Singleton
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(gameObject);
                }
            }
            DontDestroyOnLoad(gameObject);
        }
        /*
        public void UpgradeButton()
        {
            // 如果什麼按鈕都不點,就直接按下升級按鈕,會回傳錯誤訊息
            if (ActivateSkill == null)
            {
                print("<color=#ff00ff>未選擇任何技能,無法升級!</color>");
                // 播放 升級失敗 的 音效
                UpgradeFailedSound.Play();
                return;
            }

            // 確保玩家等級和經驗滿足基本需求
            if (Data.Level < ActivateSkill.RequireLevel || Data.Exp < ActivateSkill.CostExp)
            {
                print("<color=#ff00ff>等級或經驗不足,無法升級技能!</color>");
                // 播放 升級失敗 的 音效
                UpgradeFailedSound.Play();
                return;
            }

            // 檢查所有前置技能是否都已解鎖
            foreach (SkillData preSkill in ActivateSkill.PreSkills)
            {
                if (!preSkill.IsUnlocked)
                {
                    print($"<color=#ff00ff>前置技能 <color=#00ff00>{preSkill.SkillName}</color> 未解鎖，無法升級技能!</color>");
                    // 播放 升級失敗 的 音效
                    UpgradeFailedSound.Play();
                    return;
                }
            }

            // 若所有條件都符合，執行升級
            UpgradeSkill();
        }
        private void UpgradeSkill()
        {
            // 撥放 升級成功 的 音效
            UpgradeSuccessfulSound.Play();

            // 如果 SkillData.SkillName 找到 "速度" 
            if (ActivateSkill.SkillName.Contains("速度"))
            {
                print($"<color=#ff00ff>我找到<color=#00ff00>{ActivateSkill.SkillName}</color>了!</color>");
                Player.GetComponent<ThirdPersonController>().MoveSpeed = ActivateSkill.SkillValue;
            }
            if (ActivateSkill.SkillName.Contains("生命"))
            {
                print($"<color=#ff00ff>我找到<color=#00ff00>{ActivateSkill.SkillName}</color>了!</color>");
                Data.MaxHP = ActivateSkill.SkillValue;
            }
            if (ActivateSkill.SkillName.Contains("綠寶石"))
            {
                print($"<color=#ff00ff>我找到<color=#00ff00>{ActivateSkill.SkillName}</color>了!</color>");
                GetAwardDatas[0].GetExp = ActivateSkill.SkillValue;
            }
            if (ActivateSkill.SkillName.Contains("藍寶石"))
            {
                print($"<color=#ff00ff>我找到<color=#00ff00>{ActivateSkill.SkillName}</color>了!</color>");
                GetAwardDatas[1].GetExp = ActivateSkill.SkillValue;
            }
            if (ActivateSkill.SkillName.Contains("紫寶石"))
            {
                print($"<color=#ff00ff>我找到<color=#00ff00>{ActivateSkill.SkillName}</color>了!</color>");
                GetAwardDatas[2].GetExp = ActivateSkill.SkillValue;
            }
            // 把顏色改回亮的顏色(白色)
            SkillButtons[ActivateSkill.SkillIndex].GetComponent<UnityEngine.UI.Image>().color = Color.white;
            // 扣除 對應的經驗
            Data.Exp -= ActivateSkill.CostExp;
            // 更新技能資訊
            DisplaySkillInfo();
            // 完成解鎖
            ActivateSkill.IsUnlocked = true;
        }
        */
        public void DisplaySkillInfo()
        {
            // SkillImage.sprite = ActivateSkill.skillSprite;
            SkillNameText.text = ActivateSkill.SkillName;
            SkillInfoText.text = ActivateSkill.SkillInfo;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}