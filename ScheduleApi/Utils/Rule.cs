using ScheduleApi.Models;
using System.Collections.Generic;
using static ScheduleApi.Utils.ScheduleUtils;

namespace ScheduleApi.Utils {

    public class Rule {
        public int Priority { get; set; }
        public bool Status { get; set; }
        public List<Condition> Conditions { get; set; }

        public bool Valid(List<Schedule> schedules) {
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("checking Rule");
            int i = 0, j = 0;

            List<Schedule> validSchedules = new List<Schedule>(schedules);


            //List<Schedule> validSchedules = new List<Schedule>();
            foreach (Condition c in Conditions) {
                Console.WriteLine("cccccccccccccccccccccccccccccccccccccccccccccc");
                Console.WriteLine("Checking Condition " + j++);
                Console.WriteLine(c);

                validSchedules = validSchedules.Where(s => {
                    Console.WriteLine("sssssssssssssssssssssssss");
                    Console.WriteLine(s);
                    bool valid = c.Valid(s);
                    Console.WriteLine("Condition Valid: " + valid + "\n");
                    return valid;
                }).ToList();

                Console.WriteLine("valid schedules: " + validSchedules.Count);

            }

            Console.WriteLine("resulting matches: " + validSchedules.Count);
            validSchedules.ForEach(s => Console.WriteLine(s));
            //List<Schedule> validSchedules = schedules.Where(s => {
            //    Console.WriteLine("ssssssssssssssssssssssssssssssssssssssssss");
            //    Console.WriteLine(s);
            //    return ConditionsValid(s);
            //}).ToList();

            //Console.WriteLine("valid schedules: " + validSchedules.Count);
            //foreach (Schedule s in schedules) {
            //    Console.WriteLine("ssssssssssssssssssssssssssssssssssssssssss");
            //    Console.WriteLine("Checking schedule " + i++);
            //    Console.WriteLine(s);
            //foreach (Condition c in Conditions) {
            //        Console.WriteLine("cccccccccccccccccccc");
            //        Console.WriteLine("Checking Condition " + j++);
            //        Console.WriteLine(c);
            //        bool valid = c.Valid(s);
            //        Console.WriteLine("Condition Valid: " + valid + "\n");
            //    };
            //};

            return true;
        }

        //private bool ConditionsValid(List<Schedule> schedules) {
        //    int j = 0;
        //    foreach (Condition c in Conditions) {
        //        Console.WriteLine("cccccccccccccccccccc");
        //        Console.WriteLine("Checking Condition " + j++);
        //        Console.WriteLine(c);
        //        bool valid = c.Valid(s);
        //        Console.WriteLine("Condition Valid: " + valid + "\n");
        //        if (!valid)
        //            return false;
        //    }
        //    return true;
        //}

        public override string ToString() {
            string str = "Rule:\n" +
                         "    Priority: " + Priority + "\n" +
                         "    Status: " + Status + "\n" +
                         "Conditions:\n";
            Conditions.ForEach(r => str += r.ToString());
            return str;
        }
    }

    public class Condition {
        public string Name { get; set; }
        public string Type { get; set; }
        public string? Operator { get; set; }
        public int? Value { get; set; }
        public TimeRange? Shift { get; set; }

        public bool Valid(Schedule schedule) {
            Console.WriteLine("checking condition valid");

            if (Type.Equals("day") || Type.Equals("employee")) {
                if (methodMap[Type](schedule, this)) {
                    return true;
                }
            }

            return false;
        }

        public override string ToString() {
            return "Condition:\n" +
                   "    Name: " + Name + "\n" +
                   "    Type: " + Type + "\n" +
                   "    Operator: " + Operator + "\n" +
                   "    Value: " + Value + "\n";
        }

    }

    public class TimeRange {
        public string Start { get; set; }
        public string End { get; set; }
    }
}
