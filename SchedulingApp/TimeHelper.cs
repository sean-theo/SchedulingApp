using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingApp
{
    public static class TimeHelper
    {
        //Convert from UTC for display
        public static DateTime ConvertFromUTCtoLocal(DateTime appointmentTime)
        {
            if (appointmentTime.Kind == DateTimeKind.Unspecified)
            {
                appointmentTime = DateTime.SpecifyKind(appointmentTime, DateTimeKind.Utc);
            }
            return TimeZoneInfo.ConvertTimeFromUtc(appointmentTime, TimeZoneInfo.Local);
        }

       //Convert from local time to UTC for SQL
        public static DateTime ConvertFromLocalToUTC(DateTime appointmentTime)
        {
            if (appointmentTime.Kind == DateTimeKind.Unspecified)
            {
                appointmentTime = DateTime.SpecifyKind(appointmentTime, DateTimeKind.Local);
            }
            return TimeZoneInfo.ConvertTimeToUtc(appointmentTime);
        }

        
        public static string FormatLocal(DateTime utcTime)
        {
            return ConvertFromLocalToUTC(utcTime).ToString("g");
        }
    }
}
