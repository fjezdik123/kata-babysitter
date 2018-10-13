using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayoutCalcApp.Models;
using WebGrease.Css.Extensions;

namespace PayoutCalcApp.Infrastructure
{
    public static class HoursDropdownMapper
    {
        public static void CacheHoursDropdown()
        {
            var hoursViewModel = new HoursViewModel();
            var listOfHours = new List<SelectListItem> {new SelectListItem {Value = "-1", Text = "Select Time"}};
            const int noOfWorkingHoursInDropdown = 12;
            var shiftStartTime = 17;//5PM
            Enumerable.Range(0,noOfWorkingHoursInDropdown).ForEach(hour =>
            {
                var span = TimeSpan.FromHours(shiftStartTime);
                var time = DateTime.Today + span;
                listOfHours.Add(new SelectListItem
                {
                    Value =hour.ToString(),
                    Text = time.ToString("hh:mm tt")
                });
                shiftStartTime += 1;
            });
            hoursViewModel.StartTimeHours = listOfHours;
            hoursViewModel.BedTimeHours = listOfHours;
            hoursViewModel.EndTimeHours = listOfHours;
            IocContainer.Resolve<ICacheService>().SetCache(hoursViewModel, "WorkingHoursModel");
        }
    }
}