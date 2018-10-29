using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TygaSoft.Model;
using System.ServiceModel;

namespace TygaSoft.WCFClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //step 1: create an endpoint address and an instance of the wcf client.
            PDAOrderClient client = new PDAOrderClient();
            string[] users = client.GetAllUsers();

            ////// Step 2: Call the service operations.

            //int k = client.Insert("取货", DateTime.Now.ToString("yyyy-MM-ddHHmmss"), 1, DateTime.Now);
            //model.OrderCode = DateTime.Now.ToString("yyyy-MM-ddHHmmss");
            //int i = client.Insert(model);

            //Step 3: Closing the client gracefully closes the connection and cleans up resources.
            //client.Close();


            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
