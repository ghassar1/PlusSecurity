using System;
using System.Collections.Generic;
using AcSys.ShiftManager.Service.Messages;

namespace AcSys.ShiftManager.Service.Shifts
{
    public class DashboardDto
    {
        public DashboardDto()
        {
            //CurrentWeekStats = new ShiftStatsDto();
            //LastWeekStats = new ShiftStatsDto();

            PerformanceChartItems = new List<PerformanceChartItemDto>();

            //AttendanceThisWeek = new List<ChartDataItemDto>();
            //AttendanceLastWeek = new List<ChartDataItemDto>();

            //LateThisWeek = new List<ChartDataItemDto>();
            //LateLastWeek = new List<ChartDataItemDto>();

            Messages = new List<MessageDto>();
        }

        public IList<MessageDto> Messages { get; set; }

        public ShiftStatsDto CurrentWeekStats { get; set; }
        public ShiftStatsDto CurrentMonthStats { get; set; }
        public ShiftStatsDto LastMonthStats { get; set; }

        public List<PerformanceChartItemDto> PerformanceChartItems { get; set; }

        //public List<ChartDataItemDto> AttendanceThisWeek { get; set; }
        //public List<ChartDataItemDto> LateThisWeek { get; set; }

        //public List<ChartDataItemDto> AttendanceLastWeek { get; set; }
        //public List<ChartDataItemDto> LateLastWeek { get; set; }
    }

    public class ShiftStatsDto
    {
        public int Total { get; set; }
        public int TotalHours { get; set; }
        public int TotalClockedHours { get; set; }

        public int Completed { get; set; }
        public int Due { get; set; }
        public int Missed { get; set; }

        public int OnTime { get; set; }
        public int Late { get; set; }

        public double CompletedPercentage { get; set; }
        public double DuePercentage { get; set; }
        public double MissedPercentage { get; set; }

        public double OnTimePercentage { get; set; }
        public double LatePercentage { get; set; }
    }

    public class PerformanceChartItemDto
    {
        public string Label { get; set; }
        
        //public int Completed { get; set; }
        //public int OnTime { get; set; }
        //public int Late { get; set; }
        public ShiftStatsDto Stats { get; set; }
    }

    //public class ChartDataItemDto
    //{
    //    public string Label { get; set; }
    //    public int Value { get; set; }

    //    public ChartDataItemDto(string label, int value)
    //    {
    //        Label = label;
    //        Value = value;
    //    }
    //}
}
