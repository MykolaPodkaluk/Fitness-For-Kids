using System.Collections.Generic;
using FitnessForKids.Data;
using System.Linq;
using UnityEngine;
using System;

namespace FitnessForKids.UI
{
    public class BodyPartsPreview : MonoBehaviour
    {
        [SerializeField] private List<BodyPartsGender> bodyParts;
        [SerializeField] private List<GameObject> baseFront;
        [SerializeField] private List<GameObject> baseBack;
        private List<BodyPartView> frontSideParts;
        private List<BodyPartView> backSideParts;

        public void Init(Gender gender, bool isEnabled = true)
        {
            var temp = (int)gender - 1;
            baseFront[temp].SetActive(isEnabled);
            baseBack[temp].SetActive(isEnabled);
            frontSideParts = bodyParts[temp].frontSideParts;
            backSideParts = bodyParts[temp].backSideParts; 
        }

        public void ShowActiveParts(BodyPart activeBodyPart)
        {
            bool isFullBody = activeBodyPart == BodyPart.FullBody;

            foreach (var partView in frontSideParts.Concat(backSideParts))
            {
                bool isActive = isFullBody || partView.BodyPart == activeBodyPart;
                partView.IsActive = isActive;
            }
        }
    }

    [Serializable]
    public class BodyPartsGender
    {
        [SerializeField] public List<BodyPartView> frontSideParts;
        [SerializeField] public List<BodyPartView> backSideParts;
    }
}