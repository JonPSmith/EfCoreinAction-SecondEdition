// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace BookApp.BackgroundTasks
{
    public class NightlyTimer
    {
        private const int HourToTriggerOn = 1;

        /// <summary>
        /// We want the CheckFixCacheValuesService to run at 1am in the GMT Standard Time timezone
        /// </summary>
        /// <returns></returns>
        public TimeSpan TimeToWait()
        {
            var tzi = TimeZoneInfo.Utc;  //Put your timezone here
            var nowAsEdtTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzi);

            return TimeTo1AmEveryDay(nowAsEdtTime);
        }

        private TimeSpan TimeTo1AmEveryDay(DateTime nowAsEdtTime)
        {
            if (nowAsEdtTime.Hour < HourToTriggerOn)
                //The time is before the time we want it to run, so set the time to the trigger time
                return nowAsEdtTime.TimeOfDay.Subtract(TimeSpan.FromHours(HourToTriggerOn));

            //else it needs to run tomorrow at HourToTriggerOn
            return new TimeSpan(1, 0, 0, 0).Subtract(nowAsEdtTime.TimeOfDay)
                .Add(TimeSpan.FromHours(HourToTriggerOn));
        }

    }
}