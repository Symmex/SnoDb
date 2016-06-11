using System.Linq;
using NUnit.Framework;
using Symmex.SnoDb;

namespace SnoDb.Tests
{
    /// <summary>
    ///     This class is used to test SnoDb main functions
    /// </summary>
    [TestFixture]
    public class SnoDbTests
    {
        [Test]
        public void Create_Database_Default()
        {
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Steve",
                LastName = "McCann"
            };
            
            var database = new SnoDatabase("Customer");
            var c = database.RegisterCollection<Customer, int>(cu => cu.Id);
            c.AddOrReplace(customer);

            var cust = c.Query().Single(cs => cs.FirstName == "Steve");

            Assert.AreEqual(cust.FirstName, customer.FirstName);
        }
    }
}