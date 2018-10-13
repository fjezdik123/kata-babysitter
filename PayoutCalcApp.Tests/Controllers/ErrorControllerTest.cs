using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PayoutCalcApp.Controllers;
using PayoutCalcApp.Models;

namespace PayoutCalcApp.Tests.Controllers
{
    [TestClass]
    public class ErrorControllerTest
    {
        [TestMethod]
        public void PageErrorFound_GetRedirectResult_ErrorIsNull()
        {
           
            var controller = new ErrorController();

            // Act
            var result = controller.PageErrorFound(null) as RedirectResult;
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("~/Payout",result.Url);
          
        }

        [TestMethod]
        public void PageErrorFound_GetRedirectResult_ErrorIsEmpty()
        {

            var controller = new ErrorController();

            // Act
            var result = controller.PageErrorFound(string.Empty) as RedirectResult;
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("~/Payout", result.Url);

        }

        [TestMethod]
        public void PageErrorFound_GetView_ErrorIsNotNullOrEmpty()
        {

            var controller = new ErrorController();
            var msg = "Error";
            // Act
            var result = controller.PageErrorFound(msg) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(msg,result.Model.ToString());

        }

    }
}
