using FitnessForKids.Services;
using System;
using System.Globalization;
using UnityEngine;

namespace FitnessForKids.UI.Subscription
{
    public interface IFreeTrialTracker
    {
        bool IsFreeNow();
        int GetFreeDaysLeft();
    }

    public class AndroidSubscriptionFreeTrial : IFreeTrialTracker
    {
        private const string kTrialFreeKey = "FreeTrial";
        private const string kDateFormat = "yyyy-MM-dd HH:mm:ss";
        private const int kFreeDays = 10;

        private readonly IDataService _dataService;

        public AndroidSubscriptionFreeTrial(IDataService dataService)
        {
            _dataService = dataService;
        }

        public bool IsFreeNow()
        {
            var daysLeft = GetFreeDaysLeft();
            var result = daysLeft == -1
                ? false
                : true;

            return result;
        }

        public int GetFreeDaysLeft()
        {
            var expireDate = DateTime.UtcNow.AddDays(kFreeDays);
            expireDate = _dataService.UserProfileController.GetFreeTrialEndDateOrDefault(expireDate);
            var currentDate = DateTime.UtcNow;

            var difference = expireDate - currentDate;
            var daysLeft = difference.Days;
            var isTimeOut = difference.TotalMilliseconds < 0;
            var result = isTimeOut
                ? -1
                : daysLeft;

            return result;
        }
    }
}