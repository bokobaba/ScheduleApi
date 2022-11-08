using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using ScheduleApi.Utils;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static ScheduleApi.Utils.ScheduleUtils;

namespace ScheduleApi.Utils {

    public class ScheduleRequirements {
        private readonly Dictionary<int, Condition> _employeeHours = new();
        private readonly List<RequirementsDay> _dayRequirements = new();
        private readonly Dictionary<int, TimeRange> _excludedEmployeeShifts = new();
        private readonly Dictionary<int, TimeRange> _requiredEmployeeShifts = new();
        private readonly Dictionary<int, double> _totalHours = new();
        private readonly ILogger _logger;

        public ScheduleRequirements(List<Rule> rules) {
            _dayRequirements = new();

            for (int i = 1; i < 8; ++i) {
                _dayRequirements.Add(new RequirementsDay() { Day = i });
            }
            rules.ForEach(r => {
                //Debug.WriteLine("------------------------------------------------------------------------");
                //Debug.WriteLine("checking Rule");
                Debug.WriteLine("Checking Rule================================================");

                Debug.WriteLine(r.ToString());

                // add requirements for employees. employees have highest priority
                if (r.Employees.Count > 0) {
                    r.Employees.ForEach(e => {
                        Debug.WriteLine(e.Value);

                        // add Hours condition for each employee if present
                        if (r.Hours != null && !_employeeHours.ContainsKey(e.Value)) {
                            _employeeHours.Add(e.Value, r.Hours);
                        }

                        // add employee to each day present, add shift if present
                        if (r.Days.Count > 0) {
                            GetDayRequirements(r, e);

                            // if no days are present, just add required shifts to schedule
                        } else {
                            if (r.Shifts.Count > 0) { // only one shift condition if employee is present
                                // rule implies to exclude employee from shift
                                if (e.Operator != null && e.Operator.Equals("not")) {
                                    if (!_excludedEmployeeShifts.ContainsKey(e.Value)) {
                                        Debug.WriteLine("adding excludedEmployeeShift to GenSchedule");
                                        _excludedEmployeeShifts.Add(e.Value, r.Shifts[0].Shift);
                                    }
                                    //rule implies to require shift for employee
                                } else {
                                    if (!_requiredEmployeeShifts.ContainsKey(e.Value)) {
                                        Debug.WriteLine("adding requiredEmployeeShift to GenSchedule");
                                        _requiredEmployeeShifts.Add(e.Value, r.Shifts[0].Shift);
                                    }
                                }
                            }
                        }
                    });

                    // no Employees present, so only add day requirements
                } else if (r.Days.Count > 0) {
                    Debug.WriteLine("no employees found");
                    GetDayRequirements(r, null);
                }
            });

            Debug.WriteLine(this);
        }

        private static bool DayMatch(RequirementsDay d, List<Condition> days, Condition? employee) {
            Debug.WriteLine("day valid");
            bool valid = true;
            int day = d.Day;
            foreach (Condition c in days) {
                bool not = c.Operator != null && c.Operator.Equals("not");
                int conditionDay = Days[c.Name];
                bool equal = day == conditionDay || conditionDay == Days["all"];

                // not day for employee rule found so exclude employee from this day
                if (equal && employee != null && not && !d.RequiredEmployees.Contains(employee.Value)) {
                    d.ExcludedEmployees.Add(employee.Value);
                }

                valid = not ? !equal : equal;
                if (!valid)
                    break;
            };

            return valid;
        }

        private void GetDayRequirements(Rule r, Condition? e) {
            _dayRequirements.ForEach(d => {
                // not employee for day rule found so exclude employee from this day
                if (e != null && e.Operator != null && e.Operator.Equals("not"))
                    d.ExcludedEmployees.Add(e.Value);

                bool match = DayMatch(d, r.Days, e);

                if (match) {
                    if (r.Shifts.Count > 0) {
                        if (e != null) {
                            // add employees required shift for this day
                            if (!d.Shifts.ContainsKey(e.Value))
                                d.Shifts.Add(e.Value, r.Shifts[0].Shift);
                        } else {
                            // indicate that a specific shift must be filled by an employee
                            r.Shifts.ForEach(shift => {
                                if (d.RequiredShifts.TryGetValue(shift.Shift, out int count))
                                    d.RequiredShifts[shift.Shift] = ++count;
                                else
                                    d.RequiredShifts.Add(shift.Shift, 1);
                            });
                        }
                    } else {
                        // indicate that an employee must work this day, no specific shift
                        if (e != null && !d.ExcludedEmployees.Contains(e.Value))
                            d.RequiredEmployees.Add(e.Value);
                    }
                }
            });
        }

