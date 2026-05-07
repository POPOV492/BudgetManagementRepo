using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BudgetManagement.Tests
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        public void Constructor_ValidData_CreatesTransaction()
        {
            var t = new Transaction("Зарплата", 50000m, TransactionType.Доход, new DateTime(2026, 5, 1));
            Assert.AreEqual("Зарплата", t.Description);
            Assert.AreEqual(50000m, t.Amount);
        }

        [TestMethod]
        public void Properties_CanBeChanged()
        {
            var t = new Transaction("Test", 100m, TransactionType.Доход, DateTime.Now);
            t.Description = "Updated";
            Assert.AreEqual("Updated", t.Description);
        }
    }
}