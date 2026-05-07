using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BudgetManagement.Tests
{
    [TestClass]
    public class BudgetManagerTests
    {
        [TestMethod]
        public void AddTransaction_AddsToList()
        {
            if (File.Exists("transactions.txt")) File.Delete("transactions.txt");
            var bm = new BudgetManager();
            bm.AddTransaction(new Transaction("Test", 100m, TransactionType.Доход, DateTime.Now));
            Assert.AreEqual(1, bm.Transactions.Count);
        }

        [TestMethod]
        public void TotalBudget_CalculatesCorrectly()
        {
            if (File.Exists("transactions.txt")) File.Delete("transactions.txt");
            var bm = new BudgetManager();
            bm.AddTransaction(new Transaction("A", 1000m, TransactionType.Доход, DateTime.Now));
            bm.AddTransaction(new Transaction("B", 300m, TransactionType.Расход, DateTime.Now));
            Assert.AreEqual(700m, bm.TotalBudget);
        }
    }
}