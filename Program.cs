using System;

namespace TaskPlanner
{
    public class Program
    {
        static void Main(string[] args)
        {
            TaskPlanner taskPlanner = new TaskPlanner();
            var workdayStartTime = new TimeSpan(8, 0, 0);
            var workdayStopTime = new TimeSpan(16, 0, 0);

            taskPlanner.SetWorkdayStartAndStop(workdayStartTime, workdayStopTime);
            taskPlanner.SetRecurringHoliday(new DateTime(2004, 5, 17, 0, 0, 0));
            taskPlanner.SetRecurringHoliday(new DateTime(2004, 5, 24, 0, 0, 0));
            taskPlanner.SetHoliday(new DateTime(2004, 5, 27, 0, 0, 0));

            var start = new DateTime(2004, 5, 24, 18, 5, 0);
            double numberOfDays = -5.5;

            var actual = taskPlanner.GetTaskFinishingDate(start, numberOfDays);
            Console.WriteLine("end: {0:f}", actual);
        }
    }
}