        private bool HoursQuotaMet(double hours, int id, out double remain) {
            remain = 0;
            if (!_employeeHours.TryGetValue(id, out Condition? c))
                return true;

            if (c.Operator == null)
                return true;
            operatorMap.TryGetValue(c.Operator, out OperatorFunc? operation);
            if (operation == null)
                return true;
            bool comp = operation(hours, c.Value, out remain);

            remain = c.Operator.Equals(">") ? int.MaxValue : c.Value - hours;
            return comp;
        }

        public List<Schedule> GenerateSchedule(List<Employee> employees) {
            //GetScheduleRequirements(rules);

            Dictionary<EmployeeDay, Schedule> schedules = GenerateSchedules(employees);

            Debug.WriteLine(PrintSchedules(schedules));

            return schedules.Values.Where(s => s.Start != null && s.End != null).ToList();
        }

        private Dictionary<EmployeeDay, Schedule> GenerateSchedules(List<Employee> employees) {
            Dictionary<EmployeeDay, Schedule> schedules = new();

            employees.ForEach(e => {
                _totalHours.Add(e.EmployeeId, 0);
            });

            // full employee shifts already set in requirements
            Debug.WriteLine("RequiredEmployeeShifts--------------------------------------");
            AssignFullShifts(schedules);

            // Employees required on specific days
            Debug.WriteLine("RequiredEmployees-----------------------------------");
            AssignRequiredEmployees(schedules);

            // if this day requires certain shifts find an employee to fill them
            Debug.WriteLine("RequiredShifts--------------------------------------");
            AssignRequiredShifts(employees, schedules);

            // meet hours quotas if they exist for each employee
            Debug.WriteLine("HoursQota--------------------------------------");
            AssignRemainingHours(employees, schedules);

            Debug.WriteLine("totalHours");
            foreach (KeyValuePair<int, double> item in _totalHours) {
                Debug.WriteLine(item.Key + ", " + item.Value);
            }


            return schedules;
        }

        private void AssignFullShifts(Dictionary<EmployeeDay, Schedule> schedules) {
            _dayRequirements.ForEach(dayReq => {
                // full employee shifts already set in requirements
                if (dayReq.Shifts.Count > 0) {
                    Debug.WriteLine("assigining full shifts for " + ReverseDays[dayReq.Day]);
                    foreach (KeyValuePair<int, TimeRange> shift in dayReq.Shifts) {
                        double hours = _totalHours[shift.Key] + GetHours(shift.Value);
                        //if (HoursQuotaMet(hours, shift.Key, out double remain) && remain < 0)
                        //    continue;

                        EmployeeDay key = new(shift.Key, dayReq.Day);
                        Schedule schedule = new Schedule() {
                            Day = key.Day,
                            EmployeeId = key.EmployeeId,
                            Start = shift.Value.Start,
                            End = shift.Value.End,
                        };

                        schedules.Add(key, schedule);
                        Debug.WriteLine("adding schedule: " + key.EmployeeId + ", " + ReverseDays[dayReq.Day]);
                        _totalHours[key.EmployeeId] = hours;

                        // if this coincides with a required shift count it as filled
                        if (dayReq.RequiredShifts.TryGetValue(shift.Value, out int count)) {
                            dayReq.RequiredShifts[shift.Value] = count - 1;
                        }
                    }
                }
            });
        }

        private void AssignRequiredEmployees(Dictionary<EmployeeDay, Schedule> schedules) {
            _dayRequirements.ForEach(dayReq => {
                // create a schedule on this day for each required employee
                if (dayReq.RequiredEmployees.Count > 0) {
                    Debug.WriteLine("Assigning required employees on " + ReverseDays[dayReq.Day]);
                    foreach (int e in dayReq.RequiredEmployees) {
                        EmployeeDay key = new(e, dayReq.Day);

                        if (schedules.ContainsKey(key))
                            continue;

                        Schedule schedule = new Schedule() {
                            Day = key.Day,
                            EmployeeId = key.EmployeeId
                        };

                        // if this employee has a required shift, apply it
                        if (_requiredEmployeeShifts.TryGetValue(e, out TimeRange shift)) {
                            schedule.Start = shift.Start;
                            schedule.End = shift.End;

                            double hours = _totalHours[key.EmployeeId] + GetHours(shift);
                            _totalHours[key.EmployeeId] = hours;
                        } else if (dayReq.RequiredShifts.Count > 0) {
                            // find an unfilled required shift
                            Debug.WriteLine("finding a shift");
                            foreach(KeyValuePair<TimeRange, int> reqShift in dayReq.RequiredShifts) {
                                if (reqShift.Value <= 0) {
                                    continue;
                                }

                                Debug.WriteLine("reqShift: " + reqShift);
                                schedule.Start = reqShift.Key.Start;
                                schedule.End = reqShift.Key.End;
                                dayReq.RequiredShifts[reqShift.Key] = reqShift.Value - 1;
                                Debug.WriteLine("reqShift: " + reqShift);

                                double hours = _totalHours[key.EmployeeId] + GetHours(reqShift.Key);
                                _totalHours[key.EmployeeId] = hours;
                                break;
                            }
                        }

                        Debug.WriteLine("adding schedule: " + key.EmployeeId + ", " + ReverseDays[dayReq.Day]);
                        schedules.Add(key, schedule);
                    }
                }
            });
        }

