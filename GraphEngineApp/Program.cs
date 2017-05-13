using System;
using System.Collections.Generic;
using GraphEngine;
using Trinity;
using Trinity.Storage;

namespace GraphEngineApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TrinityConfig.CurrentRunningMode = RunningMode.Embedded;
            Global.LocalStorage.LoadStorage();
            int count = 0;
            foreach (var item in Global.LocalStorage.Movie_Accessor_Selector())
            {
                count++;
                //Console.WriteLine(item.Name);
            }
            Console.WriteLine(count);

            List<long> name_ids = Index.Person_Name_SubstringQuery("刘德华");
            Console.WriteLine(name_ids.Count);
            
            foreach (var cellid in name_ids)
            {
                using (var person = Global.LocalStorage.UsePerson(cellid))
                {
                    Console.WriteLine(person.Name + "||" + person.CellID);
                    foreach (var item in person.Act)
                    {
                        using (var movie = Global.LocalStorage.UseMovie(item))
                        { Console.WriteLine(movie.Name); }
                    }
                    Console.WriteLine("===========================");
                    foreach (var item in person.Direct)
                    {
                        using (var movie = Global.LocalStorage.UseMovie(item))
                        { Console.WriteLine(movie.Name); }
                    }
                }
            }
            Console.WriteLine("===========================");
            

            name_ids = Index.Person_Name_SubstringQuery("王宝强");
            Console.WriteLine(name_ids.Count);
            foreach (var cellid in name_ids)
            {
                using (var person = Global.LocalStorage.UsePerson(cellid))
                {
                    Console.WriteLine(person.Name + "||" + person.CellID);
                    foreach (var item in person.Act)
                    {
                        using (var movie = Global.LocalStorage.UseMovie(item))
                        { Console.WriteLine(movie.Name); }
                    }
                    Console.WriteLine("===========================");
                    foreach (var item in person.Direct)
                    {
                        using (var movie = Global.LocalStorage.UseMovie(item))
                        { Console.WriteLine(movie.Name); }
                    }
                }
            }
            Console.WriteLine("===========================");
        }
    }
}
