using System.Collections.Generic;
using FitnessForKids.Data;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace FitnessForKids.UI
{
    public interface ISkillIndicator
    {
        public Skill Skill { get; }
        public IndicatorState State { get; set; }
    }

    public class SkillIndicator : MonoBehaviour, ISkillIndicator
    {
        [SerializeField] private TMP_Text textLabel;
        [SerializeField] private Image icon;
        [SerializeField] private List<Sprite> stateImages;
        [SerializeField] private List<Color> stateColors;
        [SerializeField] private Skill skill;
        public Skill Skill => skill;
        private IndicatorState state = IndicatorState.Inactive;
        public IndicatorState State
        {
            get { return state; }
            set
            {
                state = value;
                UpdateState();
            }
        }

        private void UpdateState()
        {
            if (state == IndicatorState.Inactive)
            {
                icon.sprite = stateImages[0];
                textLabel.color = stateColors[0];
            }
            else
            {
                icon.sprite = stateImages[1];
                textLabel.color = stateColors[1];
            }
        }
    }
}