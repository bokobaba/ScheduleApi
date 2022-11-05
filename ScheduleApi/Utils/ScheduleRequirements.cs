using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using ScheduleApi.Utils;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static ScheduleApi.Utils.ScheduleUtils;

namespace ScheduleApi.Utils {

    public class ScheduleRequirements {
        private readonly Dictionary<int, Condition> _employeeHours = new();
        private readonly List<RequirementsDay> _schedules = new();
        private readonly Dictionary<int, TimeRange> _excludedEmployeeShifts = new();
        private readonly Dictionary<int, TimeRange> _requiredEmployeeShifts = new();
        private readonly Dictionary<int, double> _totalHours = new();
        private readonly ILogger _logger;

        public ScheduleRequirements(List<Rule> rules) {
            _schedules = new();

            for (int i = 0; i < 7; ++i) {
                _schedules.Add(new RequirementsDay() { Day = i });
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
                            GetValidSchedules(r, e);

                            // if no days are present, just add required shifts to schedule
                        } else {
                            if (r.Shift != null) {
                                // rule implies to exclude employee from shift
                                if (e.Operator != null && e.Operator.Equals("not")) {
                                    if (!_excludedEmployeeShifts.ContainsKey(e.Value)) {
                                        Debug.WriteLine("adding excludedEmployeeShift to GenSchedule");
                                        _excludedEmployeeShifts.Add(e.Value, r.Shift.Shift);
                                    }
                                    //rule implies to require shift for employee
                                } else {
                                    if (!_requiredEmployeeShifts.ContainsKey(e.Value)) {
                                        Debug.WriteLine("adding requiredEmployeeShift to GenSchedule");
                                        _requiredEmployeeShifts.Add(e.Value, r.Shift.Shift);
                                    }
                                }
                            }
                        }
                    });

