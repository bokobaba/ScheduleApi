using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ScheduleApi.Utils {
    public static class ScheduleUtils {

        public enum ConditionTypes {
            EMPLOYEE, DAY, SHIFT, HOURS
        }

        public static readonly Dictionary<string, int> Days = new() {
            { "monday", 1 },
            { "tuesday", 2 },
            { "wednesday", 3 },
            { "thursday", 4 },
            { "friday", 5 },
            { "saturday", 6 },
            { "sunday", 7},
            { "all", 8 },
        };

        public static readonly Dictionary<int, string> ReverseDays = Days.ToDictionary(x => x.Value, x => x.Key);

        public delegate bool OperatorFunc(double hours, int comp, out double remain);

        public static readonly Dictionary<string, OperatorFunc> operatorMap =
            new() {
                { "=", delegate(double hours, int comp, out double remain) {
                    remain = comp - hours;
                    return hours == comp;
                }},
                { ">", delegate (double hours, int comp, out double remain) {
                    remain = int.MaxValue;
                    return hours >= comp;
                }},
                { "<", delegate(double hours, int comp, out double remain) {
                    remain = comp - hours;
                    return hours <= comp;
                }},
        };

    }

    public struct TimeRange : IEquatable<TimeRange> {
        public string Start;
        public string End;

        public bool Equals(TimeRange other) {
            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        public override string ToString() {
            return Start + ", " + End;
        }
    }

    public struct EmployeeDay : IEqualityComparer<EmployeeDay> {
        public int Day;
        public int EmployeeId;

        public EmployeeDay(int employeeId, int day) {
            Day = day;
            EmployeeId = employeeId;
        }

        public bool Equals(EmployeeDay x, EmployeeDay y) {
            return x.Day == y.Day && x.EmployeeId == y.EmployeeId;
        }

        public int GetHashCode([DisallowNull] EmployeeDay obj) {
            return HashCode.Combine(obj.Day, obj.EmployeeId);
        }
    }
}
