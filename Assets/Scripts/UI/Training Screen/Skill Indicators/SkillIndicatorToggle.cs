using FitnessForKids.Data;
using UnityEngine.UI;
using UnityEngine;

namespace FitnessForKids.UI
{
    public class SkillIndicatorToggle : MonoBehaviour, ISkillIndicator
    {
        [SerializeField] private Skill _skill;
        [SerializeField] private Toggle _toggle;
        public Skill Skill => _skill;
        private IndicatorState _state = IndicatorState.Inactive;
        public IndicatorState State
        {
            get { return _state; }
            set
            {
                _state = value;
                UpdateState();
            }
        }

        private void UpdateState()
        {
            var isActive = _state == IndicatorState.Active;
            _toggle.isOn = isActive;
        }
    }
}