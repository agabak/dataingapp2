using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var companies = new List<Company>
            {
                new Company { Id = "Fue",  Name = "Agaba"},
                new Company { Id = "Fue1",  Name = "Agaba1"},
                new Company { Id = "Fue2",  Name = "Agaba3"},
                new Company { Id = "Fue3",  Name = "Agaba4"},
                new Company { Id = "Fue4",  Name = "Agaba4"}

            };

            var parties = new List<Party>
            {  new Party { ReferenceNumberType="Fue2", ReferenceNumberValue ="Working-1"},
               new Party { ReferenceNumberType="Fue3", ReferenceNumberValue = "Working-2" },
               new Party { ReferenceNumberType="Fue4", ReferenceNumberValue = "Working-3"}
            };

            var  dlistdictionary = parties.ToDictionary(x => x.ReferenceNumberType, x => x.ReferenceNumberValue);

            foreach(var item in companies)
            {
                if(dlistdictionary.ContainsKey(item.Id))
                {
                    item.Value = dlistdictionary[item.Id];
                }
            }

            foreach(var item in companies)
            {
                Console.WriteLine(item.Id + " - " + item.Name + " - "  + item.Value);
            }

            Console.ReadLine();
        }
    } 
}