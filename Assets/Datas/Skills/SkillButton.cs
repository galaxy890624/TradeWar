using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace galaxy890624
{
    /// <summary>
    /// 技能按鈕
    /// </summary>
    public class SkillButton : MonoBehaviour, IPointerClickHandler
    {
        public SkillData SkillData;
        public void OnPointerClick(PointerEventData eventData)
        {
            SkillManager.Instance.ActivateSkill = SkillData; // 調用
            SkillManager.Instance.DisplaySkillInfo();
        }
    }
}
