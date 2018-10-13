using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PayoutCalcApp;
using PayoutCalcApp.Controllers;
using PayoutCalcApp.Infrastructure;
using PayoutCalcApp.Models;

namespace PayoutCalcApp.Tests.Controllers
{
    //Fail, Pass, Refactor, Iterative design
    [TestClass]
    public class PayoutControllerTest
    {
        [TestInitialize]
        public void Setup()
        {
            DependencyResolver.SetResolver(new NinjectDependencyResolver());
            IocContainer.RegisterBindings();
        }


        [TestMethod]
        public void Index_GetRedirectToRouteResult_IfDropdownNotCached()
        {
            // Arrange
            var controller = new PayoutController(null);
            // Act
            var result = controller.Index() as RedirectToRouteResult;
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("~/Error/PageErrorFound/?msg=Dropdown was not populated, click on 'Return Back'",result.RouteName);
        }

        [TestMethod]
        public void Index_GetView_IfDropdownIsCached()
        {
            var countofHoursInCachedDropdown = 13;
            var defaultSelectedValue = -1;
            // Arrange
            HoursDropdownMapper.CacheHoursDropdown();
            var cacheService = IocContainer.Resolve<ICacheService>();
            var controller = new PayoutController(cacheService);
            // Act
            var result = controller.Index() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(countofHoursInCachedDropdown,((HoursViewModel)result.Model)?.StartTimeHours.Count());
            Assert.AreEqual(countofHoursInCachedDropdown, ((HoursViewModel)result.Model)?.BedTimeHours.Count());
            Assert.AreEqual(countofHoursInCachedDropdown, ((HoursViewModel)result.Model)?.EndTimeHours.Count());
            Assert.AreEqual(defaultSelectedValue, ((HoursViewModel) result.Model)?.SelectedStartTimeHours);
            Assert.AreEqual(defaultSelectedValue, ((HoursViewModel)result.Model)?.SelectedBedTimeHours);
            Assert.AreEqual(defaultSelectedValue, ((HoursViewModel)result.Model)?.SelectedEndTimeHours);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CalculatePayout_GetIndexView_IfHoursViewModelIsNull()
        {
            // Arrange
            var controller = new PayoutController(null);

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
            //start time can be the same as bed time
            //end time must be > then start time and bed time, since we do not pay franctional pay, so babysitter must be paid at least 1 hour
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

            var controller = new PayoutController(null);
            var listOfHoursViewModels = new List<HoursViewModel> {
                new HoursViewModel(),//HoursViewModel is null
                new HoursViewModel{
                    SelectedStartTimeHours = 0,//5pm bedtime is also astart time
                    SelectedBedTimeHours = 0,//5pm
                    SelectedEndTimeHours = 0//5pm must be paid at least 1 hour
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
            var controller = new PayoutController(null);
            var listOfHoursViewModels = new List<Dictionary<HoursViewModel, double>>
            {
                new Dictionary<HoursViewModel, double>
                {
                    {
                        new HoursViewModel
                        {
                            SelectedStartTimeHours = 7, //12am
                            SelectedBedTimeHours = 7, //12am
                            SelectedEndTimeHours = 11//4am 
                        },
                        64.00 //4*16 = 64
                    }
                },
                new Dictionary<HoursViewModel, double>
                {
                    {
                        new HoursViewModel
                        {
                            SelectedStartTimeHours = 7, //12am all hours over midnight
                            SelectedBedTimeHours = 8, //1am
                            SelectedEndTimeHours = 9//2am 
                        },
                        32.00 //2*16 = 32
                    }
                },
                new Dictionary<HoursViewModel, double>
                {
                    {
                        new HoursViewModel
                        {
                            SelectedStartTimeHours = 2, //7pm
                            SelectedBedTimeHours = 2, //7pm
                            SelectedEndTimeHours = 7//12am edge case
                        },
                        40.00 //Expected Result 5*8=40
                    }
                },
                new Dictionary<HoursViewModel, double>
                {
                    {
                        new HoursViewModel
                        {
                            SelectedStartTimeHours = 0, //5pm
                            SelectedBedTimeHours = 0, //5pm
                            SelectedEndTimeHours = 6 //11pm
                        },
                        48.00 //Expected Result (6*8)= 48
                    }
                },
                new Dictionary<HoursViewModel, double>
                {
                    {
                        new HoursViewModel
                        {
                            SelectedStartTimeHours = 0, //5pm edge case
                            SelectedBedTimeHours = 7, //12am
                            SelectedEndTimeHours = 11 //4am
                        },
                        148.00 //Expected Result (7*12)+(4*16) = 148
                    }
                },
                new Dictionary<HoursViewModel, double>
                {
                    {
                        new HoursViewModel
                        {

                            SelectedStartTimeHours = 0, //5pm
                            SelectedBedTimeHours = 6, //11pm
                            SelectedEndTimeHours = 10 //3am
                        },
                       128.00 //Expected Result (6*12)+(1*8)+(3*16)= 128
                    }
                },
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

