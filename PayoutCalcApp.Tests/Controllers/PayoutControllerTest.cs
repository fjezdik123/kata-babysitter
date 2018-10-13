﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PayoutCalcApp;
using PayoutCalcApp.Controllers;
using PayoutCalcApp.Models;

namespace PayoutCalcApp.Tests.Controllers
{
    //Fail, Pass, Refactor, Iterative design
    [TestClass]
    public class PayoutControllerTest
    {

        [TestMethod]
        public void Index()
        {
            // Arrange
            var controller = new PayoutController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CalculatePayout_GetIndexView_IfHoursViewModelIsNull()
        {
            // Arrange
            var controller = new PayoutController();

            // Act
            var result = controller.CalculatePayout(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index",result.ViewName);
        }

        [TestMethod]
        public void CalculatePayout_GetJsonErrorMessage_IfHoursViewModelIsNotValid()
        {

            // Arrange

            #region Prerequisites For Test
            //gets paid $12/hour from start-time to bedtime
            //gets paid $8 / hour from bedtime to midnight
            //gets paid $16 / hour from midnight to end of job
            //mapping
            //Time   hours
            //5PM     0 
            //6PM    1
            //7PM    2
            //8PM    3
            //9PM    4
            //10PM   5
            //11PM   6
            //12AM   7
            //1AM    8
            //2AM    9
            //3AM   10
            //4AM    11
            #endregion

            var controller = new PayoutController();
            var listOfHoursViewModels = new List<HoursViewModel> {
                new HoursViewModel(),//null
                new HoursViewModel{
                    SelectedStartTimeHours = 0,
                    SelectedBedTimeHours = 0,
                    SelectedEndTimeHours = 0
                                    },
                new HoursViewModel{
                    // start time after bed time
                    SelectedStartTimeHours = 5,
                    SelectedBedTimeHours = 4,
                    SelectedEndTimeHours = 10
                                    },
                new HoursViewModel{
                    // start time after end time
                    SelectedStartTimeHours = 7,
                    SelectedBedTimeHours = 4,
                    SelectedEndTimeHours = 5
                },
                new HoursViewModel{
                    // bed time before start time
                    SelectedStartTimeHours = 8,
                    SelectedBedTimeHours = 7,
                    SelectedEndTimeHours = 10
                },
                new HoursViewModel{
                    // end time before start time
                    SelectedStartTimeHours = 8,
                    SelectedBedTimeHours = 10,
                    SelectedEndTimeHours = 6
                },
                new HoursViewModel{
                    // end time before bed time
                    SelectedStartTimeHours = 8,
                    SelectedBedTimeHours = 10,
                    SelectedEndTimeHours = 9
                },

            };

            // Act
            listOfHoursViewModels.ForEach(hModel =>
            {
                var result = controller.CalculatePayout(hModel) as JsonResult;
                // Assert
                Assert.AreEqual("Selected Hours are not valid", ((HoursViewModel)result?.Data)?.ErrorMessage);
            });
        }

        [TestMethod]
        public void CalculatePayout_GetViewPayoutTotal_IfHoursViewModelIsValid()
        {
            // Arrange
            var controller = new PayoutController();
            var listOfHoursViewModels = new List<Dictionary<HoursViewModel, double>>
            {
                new Dictionary<HoursViewModel, double>
                {
                    {
                        new HoursViewModel
                        {

                            SelectedStartTimeHours = 5, //10pm
                            SelectedBedTimeHours = 6, //11pm
                            SelectedEndTimeHours = 10 //3am
                        },
                        68.00 //Expected Result (1*12)+(1*8)+(3*16)= 68
                    }
                },
                new Dictionary<HoursViewModel, double>
                {
                    {
                        new HoursViewModel
                        {
                            SelectedStartTimeHours = 1, //6pm
                            SelectedBedTimeHours = 3, //8pm
                            SelectedEndTimeHours = 4 //9pm
                        },
                        32.00 //Expected Result (2*12)+(1*8)= 32
                    }
                },
                new Dictionary<HoursViewModel, double>
                {
                    {
                        new HoursViewModel
                        {
                            SelectedStartTimeHours = 6, //11pm
                            SelectedBedTimeHours = 8, //1am
                            SelectedEndTimeHours = 11 //4am
                        },
                        76.00 //Expected Result (1*12)+(4*16)= 76
                    }
                }
            };

            // Act
            listOfHoursViewModels.ForEach(hModel =>
            {
                var result = controller.CalculatePayout(hModel.Keys.First()) as PartialViewResult;
                // Assert
                Assert.AreEqual(hModel.Values.First(), ((HoursViewModel)result?.Model)?.TotalPayout);
            });
           
        }


    }
}