        private void AssignRequiredShifts(List<Employee> employees,
            Dictionary<EmployeeDay, Schedule> schedules) {
            Dictionary<int, List<TimeRange>> unassignedReqShifts = new();

            foreach (RequirementsDay dayReq in _dayRequirements) {
                if (dayReq.RequiredShifts.Count < 0)
                    continue;

                foreach (Employee e in employees) {
                    if (dayReq.ExcludedEmployees.Contains(e.EmployeeId)) {
                        continue;
                    }

                    foreach (KeyValuePair<TimeRange, int> reqShift in dayReq.RequiredShifts) {
                    if (reqShift.Value <= 0)
                        continue;

                    for (int i = 0; i < reqShift.Value; ++i) {
                        Debug.WriteLine("");
                        Debug.WriteLine("reqShift: " + ReverseDays[dayReq.Day] + ", " + reqShift);
                            double hours = _totalHours[e.EmployeeId] + GetHours(reqShift.Key);
                            HoursQuotaMet(hours, e.EmployeeId, out double remain);
                            if (remain < 0) {
                                Debug.WriteLine("unable to add hours for employee: " + e.EmployeeId);
                                continue;
                            }

                            EmployeeDay key = new(e.EmployeeId, dayReq.Day);
                            Schedule schedule = new();
                            bool add = true;
                            if (schedules.ContainsKey(key)) {
                                schedule = schedules[key];
                                add = false;
                            }

                            // schedule already has a shift assigned for this employee
                            if (schedule.Start != null) {
                                Debug.WriteLine(e.EmployeeId + " already assigned to this day");
                                continue;
                            }

                            //ignore this shift for this employee if it exists in excluded shifts
                            if (_excludedEmployeeShifts.TryGetValue(e.EmployeeId, out TimeRange exShift)) {
                                if (exShift.Equals(reqShift)) {
                                    Debug.WriteLine(e.EmployeeId + " is excluded from this shift");
                                    continue;
                                }
                            }

                            Debug.WriteLine("adding schedule: " + key.EmployeeId + ", " + ReverseDays[dayReq.Day]);
                            schedule.Start = reqShift.Key.Start;
                            schedule.End = reqShift.Key.End;
                            dayReq.RequiredShifts[reqShift.Key] = reqShift.Value - 1;
                            Debug.WriteLine("reqShift: " + reqShift);
                            _totalHours[key.EmployeeId] = hours;

                            if (add) {
                                schedule.EmployeeId = key.EmployeeId;
                                schedule.Day = key.Day;
                                schedules.Add(key, schedule);
                            }

                            break;
                        }
                    }
                }
            }
        }

        private void AssignRemainingHours(List<Employee> employees,
            Dictionary<EmployeeDay, Schedule> schedules) {
            foreach (Employee e in employees) {
                if (!_employeeHours.TryGetValue(e.EmployeeId, out _)) {
                    Debug.WriteLine("employee: " + e.EmployeeId + " has no hours quota");
                    continue;
                }

                double hours = _totalHours[e.EmployeeId];
                bool quotaMet = HoursQuotaMet(hours, e.EmployeeId, out double remain);
                Debug.WriteLine("quota met for " + e.EmployeeId + " = " + quotaMet + ", remain = " + remain);
                if (remain <= 0)
                    continue;

                Debug.WriteLine("finding shifts for employee: " + e.EmployeeId);
                FindShifts(e.EmployeeId, schedules, remain);
            }
        }

