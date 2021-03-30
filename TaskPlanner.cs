using System;
using System.Collections.Generic;

namespace TaskPlanner
{
    public class TaskPlanner
    {
        private List<DateTime> holidays = new List<DateTime>();
        private List<DateTime> recurringHolidays = new List<DateTime>();
        private TimeSpan startTime;
        private TimeSpan stopTime;

        public void SetWorkdayStartAndStop(TimeSpan workdayStartTime, TimeSpan workdayStopTime)
        {
            startTime = workdayStartTime;
            stopTime = workdayStopTime;
        }
        public void SetHoliday(DateTime date)
        {
            holidays.Add(date);
        }

        public void SetRecurringHoliday(DateTime date)
        {
            recurringHolidays.Add(date);
        }


        public DateTime GetTaskFinishingDate(DateTime start, double days)
        {
            DateTime calculatedDate = new DateTime();
            double decimalPartOfday = days % 1;
            calculatedDate = start;

            calculatedDate = CountTime(calculatedDate, decimalPartOfday, days);
            calculatedDate = CountDays(calculatedDate, days);

            return calculatedDate;
        }

        private bool CheckHolidays(DateTime calculatedDate)
        {
            foreach (DateTime holiday in holidays)
            {
                if (DateTime.Equals(calculatedDate.Date, holiday.Date))
                {
                    return true;
                }

            }
            foreach (DateTime recurringHoliday in recurringHolidays)
            {
                if (recurringHoliday.Month == calculatedDate.Month && recurringHoliday.Day == calculatedDate.Day)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckHolidaysAndWeekDays(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday || CheckHolidays(date))
                return true;
            else
                return false;
        }

        private DateTime CountTime(DateTime calculatedDate, double decimalPartOfday, double estimatedDays)
        {
            TimeSpan WorkingDuration = stopTime - startTime;
            TimeSpan timeDuration;
            TimeSpan wholeTime = TimeSpan.Zero;

            timeDuration = WorkingDuration * decimalPartOfday;

            if (estimatedDays > 0)
            {
                if (CheckHolidays(calculatedDate))
                {
                    wholeTime = startTime + timeDuration;
                    calculatedDate = calculatedDate.AddDays(1);
                }
                else if (calculatedDate.TimeOfDay > startTime && calculatedDate.TimeOfDay < stopTime)
                {
                    wholeTime = calculatedDate.TimeOfDay + timeDuration;
                }
                else if (calculatedDate.TimeOfDay <= startTime)
                {
                    wholeTime = stopTime + timeDuration;
                    calculatedDate = calculatedDate.AddDays(-1);
                }
                else if (calculatedDate.TimeOfDay > stopTime)
                {
                    calculatedDate = calculatedDate.AddDays(1);
                    wholeTime = startTime + timeDuration;
                }
                else if (calculatedDate.TimeOfDay == stopTime)
                {
                    wholeTime = stopTime + timeDuration;
                }
            }
            else
            {
                if (CheckHolidays(calculatedDate))
                {
                    wholeTime = stopTime + timeDuration;
                }
                else if (calculatedDate.TimeOfDay > startTime && calculatedDate.TimeOfDay < stopTime)
                {
                    wholeTime = calculatedDate.TimeOfDay + timeDuration;
                }
                else if (calculatedDate.TimeOfDay <= startTime && decimalPartOfday != 0)
                {
                    wholeTime = stopTime + timeDuration;
                    calculatedDate = calculatedDate.AddDays(-1);
                }
                else if (calculatedDate.TimeOfDay == startTime && decimalPartOfday == 0)
                {
                    wholeTime = startTime + timeDuration;
                }
                else if (calculatedDate.TimeOfDay > stopTime)
                {
                    wholeTime = stopTime + timeDuration;
                }
                else if (calculatedDate.TimeOfDay == stopTime && decimalPartOfday == 0)
                {
                    wholeTime = startTime - timeDuration;
                    calculatedDate = calculatedDate.AddDays(1);
                }
            }

            return SplitTime(calculatedDate, wholeTime, estimatedDays, decimalPartOfday);
        }

        private DateTime SplitTime(DateTime calculatedDate, TimeSpan wholeTime, double estimatedDays, double decimalPartOfday)
        {
            TimeSpan endTime;
            if (estimatedDays >= 0)
            {
                if (wholeTime < startTime)
                {
                    TimeSpan splitTime = startTime - wholeTime;
                    endTime = stopTime - splitTime;
                }
                else if (wholeTime > stopTime)
                {
                    TimeSpan splitTime = wholeTime - stopTime;
                    endTime = startTime + splitTime;
                    calculatedDate = calculatedDate.AddDays(1);
                }
                else
                {
                    endTime = wholeTime;
                }
            }
            else
            {
                if (wholeTime < startTime)
                {
                    TimeSpan splitTime = startTime - wholeTime;
                    endTime = stopTime - splitTime;
                    calculatedDate = calculatedDate.AddDays(-1);
                }
                else if (wholeTime > stopTime)
                {
                    endTime = stopTime;
                }
                else if (wholeTime == startTime && decimalPartOfday < 0)
                {
                    endTime = wholeTime;
                    calculatedDate = calculatedDate.AddDays(1);
                }
                else
                {
                    endTime = wholeTime;
                }

            }
            return new DateTime(calculatedDate.Year, calculatedDate.Month, calculatedDate.Day, endTime.Hours, endTime.Minutes, 00);
        }

        private DateTime CountDays(DateTime date, double estimatedDays)
        {
            int days = (int)estimatedDays;
            DateTime endDate = date;
            DateTime loopingDate = date;


            if (estimatedDays >= 0)
            {
                if (days == 0)
                {
                    loopingDate = loopingDate.AddDays(-1);
                }
                else
                {
                    endDate = endDate.AddDays(days);
                }
                while (!DateTime.Equals(loopingDate, endDate))
                {
                    loopingDate = loopingDate.AddDays(1);

                    if (CheckHolidaysAndWeekDays(loopingDate))
                    {
                        endDate = endDate.AddDays(1);
                    }
                }
            }
            else
            {
                if (days == 0)
                {
                    endDate = endDate.AddDays(-1);
                }
                else
                {
                    endDate = endDate.AddDays(days);
                }
                while (!DateTime.Equals(loopingDate, endDate))
                {
                    loopingDate = loopingDate.AddDays(-1);

                    if (CheckHolidaysAndWeekDays(loopingDate))
                    {
                        endDate = endDate.AddDays(-1);
                    }
                }
            }
            return endDate;
        }
    }
}
