using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayoutCalcApp.Models;

namespace PayoutCalcApp.Controllers
{
    public class PayoutController : Controller
    {
        private const int MidnightHoursFromShiftStart = 7; //hours
        private const int MaxWorkingHoursFromStart = 11;
        private const double StartToBedPay = 12.00; //$12 pay
        private const double BedToMidnightPay = 8;//$8 pay
        private const double MidnightToEndPay = 16; //$16 pay

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult CalculatePayout(HoursViewModel hoursModel)
        {
            if (hoursModel == null) return View("Index");
            hoursModel.ErrorMessage = string.Empty;
            if (IsModelValid(hoursModel))
            {
                var selectedStartTimeHours = hoursModel.SelectedStartTimeHours;
                var selectedBedTimeHours = hoursModel.SelectedBedTimeHours;
                var selectedEndTimeHours = hoursModel.SelectedEndTimeHours;
                hoursModel.TotalPayout = selectedEndTimeHours <= MidnightHoursFromShiftStart
                    ? GetTotalPayoutNotOverMidnight(selectedStartTimeHours, selectedBedTimeHours, selectedEndTimeHours)//babysitter is not working over midnight
                    : GetTotalPayoutOverMidnight(selectedStartTimeHours, selectedBedTimeHours, selectedEndTimeHours);

            }
            else
            {
                hoursModel.ErrorMessage = "Selected Hours are not valid";
                return Json(hoursModel, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_TotalPayout", hoursModel);
        }

        internal double? GetTotalPayoutOverMidnight(int selectedStartTimeHours, int selectedBedTimeHours, int selectedEndTimeHours)
        {
            double startToBedPayTotal;
            var bedToMidnightPayTotal = 0.00;
            var midnightToEndPayTotal = (selectedEndTimeHours - MidnightHoursFromShiftStart) * MidnightToEndPay;
            var isBedTimeOverMidnight = selectedBedTimeHours > MidnightHoursFromShiftStart;
            if (isBedTimeOverMidnight)
            {
                startToBedPayTotal = (MidnightHoursFromShiftStart - selectedStartTimeHours) * StartToBedPay;

            }
            else
            {
                //bedtime is over midnight
                startToBedPayTotal = (selectedBedTimeHours - selectedStartTimeHours) * StartToBedPay;
                bedToMidnightPayTotal = (MidnightHoursFromShiftStart - selectedBedTimeHours) * BedToMidnightPay;
            }

            return midnightToEndPayTotal + startToBedPayTotal + bedToMidnightPayTotal;
        }

        internal double? GetTotalPayoutNotOverMidnight(int selectedStartTimeHours, int selectedBedTimeHours, int selectedEndTimeHours)
        {
            var startToBedPayTotal = (selectedBedTimeHours - selectedStartTimeHours) * StartToBedPay;
            var bedToMidnightPayTotal = (selectedEndTimeHours - selectedBedTimeHours) * BedToMidnightPay;
            return startToBedPayTotal + bedToMidnightPayTotal;
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        internal bool IsModelValid(HoursViewModel hoursModel)
        {            
            return  hoursModel != null &&
                    hoursModel.SelectedStartTimeHours != -1 && //-1 is default value for dropdown for 'Select Time'
                    hoursModel.SelectedBedTimeHours != -1 &&
                    hoursModel.SelectedEndTimeHours != -1 &&
                    hoursModel.SelectedStartTimeHours >= 0 &&//start time can start at 0, 5pm start
                    hoursModel.SelectedBedTimeHours > 0 &&
                    hoursModel.SelectedEndTimeHours > 0 &&
                    hoursModel.SelectedStartTimeHours < hoursModel.SelectedBedTimeHours &&
                    hoursModel.SelectedBedTimeHours < hoursModel.SelectedEndTimeHours &&
                    hoursModel.SelectedStartTimeHours <= MaxWorkingHoursFromStart &&
                    hoursModel.SelectedBedTimeHours <= MaxWorkingHoursFromStart &&
                    hoursModel.SelectedEndTimeHours <= MaxWorkingHoursFromStart &&
                    ModelState.IsValid;
        }
    }
}