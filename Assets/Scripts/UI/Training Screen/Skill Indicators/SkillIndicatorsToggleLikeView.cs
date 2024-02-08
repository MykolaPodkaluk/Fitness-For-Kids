using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FitnessForKids.UI
{
    public class SkillIndicatorsToggleLikeView : SkillIndicatorsViewBase
    {
        [SerializeField] List<SkillIndicatorToggle> indicators;
        protected override List<ISkillIndicator> _skillIndicators => indicators.Cast<ISkillIndicator>().ToList();
    }
}