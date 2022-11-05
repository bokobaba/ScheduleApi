using static ScheduleApi.Utils.ScheduleUtils;

namespace ScheduleApi.Utils {

    public class Rule {
        public int Priority { get; set; }
        public bool Status { get; set; }
        public List<Condition> Days = new();
        public List<Condition> Employees = new();
        public Condition Shift { get; set; }
        public Condition Hours { get; set; }

        public override string ToString() {
            string str = "Rule:\n" +
                         "    Priority: " + Priority + "\n" +
                         "    Status: " + Status + "\n" +
                         "Conditions:\n";
            Days.ForEach(r => str += r.ToString());
            Employees.ForEach(r => str += r.ToString());
            str += Shift == null ? "" : Shift.ToString();
            str += Hours == null ? "" : Hours.ToString();

            return str;
        }
    }

    public class Condition {
        public string Name { get; set; }
        public ConditionTypes Type { get; set; }
        public string? Operator { get; set; }
        public int Value { get; set; }
        public TimeRange Shift { get; set; }
        //= new TimeRange() { Start = "09:00", End = "17:00" };

        public override string ToString() {
            return "Condition:\n" +
                   "    Name: " + Name + "\n" +
                   "    Type: " + Type + "\n" +
                   "    Operator: " + Operator + "\n" +
                   "    Value: " + Value + "\n";
        }
    }
}

    
