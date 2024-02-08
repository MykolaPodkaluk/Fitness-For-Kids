using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FitnessForKids.UI
{
    public class SkillIndicatorsView : SkillIndicatorsViewBase
    {
        [SerializeField] List<SkillIndicator> indicators;
        protected override List<ISkillIndicator> _skillIndicators => indicators.Cast<ISkillIndicator>().ToList();
    }
}