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

            Console.WriteLine("The character list: ");
            foreach (var character in Global.LocalStorage.Character_Accessor_Selector())
            {
                Console.WriteLine(character.Name);
            }
            Console.WriteLine("\n=============" + string.Join(" ", Index.SubstringQuery(Index.Character.Name, "华是")));
            Console.WriteLine("\n=============" + string.Join(" ", Index.Character_Name_SubstringQuery("刘德")));
            Console.WriteLine();

            Console.WriteLine("The performer list: ");
            foreach (var performer in Global.LocalStorage.Performer_Accessor_Selector())
            {
                Console.WriteLine(performer.Name);
            }
            Console.WriteLine();

            Console.WriteLine("The movie list: ");
            foreach (var movie in Global.LocalStorage.Movie_Accessor_Selector())
            {
                Console.WriteLine(movie.Name);
            }
            Console.WriteLine("\n=============" + string.Join(" ", Index.Movie_Name_SubstringQuery("传奇")));
            Console.WriteLine();

            long spouse_id = -1;
            var lm = Global.LocalStorage.LoadCharacter(123456);
            Console.WriteLine(lm.CellID);
            using (var cm = Global.LocalStorage.UseCharacter(123456))
            {
                if (cm.Married)
                    spouse_id = cm.Spouse;
            }

            Console.Write("The spouse of Monica is: ");

            using (var cm = Global.LocalStorage.UseCharacter(spouse_id))
            {
                Console.WriteLine(cm.Name);
            }
            
        }
    }
}