                    // no Employees present, so only add day requirements
                } else if (r.Days.Count > 0) {
                    Debug.WriteLine("no employees found");
                    GetValidSchedules(r, null);
                }
            });

            Debug.WriteLine(this);
        }

        private static bool DayValid(RequirementsDay d, List<Condition> days, Condition? employee) {
            bool valid = true;
            int day = d.Day;
            foreach (Condition c in days) {
                bool not = c.Operator != null && c.Operator.Equals("not");
                int conditionDay = Days[c.Name];
                bool equal = day == conditionDay || conditionDay == 7;

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

        private void GetValidSchedules(Rule r, Condition? e) {
            _schedules.ForEach(d => {
                // not employee for day rule found so exclude employee from this day
                if (e != null && e.Operator != null && e.Operator.Equals("not"))
                    d.ExcludedEmployees.Add(e.Value);

                bool valid = DayValid(d, r.Days, e);

                if (valid) {
                    if (r.Shift != null) {
                        if (e != null) {
                            if (!d.Shifts.ContainsKey(e.Value))
                                d.Shifts.Add(e.Value, r.Shift.Shift);
                        } else {
                            d.RequiredShifts.Add(r.Shift.Shift);
                        }
                    } else {
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
            bool comp = operatorMap[c.Operator](hours, c.Value, out remain);

            remain = c.Operator.Equals(">") ? int.MaxValue : c.Value - hours;
            return comp;
        }

        public List<Schedule> GenerateSchedule(List<Employee> employees) {
            //GetScheduleRequirements(rules);

            Dictionary<EmployeeDay, Schedule> schedules = GenerateSchedules(employees);

            Debug.WriteLine(PrintSchedules(schedules));

            return schedules.Values.ToList();
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
            Dictionary<int, List<TimeRange>> unassigned;
            unassigned = AssignRequiredShifts(employees, schedules);

            // meet hours quotas if they exist for each employee
            Debug.WriteLine("HoursQota--------------------------------------");
            AssignRemainingHours(employees, schedules, unassigned);

            Debug.WriteLine("totalHours");
            foreach (KeyValuePair<int, double> item in _totalHours) {
                Debug.WriteLine(item.Key + ", " + item.Value);
            }


            return schedules;
        }

        private void AssignFullShifts(Dictionary<EmployeeDay, Schedule> schedules) {
            _schedules.ForEach(dayReq => {
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
                    }
                }
            });
        }

        private void AssignRequiredEmployees(Dictionary<EmployeeDay, Schedule> schedules) {
            _schedules.ForEach(dayReq => {
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
                            //HoursQuotaMet(hours, key.EmployeeId);
                            _totalHours[key.EmployeeId] = hours;
                            Debug.WriteLine("adding schedule: " + key.EmployeeId + ", " + ReverseDays[dayReq.Day]);

                        }

                        schedules.Add(key, schedule);
                    }
                }
            });
        }

        private Dictionary<int, List<TimeRange>> AssignRequiredShifts(List<Employee> employees,
            Dictionary<EmployeeDay, Schedule> schedules) {
            Dictionary<int, List<TimeRange>> unassignedReqShifts = new();

            int index = 0;
            foreach (RequirementsDay dayReq in _schedules) {
                if (dayReq.RequiredShifts.Count < 0)
                    continue;

                unassignedReqShifts.Add(index, new());

                foreach (TimeRange reqShift in dayReq.RequiredShifts) {
                    Debug.WriteLine("reqShift: " + ReverseDays[dayReq.Day] + ", " + reqShift);
                    bool assigned = false;
                    foreach (Employee e in employees) {
                        double hours = _totalHours[e.EmployeeId] + GetHours(reqShift);
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
                        schedule.Start = reqShift.Start;
                        schedule.End = reqShift.End;
                        _totalHours[key.EmployeeId] = hours;

                        if (add) {
                            schedule.EmployeeId = key.EmployeeId;
                            schedule.Day = key.Day;
                            schedules.Add(key, schedule);
                        }

                        assigned = true;
                        break;
                    }

                    if (!assigned) {
                        Debug.WriteLine(dayReq.Day + ", " + reqShift + ", unassigned-------------------");
                        unassignedReqShifts[index].Add(reqShift);
                    }
                }
                ++index;
            }

            return unassignedReqShifts;
        }

        private void AssignRemainingHours(List<Employee> employees,
            Dictionary<EmployeeDay, Schedule> schedules, Dictionary<int, List<TimeRange>> unassigned) {
            Debug.WriteLine("unassigned: " + unassigned.Count);
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

                FindShifts(e.EmployeeId, schedules, remain);
            }
        }

        private void FindShifts(int employee, Dictionary<EmployeeDay, Schedule> schedules, double remain) {
            foreach (RequirementsDay reqDay in _schedules) {
                EmployeeDay key = new(employee, reqDay.Day);

                // employee already scheduled this day
                if (schedules.ContainsKey(key))
                    continue;

                // find a shift in required shifts for this day
                foreach (TimeRange reqShift in reqDay.RequiredShifts) {
                    Debug.WriteLine(ReverseDays[reqDay.Day] + ", " + reqShift);
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
                        Start = reqShift.Start,
                        End = reqShift.End,
                    };
                    schedules.Add(key, schedule);
                    double hours = GetHours(reqShift);
                    _totalHours[key.EmployeeId] += hours;

                    remain -= hours;
                    if (remain <= 0)
                        return;

                    break;
                }
            }
        }

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
            _schedules.ForEach(s => str += s.ToString() + "\n");

            return str;
        }

        private class RequirementsDay {
            public int Day { get; set; }
            public HashSet<int> ExcludedEmployees = new();
            public HashSet<int> RequiredEmployees = new();
            public Dictionary<int, TimeRange> Shifts = new();
            public HashSet<TimeRange> RequiredShifts = new();
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
                foreach (TimeRange s in RequiredShifts) { str += "[" + s.Start + ", " + s.End + "], "; };

                return str;
            }
        }
    }
}