using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using System.Data.SqlClient;

namespace WebApplication1.Controllers
{
    public class DataController : Controller
    {   
        
        public ActionResult Index()
        {
            return View();
        }
        [Route("data/sort")]
        public ActionResult Sort(String response, bool ascending = false)
        {
            int initialTime = DateTime.UtcNow.Millisecond;
            response = response.Trim(); // This might not be necessary. Keeping it for safeguarding purposes.
            Array responseArray = response.Split(" ", StringSplitOptions.RemoveEmptyEntries); // the way to remove double entries.
            List<int> intList = new List<int>();
            List<String> sortedList = new List<string>();
            foreach (string entry in responseArray)
            {
                if (int.TryParse(entry, out int value)) // this ensures we only get numbers. We can't get empty values, but someone
                { // could quite easily put a string such as "AAAA A AA A A A A A A" and ask that we sort it.
                    // Admittedly, that could be accomplished by the localeCompare() function.
                    intList.Add(value);
                }
            }
            if (intList.Count == 0)
            {
                Data lackOfInt = new Data() { Response = "It appears you didn't enter any numbers." };
                return View(lackOfInt);
            }
            if (ascending)
            {
                bool sorted = false;
                while (!sorted)
                {
                    int sortCounter = 0;
                    //this ensures that we only get integers.
                    for (int i = 0; i < intList.Count - 1; i++)
                    {
                        if (intList[i] > intList[i + 1])
                        {
                            int holderValue1 = intList[i];
                            int holderValue2 = intList[i + 1];
                            intList[i] = holderValue2;
                            intList[i + 1] = holderValue1;
                            sortedList.Add("{ \"response:\" \"Changed value " + holderValue1 + " With " + holderValue2 + "\"}");
                        }
                        else
                        {
                            sortCounter++;
                        }
                        if (sortCounter == intList.Count -1)
                        {
                            sorted = true;
                        }
                    }
                }
            }
            if (!ascending)
            {
                bool sorted = false;
                while (!sorted)
                {
                    int sortCounter = 0;
                    //this ensures that we only get integers.
                    for (int i = 0; i < intList.Count - 1; i++)
                    {
                        if (intList[i] < intList[i + 1])
                        {
                            int holderValue1 = intList[i];
                            int holderValue2 = intList[i + 1];
                            intList[i] = holderValue2;
                            intList[i + 1] = holderValue1;
                            sortedList.Add("{ \"response:\" \"Changed value " + holderValue1 + " With " + holderValue2 + "\"}");
                        }
                        else
                        {
                            sortCounter++;
                        }
                        if (sortCounter == intList.Count - 1) // It's safe to assume it's sorted if there was no sort operations required throughout the list.
                        {
                            sorted = true;
                        }
                    }
                }
            }
            String sortedResponse = "";
            foreach(int number in intList)
            {
                sortedResponse = sortedResponse + number + " "; // sorts the response list so that it has spaces. 
            }
            string listOfSorts = string.Join(Environment.NewLine, sortedList);
            string JsonString = "{" + listOfSorts + "}"; // this is preparing the JSON String for exporting purposes.
            int finishedTime = DateTime.UtcNow.Millisecond; // this should be executed after the sorting. 
            int timeDifference = finishedTime - initialTime; 
            Data responseData = new Data() { Response = response, IsAscending = ascending, SortedResponse = sortedResponse, SortList = JsonString, TimeTaken = timeDifference };
            string connectionString; 
            connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=model;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
            String query = "INSERT INTO DataTable (sortedResponse, sortedList, timeTaken) VALUES (@sortedResponse, @sortedList, @timeTaken)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@sortedResponse", sortedResponse); // this method prevents SQL injection
                command.Parameters.AddWithValue("@sortedList", listOfSorts);  // by not attempting to parse the value like a typical SQL query.
                command.Parameters.AddWithValue("@timeTaken", timeDifference);

                connection.Open();
                int result = command.ExecuteNonQuery();

                // Not entirely sure where to hand this error off to. Does the user need to see this? Then it should be added to View.
                if (result < 0)
                    Console.WriteLine("Error inserting data into Database!");
            }
        }
            return View(responseData);
        }
       
    }
}
