using System.Collections.Generic;
using FitnessForKids.Data;
using UnityEngine;

namespace FitnessForKids.UI
{
    public abstract class SkillIndicatorsViewBase : MonoBehaviour
    {
        protected abstract List<ISkillIndicator> _skillIndicators { get; }
        public virtual void UpdateIndicators(List<Skill> activeSkills)
        {
            foreach (ISkillIndicator indicator in _skillIndicators)
            {
                var isActive = activeSkills.Contains(indicator.Skill);
                indicator.State = isActive ? IndicatorState.Active : IndicatorState.Inactive;
            }
        }
    }
}