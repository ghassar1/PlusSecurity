using System;
using AcSys.Core.Data.Model.Base;
using AcSys.Core.Extensions;
using AcSys.ShiftManager.Model.Helpers;

namespace AcSys.ShiftManager.Model
{
    public class Shift : EntityBase
    {
        public Shift()
        {
            IsOpen = true;
            TotalBreakMins = 30;
        }

        public Shift(DateTime startTime, DateTime endTime, int totalBreakMins, User employee = null)
        {
            _employee = employee;
            IsOpen = employee == null;

            StartTime = startTime;
            EndTime = endTime;

            TotalBreakMins = totalBreakMins;
            Status = Enums.ShiftStatus.Due;

            UpdateStatus();

            if (TotalMins < 60)
                throw new ApplicationException("Shift duration cannot be less than one hour.");

            if (EndTime.IsBefore(StartTime))
                throw new ApplicationException("Shift end time cannot be earlier than it's start time.");

            if (StartTime.DateDiff("hour", EndTime) < 2)
                throw new ApplicationException("Shift duration cannot be less than one hour.");
        }

        User _employee;
        public virtual User Employee
        {
            get { return _employee; }
            set { _employee = value; }
        }

        public bool IsOpen { get; set; }

        public string Title { get; set; }
        public string Notes { get; set; }

        //public DateTime Date { get; set; }
        //public TimeSpan StartTime { get; set; }
        //public TimeSpan EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public DateTime? ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }

        public int TotalBreakMins { get; set; }
        //public DateTime? BreakStartTime { get; set; }
        //public DateTime? BreakEndTime { get; set; }
        //public int BreakMins { get; set; }

        public int TotalMins { get; set; }
        public int LateMins { get; set; }
        public int ClockedMins { get; set; }
        public int ShortMins { get; set; }

        public Enums.ShiftStatus Status { get; set; }
        public Enums.ShiftStartStatus StartStatus { get; set; }
        public Enums.ShiftEndStatus EndStatus { get; set; }

        public void Take(User employee)
        {
            Employee = employee;
            UpdateStatus();
        }

        public void Leave()
        {
            Employee.Shifts.Remove(this);
            Employee = null;

            UpdateStatus();
        }

        public void ClockIn(DateTime time)
        {
            ClockInTime = time;

            UpdateStatus();
        }

        public void ClockOut(DateTime time)
        {
            ClockOutTime = time;

            UpdateStatus();
        }

        public void UpdateStatus()
        {
            IsOpen = Employee == null;

            UpdateTotalMins();

            if (ClockInTime.HasValue)
            {
                //Status = Enums.ShiftStatus.ClockedIn;

                LateMins = (int)StartTime.DateDiff("minute", ClockInTime.Value);
                if (LateMins >= 0)  //clocking in on exact minute should be considered late.
                    StartStatus = Enums.ShiftStartStatus.Late;
                else if (LateMins < 0)
                    StartStatus = Enums.ShiftStartStatus.OnTime;
                else
                    StartStatus = Enums.ShiftStartStatus.None;


                if (ClockOutTime.HasValue)
                {
                    //TODO: Clocked in minutes should be calculated starting from shift start time or employee clock in time?
                    ClockedMins = (int)ClockInTime.Value.DateDiff("minute", ClockOutTime.Value);
                    //ClockedMins = (int)StartTime.DateDiff("minute", ClockOutTime.Value);

                    //if (ClockedMins >= TotalMins)
                    //    Status = Enums.ShiftStatus.Complete;

                    Status = Enums.ShiftStatus.Complete;

                    var endDiff = (int)EndTime.DateDiff("minute", ClockOutTime.Value);
                    if (endDiff < 0)
                        EndStatus = Enums.ShiftEndStatus.Early;
                    else if (endDiff < 15)
                        EndStatus = Enums.ShiftEndStatus.OnTime;
                    else if (endDiff > 15)
                        EndStatus = Enums.ShiftEndStatus.OnTime;    //Enums.ShiftEndStatus.Late;
                }
                else
                {
                    Status = Enums.ShiftStatus.Due;
                    EndStatus = Enums.ShiftEndStatus.None;
                }
            }
            else
            {
                Status = Enums.ShiftStatus.Due;
                StartStatus = Enums.ShiftStartStatus.None;
                EndStatus = Enums.ShiftEndStatus.None;
            }

            if (!(ClockInTime.HasValue && ClockOutTime.HasValue) && EndTime.IsBefore(DateTime.Now))
            {
                Status = Enums.ShiftStatus.Missed;
                StartStatus = Enums.ShiftStartStatus.None;
                EndStatus = Enums.ShiftEndStatus.None;
            }
        }

        void UpdateTotalMins()
        {
            TotalMins = (int)StartTime.DateDiff("minute", EndTime);
        }

        public bool StartingShortly()
        {
            return DateTime.Now.DateDiff("minute", StartTime) <= 15;
            //return StartTime.AddMinutes(-15).IsEarlierThan(DateTime.Now);
        }

        public bool NotStartingShortly()
        {
            return !StartingShortly();
        }

        public bool HasClockedIn()
        {
            return ClockInTime != null;
        }

        public bool HasNotClockedIn()
        {
            return !HasClockedIn();
        }

        public bool HasClockedOut()
        {
            return ClockOutTime != null;
        }

        public bool HasNotClockedOut()
        {
            return !HasClockedOut();
        }

        public bool HasReasonableTimeInStart()
        {
            return DateTime.Now.DateDiff("minute", StartTime) > 15;
        }

        public override string ToString()
        {
            return "Shift Titled: '{0} for '{1} - {2}'".FormatWith(Title, StartTime.ToFormattedTimeString(), EndTime.ToFormattedTimeString());
        }

        public override string ToDescription()
        {
            return ToString();
        }

        public bool HasPassed()
        {
            return EndTime.IsBefore(DateTime.Now);
        }
    }
}
