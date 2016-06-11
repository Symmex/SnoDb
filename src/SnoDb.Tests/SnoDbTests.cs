using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Runtime.Remoting;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using NUnit.Common;
using NUnit.Framework;

using SnoDb;
using Symmex.SnoDb;

namespace SnoDb.Tests
{

    /// <summary>
    /// this class is used to test sno db main functions
    /// </summary>
    [TestFixture]
    public class SnoDbTests
    {
        [Test]
        public void Create_Database_Default()
        {
            // prepare test 
            var customer = new Customer
            {                  
                Id = 1,
                FirstName = "Steve",
                LastName = "McCann"
                };
            // execute
            var database = new SnoDatabase("Customer");
            var c = database.RegisterCollection<Customer, int>(cu => cu.Id);
            c.AddOrReplace(customer);

            database.Load();
            var cust = c.Query().Single(cs => cs.FirstName == "Steve");
            
            Assert.AreEqual(cust.FirstName , customer.FirstName);


        }
    }


    /// <summary>
    /// test class to use for tests
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
    }
    
}
