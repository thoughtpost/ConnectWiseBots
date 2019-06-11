using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;

using System.Collections.Generic;
using System.Threading.Tasks;

using Thoughtpost.ConnectWise;
using Thoughtpost.ConnectWise.Manage;
using Thoughtpost.ConnectWise.Manage.Models;

namespace Thoughtpost.ConnectWise.Tests
{
    [TestClass]
    public class ManageTests
    {
        protected ManageApiClient GetApiClient()
        {
            ManageApiClient client = new ManageApiClient();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cwSection = config.GetSection("ConnectWise");

            client.AppId = cwSection["AppId"];
            client.CompanyName = cwSection["CompanyName"];
            client.SiteUrl = cwSection["SiteUrl"];
            client.PublicKey = cwSection["PublicKey"];
            client.PrivateKey = cwSection["PrivateKey"];
            client.ClientId = cwSection["ClientId"];

            return client;
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetContactByEmail()
        {
            ManageApiClient client = GetApiClient();

            List<Contact> contacts = await client.GetContactByCommunication("chris@thoughtpost.com");

            Assert.IsTrue(contacts.Count == 1);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetContactByPhone()
        {
            ManageApiClient client = GetApiClient();

            List<Contact> contacts = await client.GetContactByCommunication("7042025210");

            Assert.IsTrue(contacts.Count == 1);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task GetTicket()
        {
            ManageApiClient client = GetApiClient();

            Ticket ticket = await client.GetTicket(542);

            Assert.IsTrue(ticket.Id == 542);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task CreateTicket()
        {
            ManageApiClient client = GetApiClient();

            Ticket ticket = await client.CreateTicket(
                "IT Nation Ticket", 
                "IndigoStrawberryCo", 
                SeverityEnum.High);

            Assert.IsTrue(ticket != null);
        }

    }
}
