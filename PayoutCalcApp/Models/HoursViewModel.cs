using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayoutCalcApp.Models
{
    public class HoursViewModel
    {
        [Display(Name="Start Time:")]
        public int SelectedStartTimeHours { get; set; } = -1;
        [Display(Name = "Bed Time:")]
        public int SelectedBedTimeHours { get; set; } = -1;
        [Display(Name = "End Time:")]
        public int SelectedEndTimeHours { get; set; } = -1;
        public double? TotalPayout { get; set; }
        public string ErrorMessage { get; set; }
        public IEnumerable<SelectListItem> StartTimeHours { get; set; }
        public IEnumerable<SelectListItem> BedTimeHours { get; set; }
        public IEnumerable<SelectListItem> EndTimeHours { get; set; }
    }
}