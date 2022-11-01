using ScheduleApi.Models;

namespace ScheduleApi.Utils {
    public static class ScheduleUtils {

        public static Dictionary<string, int> Days = new Dictionary<string, int> {
            { "monday", 0 },
            { "tuesday", 1 },
            { "wednesday", 2 },
            { "thursday", 3 },
            { "friday", 4 },
            { "saturday", 5 },
            { "sunday", 6}
        };

        public static Dictionary<string, Func<Schedule, Condition, bool>> methodMap =
            new Dictionary<string, Func<Schedule, Condition, bool>>() {
                { "day", (s, c) => DayValid(s, c) },
                { "employee", (s, c) => EmployeeValid(s, c) }
        };

        private static bool DayValid(Schedule s, Condition c) {
            Console.WriteLine("DayValid?");
            //Console.WriteLine("Day: " + c.Name);
            //Console.WriteLine(s.Day + " == " + Days[c.Name.ToLower()]);
            //Console.WriteLine(s.Day == Days[c.Name.ToLower()]);
            bool not = c.Operator != null && c.Operator.Equals("not");
            bool valid = s.Day == Days[c.Name];
            return not ? !valid : valid;
        }

        private static bool EmployeeValid(Schedule s, Condition c) {
            Console.WriteLine("EmployeeValid");
            //Console.WriteLine("employeeId: " + c.Name);
            bool not = c.Operator != null && c.Operator.Equals("not");
            bool valid = s.EmployeeId == c.Value;
            return not ? !valid : valid;
        }
    }
}
