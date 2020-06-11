using NUnit.Framework;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WpfApp.Models;
using WpfApp.Services;

namespace NUnitTestProject
{
    public class Tests
    {
        private IOrderService orderServiceMock;

        [SetUp]
        public void Setup()
        {
            orderServiceMock = new OrderService();
        }

        [TestCase("data.csv")]
        public void LoadFileTest(string path)
        {
            var fi = new FileInfo(path);
            Debug.WriteLine(fi.FullName);
            var data = orderServiceMock.LoadFile(path).ToList();
            using (var reader = File.OpenText(path))
            {
                string line = reader.ReadLine();
                Assert.AreEqual(line, Order.CSVHeaders());

                for (int i = 0; i < data.Count; i++)
                {
                    line = reader.ReadLine();
                    Assert.IsNotNull(line);
                    Assert.AreEqual(line, data[i].ToCSV());
                }
            }
        }

        [TestCaseSource(typeof(TestData), "Orders")]
        public void OrderValidationSuccessTest(Order order)
        {
            order.ForceValidate();
            Assert.IsFalse(order.ValidationErrors.Any());
            Assert.IsEmpty(order.Error);
        }
        
        [TestCaseSource(typeof(TestData), "ErrorOrders")]
        public void OrderValidationErrorTest(Order order)
        {
            order.ForceValidate();
            Assert.IsNotEmpty(order.Error);
        }
    }

    public class TestData
    {
        public static IEnumerable Orders
        {
            get
            {
                yield return new TestCaseData(new Order("aaa", Guid.NewGuid(), "aaa", "aaa", 100.0m, 2));
                yield return new TestCaseData(new Order(new string('a', 100), Guid.NewGuid(), new string('a', 100), new string('a', 100), 100.0m, 2));
                yield return new TestCaseData(new Order("aaa", Guid.NewGuid(), "aaa", "aaa", decimal.MaxValue, 2));
                yield return new TestCaseData(new Order("aaa", Guid.NewGuid(), "aaa", "aaa", decimal.MinValue, 2));
                yield return new TestCaseData(new Order("aaa", Guid.NewGuid(), "aaa", "aaa", 100.0m, long.MaxValue));
            }
        }

        public static IEnumerable ErrorOrders
        {
            get
            {
                yield return new TestCaseData(new Order(string.Empty, Guid.NewGuid(), "aaa", "aaa", 100.0m, 2));
                yield return new TestCaseData(new Order(null, Guid.NewGuid(), "aaa", "aaa", 100.0m, 2));
                yield return new TestCaseData(new Order("aaa", null, "aaa", "aaa", 100.0m, 2));
                yield return new TestCaseData(new Order("aaa", Guid.NewGuid(), string.Empty, "aaa", 100.0m, 2));
                yield return new TestCaseData(new Order("aaa", Guid.NewGuid(), null, "aaa", 100.0m, 2));
                yield return new TestCaseData(new Order("aaa", Guid.NewGuid(), "aaa", string.Empty, 100.0m, 2));
                yield return new TestCaseData(new Order("aaa", Guid.NewGuid(), "aaa", null, 100.0m, 2));
                yield return new TestCaseData(new Order("aaa", Guid.NewGuid(), "aaa", "aaa", null, 2));
                yield return new TestCaseData(new Order("aaa", Guid.NewGuid(), "aaa", "aaa", 100.0m, null));
                yield return new TestCaseData(new Order("aaa", Guid.NewGuid(), "aaa", "aaa", 100.0m, 0));
                yield return new TestCaseData(new Order("aaa", Guid.NewGuid(), "aaa", "aaa", 100.0m, -8));
            }
        }
    }
}