        private void FindShifts(int employee, Dictionary<EmployeeDay, Schedule> schedules, double remain) {
            foreach (RequirementsDay reqDay in _dayRequirements) {
                EmployeeDay key = new(employee, reqDay.Day);

                // employee already scheduled this day
                if (schedules.ContainsKey(key))
                    continue;

                // employee excluded from this day
                if (reqDay.ExcludedEmployees.Contains(key.EmployeeId)) {
                    Debug.WriteLine(employee + " cannot work on " + reqDay.Day);
                    continue;
                }

                // find a shift in required shifts for this day
                foreach (KeyValuePair<TimeRange, int> reqShift in reqDay.RequiredShifts) {
                    Debug.WriteLine(ReverseDays[reqDay.Day] + ", " + reqShift.Key);
                    //if (reqShift.Value <= 0)
                    //    continue;
                    //ignore this shift for this employee if it exists in excluded shifts
                    if (_excludedEmployeeShifts.TryGetValue(employee, out TimeRange exShift)) {
                        if (exShift.Equals(reqShift)) {
                            Debug.WriteLine(employee + " is excluded from this shift");
                            continue;
                        }
                    }

                    Debug.WriteLine("assigning scheudle for " + employee);

                    Schedule schedule = new Schedule() {
                        Day = reqDay.Day,
                        EmployeeId = employee,
                        Start = reqShift.Key.Start,
                        End = reqShift.Key.End,
                    };
                    schedules.Add(key, schedule);
                    double hours = GetHours(reqShift.Key);
                    reqDay.RequiredShifts[reqShift.Key] = reqShift.Value - 1;

                    _totalHours[key.EmployeeId] += hours;

                    remain -= hours;
                    if (remain <= 0)
                        return;

                    break;
                }
            }
        }

        //private static bool RequiredShiftAssigned(KeyValuePair<TimeRange, int> reqShift, int day,
        //    Dictionary<EmployeeDay, Schedule> schedules) {
        //    return schedules.Values.Any(s =>
        //                            s.Start != null && s.End != null &&
        //                            s.Day == day &&
        //                            s.Start.Equals(reqShift.Start) &&
        //                            s.End.Equals(reqShift.End));
        //}

        private static double GetHours(TimeRange shift) {
            if (!DateTime.TryParse(shift.Start, out DateTime start) ||
                !DateTime.TryParse(shift.End, out DateTime end))
                throw new AppException("incorrectly formatted time");
            if (start.CompareTo(end) > 0)
                throw new AppException("incorrectly formatted time: start time is greater than end");
            return Math.Round(end.Subtract(start).TotalHours, 2);
        }

        // for debugging
        private static string PrintSchedules(Dictionary<EmployeeDay, Schedule> schedules) {
            string str = "";
            HashSet<int> employees = new();
            SortedSet<int> days = new();

            foreach (EmployeeDay ed in schedules.Keys) {
                employees.Add(ed.EmployeeId);
                days.Add(ed.Day);
            }

            int pad = 15;

            str += "Name".PadRight(pad);
            foreach (int day in days) {
                str += ReverseDays[day].PadRight(pad);
            }
            str += "\n";
            foreach (int e in employees) {
                str += (e + "").PadRight(pad);
                foreach (int day in days) {
                    EmployeeDay ed = new(e, day);
                    if (schedules.TryGetValue(ed, out Schedule? s)) {
                        str += (s.Start + "-" + s.End).PadRight(pad);
                    } else {
                        str += "".PadRight(pad);
                    }
                }
                str += "\n";
            }

            return str;
        }

        public override string ToString() {
            string str = "GenSchedule\n";
            str += "   requiredEmployeeShifts: ";
            foreach (KeyValuePair<int, TimeRange> shift in _requiredEmployeeShifts) {
                str += "[" + shift.Key + ", " + shift.Value.Start + ", " + shift.Value.End + "],";
            }
            str += "\n   ExcludedEmployeeShifts: ";
            foreach (KeyValuePair<int, TimeRange> shift in _excludedEmployeeShifts) {
                str += "[" + shift.Key + ", " + shift.Value.Start + ", " + shift.Value.End + "],";
            }
            str += "\n   EmployeeHours: ";
            foreach (KeyValuePair<int, Condition> hours in _employeeHours) {
                str += "[" + hours.Key + ", " + hours.Value.Operator + ", " + hours.Value.Value + "],";
            }
            str += "\n";
            _dayRequirements.ForEach(s => str += s.ToString() + "\n");

            return str;
        }

        private class RequirementsDay {
            public int Day { get; set; }
            public HashSet<int> ExcludedEmployees = new();
            public HashSet<int> RequiredEmployees = new();
            public Dictionary<int, TimeRange> Shifts = new();
            public Dictionary<TimeRange, int> RequiredShifts = new();
            public override string ToString() { 
                string str = "GenScheduleDay\n";
                str += "   Day: " + ReverseDays[Day] + "\n";
                str += "   requiredEmployees: ";
                foreach (int e in RequiredEmployees) { str += e + ", "; };
                str += "\n   ExcludedEmployees: ";
                foreach (int e in ExcludedEmployees) { str += e + ", "; };
                str += "\n   Shifts: ";
                foreach (KeyValuePair<int, TimeRange> item in Shifts) {
                    str += "[" + item.Key + ", " + item.Value.Start + ", " + item.Value.End + "], ";
                }
                str += "\n   requiredShifts: ";
                foreach (KeyValuePair<TimeRange, int> s in RequiredShifts) { str += "[" + s + "],";  };

                return str;
            }
        }
    }
}