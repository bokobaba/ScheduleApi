using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using static ScheduleApi.Utils.ScheduleUtils;

namespace ScheduleApi.Utils {

    public class Rule {
        public int Priority { get; set; }
        public bool Status { get; set; }
        public List<Condition> Days = new();
        public List<Condition> Employees = new();
        public List<Condition> Shifts = new();
        public Condition Hours { get; set; }

        public Rule(RuleGroup ruleGroup, List<Employee> employees, List<Shift> shifts) {
            // rules separated by ; operator separated by , type separated by :
            string[] rules = ruleGroup.Rules.Split(";");
            if (rules.Length == 0) return;

            foreach (string rule in rules) {
                if (rule == null || rule.Equals(""))
                    continue;

                string[] properties = rule.Split(":");

                if (properties.Length < 2)
                    throw new AppException("incorrectly formatted rule");

                string[] description = properties[1].Split(",");
                if (description.Length < 1)
                    throw new AppException("incorrectly formatted rule");

                string type = properties[0].ToLower();

                switch (type) {
                    case "hours":
                        Hours = ParseHours(description);
                        break;
                    case "employee":
                        Employees.AddRange(ParseEmployee(description, employees));
                        break;
                    case "day":
                        Days.Add(ParseDay(description));
                        break;
                    case "shift":
                        Condition? c = ParseShift(description, shifts);
                        if (c != null)
                            Shifts.Add(c);
                        break;
                    default:
                        throw new AppException("invalid identifier: " + type);
                }
            }
        }

        private static Condition ParseHours(string[] description) {
            if (description.Length != 2 || !int.TryParse(description[1], out _))
                throw new AppException("incorrectly formatted rule");

            return new Condition() {
                Name = "hours",
                Type = ConditionTypes.HOURS,
                Operator = description[0].Trim().ToLower(),
                Value = int.Parse(description[1]),
            };
        }

        private static Condition? ParseShift(string[] description, List<Shift> shifts) {
            if (description.Length != 1 || !int.TryParse(description[0], out int id))
                throw new AppException("incorrectly formatted rule");
            Shift? s = shifts.Find(s => s.ID == id);
            if (s == null)
                return null;

            return new Condition() {
                Name = "shift",
                Type = ConditionTypes.SHIFT,
                Value = id,
                Shift = new TimeRange() {
                    Start = s.Start,
                    End = s.End
                }
            };
        }

        private static List<Condition> ParseEmployee(string[] description, List<Employee> employees) {
            bool opPresent = description.Length > 1;
            string name = opPresent ? description[1].Trim().ToLower() : description[0].Trim().ToLower();
            string? op = opPresent ? description[0].ToLower() : null;

            if (name.Equals("all")) {
                List<Condition> conditions = new();
                employees.ForEach(e => {
                    conditions.Add(new Condition() {
                        Name = name,
                        Type = ConditionTypes.EMPLOYEE,
                        Operator = op,
                        Value = e.EmployeeId
                    });
                });
                return conditions;
            }
            if (!int.TryParse(name, out int value))
                throw new AppException("incorrectly formatted rule");

            return new List<Condition>() {
                new Condition() {
                    Name = name,
                    Type = ConditionTypes.EMPLOYEE,
                    Operator = op,
                    Value = value
            }};
        }

        private static Condition ParseDay(string[] description) {
            bool opPresent = description.Length > 1;
            string name = opPresent ? description[1] : description[0];
            string? op = opPresent ? description[0].Trim().ToLower() : null;

            return new Condition() {
                Name = name.Trim().ToLower(),
                Type = ConditionTypes.DAY,
                Operator = op
            };
        }

        public override string ToString() {
            string str = "Rule:\n" +
                         "    Priority: " + Priority + "\n" +
                         "    Status: " + Status + "\n" +
                         "Conditions:\n";
            Days.ForEach(r => str += r.ToString());
            Employees.ForEach(r => str += r.ToString());
            Shifts.ForEach(s => str += (s.ToString() + ", "));
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

    
