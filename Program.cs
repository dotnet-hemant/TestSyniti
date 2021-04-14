using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace TestJson
{
    class Program
    {
        const string ValidZip = @"^\d{5}(?:[-\s]\d{4})?$";
        const string FileName = "data.json";
        /// <summary>
        /// Main Entry Program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                var jsonData = GetData(FileName);
                //Convert to object for processing
                var demographicList = jsonData.Select(data => data.ToObject<Demographic>());

                //duplicates incldues all blnak/null - and multiple row of duplicates
                var duplicates = from d1 in demographicList
                                 join d2 in demographicList
                                 on new { d1.Name, d1.Address, d1.Zip } equals new { d2.Name, d2.Address, d2.Zip }
                                 where d1.ID != d2.ID
                                // orderby d1.Name, d1.Address, d1.Zip
                                 select d1.ID;

                //get unique duplicates 
                var finalDuplicates = duplicates.Distinct();
                //print duplicates
                //  Console.WriteLine("Duplicate ID");
                foreach (var id in finalDuplicates)
                {
                    Console.WriteLine(id);
                }

                //  Console.WriteLine("Invalid Id");
                //ignore already printed
                var invalidList = duplicates.Except(duplicates);

                Parallel.ForEach(jsonData, (item) =>
                {
                    var demographic = item.ToObject<Demographic>();

                    if (string.IsNullOrWhiteSpace(demographic.Name) || string.IsNullOrWhiteSpace(demographic.Address)
                    || string.IsNullOrWhiteSpace(demographic.Zip) || !System.Text.RegularExpressions.Regex.IsMatch(demographic.Zip, ValidZip))
                    {
                        Console.WriteLine(demographic.ID);
                    }
                });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error occured" + ex.Message);
            }

            Console.ReadLine();
        }


        /// <summary>
        /// Get Json data
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static JArray GetData(string filename)
        {
            return JArray.Parse(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename)));
        }


    }

}
