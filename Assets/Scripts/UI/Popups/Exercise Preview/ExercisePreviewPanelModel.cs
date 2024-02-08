
namespace FitnessForKids.UI
{
    public class ExercisePreviewPanelModel : IModel
    {
        private string _exerciseId;
        public string Name => GetName();
        public string Description => GetDescription();
        public const string kNamesTableKey = "Sports Exercises Names";
        public const string kDescriptionsTableKey = "Sports Exercises Descriptions";
        private const string kNameFormat = "<color={1}>{0}</color>";
        private const string kNameColor = "#FE7925";

        public ExercisePreviewPanelModel(string exerciseId)
        {
            _exerciseId = exerciseId;
        }

        private string GetName()
        {
            string localizedName = LocalizationController.GetLocalizedString(kNamesTableKey, _exerciseId);
            string name = string.Format(kNameFormat, localizedName, kNameColor);
            return name;
        }

        private string GetDescription()
        {
            string fullDescription = LocalizationController.GetLocalizedString(kDescriptionsTableKey, _exerciseId);
            return fullDescription;
        }
    }
}