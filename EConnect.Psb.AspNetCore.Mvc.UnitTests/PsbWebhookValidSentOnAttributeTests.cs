using System;
using System.ComponentModel.DataAnnotations;
using EConnect.Psb.AspNetCore.Mvc.UnitTests.Helpers;
using EConnect.Psb.AspNetCore.Mvc.Webhook;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.AspNetCore.Mvc.UnitTests
{
    [TestClass]
    public class PsbWebhookValidSentOnAttributeTests
    {

        [TestMethod]
        public void PsbWebhookShouldBeValid()
        {
            // Arrange
            var webhook = PsbWebHookExamples.InvoiceSent(DateTimeOffset.UtcNow);
            var attr = new PsbWebhookValidSentOnAttribute();

            // Act
            var res = attr.IsValid(webhook);

            // Assert
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void PsbWebhookShouldBeInValid()
        {
            // Arrange
            var webhook = PsbWebHookExamples.InvoiceSent(DateTimeOffset.UtcNow.AddMinutes(-10));
            var attr = new PsbWebhookValidSentOnAttribute();
            
            // Act
            var res = attr.IsValid(webhook);
            var message = attr.GetValidationResult(webhook, new ValidationContext(webhook)).ErrorMessage;

            // Assert
            Assert.IsFalse(res);
            Assert.IsNotNull(message);
            Assert.AreEqual("PsbWebhookEvent does not fall in the expected time range. The event is too old or too new.", message);
        }
    }
}