using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using GraphEngine;

namespace GraphEngineDataImport
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //AddToyData();
            MovieEntityImport movie_entity_import = new MovieEntityImport();
            string filename = @"D:\MovieDomain\GraphEngine\input\test_movie.csv";
            movie_entity_import.ImportMovie(filename);
        }

        static void AddToyData()
        {
            TrinityConfig.CurrentRunningMode = RunningMode.Embedded;

            // Characters
            Character Rachel = new Character(Name: "马德华是猪八戒", Gender: 0, Married: true);
            Character Monica = new Character(Name: "刘德华", Gender: 0, Married: true);
            Monica.CellID = 123456;
            Character Phoebe = new Character(Name: "Phoebe Buffay", Gender: 0, Married: true);
            Character Joey = new Character(Name: "Joey Tribbiani", Gender: 1, Married: false);
            Character Chandler = new Character(Name: "Chandler Bing", Gender: 1, Married: true);
            Character Ross = new Character(Name: "Ross Geller", Gender: 1, Married: true);

            // Performers
            Performer Jennifer = new Performer(Name: "Jennifer Aniston", Age: 43, Characters: new List<long>());
            Performer Courteney = new Performer(Name: "Courteney Cox", Age: 48, Characters: new List<long>());
            Performer Lisa = new Performer(Name: "Lisa Kudrow", Age: 49, Characters: new List<long>());
            Performer Matt = new Performer(Name: "Matt Le Blanc", Age: 45, Characters: new List<long>());
            Performer Matthew = new Performer(Name: "Matthew Perry", Age: 43, Characters: new List<long>());
            Performer David = new Performer(Name: "David Schwimmer", Age: 45, Characters: new List<long>());

            // Portrayal Relationship
            Rachel.Performer = Jennifer.CellID;
            Jennifer.Characters.Add(Rachel.CellID);

            Monica.Performer = Courteney.CellID;
            Courteney.Characters.Add(Monica.CellID);

            Phoebe.Performer = Lisa.CellID;
            Lisa.Characters.Add(Phoebe.CellID);

            Joey.Performer = Matt.CellID;
            Matt.Characters.Add(Joey.CellID);

            Chandler.Performer = Matthew.CellID;
            Matthew.Characters.Add(Chandler.CellID);

            Ross.Performer = David.CellID;
            David.Characters.Add(Ross.CellID);

            // Marriage relationship
            Monica.Spouse = Chandler.CellID;
            Chandler.Spouse = Monica.CellID;

            Rachel.Spouse = Ross.CellID;
            Ross.Spouse = Rachel.CellID;

            // Friendship
            Friendship friend_ship = new Friendship(new List<long>());
            friend_ship.friends.Add(Rachel.CellID);
            friend_ship.friends.Add(Monica.CellID);
            friend_ship.friends.Add(Phoebe.CellID);
            friend_ship.friends.Add(Joey.CellID);
            friend_ship.friends.Add(Chandler.CellID);
            friend_ship.friends.Add(Ross.CellID);
            Global.LocalStorage.SaveFriendship(friend_ship);

            // Save Runtime cells to Trinity memory storage
            Global.LocalStorage.SavePerformer(Jennifer);
            Global.LocalStorage.SavePerformer(Courteney);
            Global.LocalStorage.SavePerformer(Lisa);
            Global.LocalStorage.SavePerformer(Matt);
            Global.LocalStorage.SavePerformer(Matthew);
            Global.LocalStorage.SavePerformer(David);

            Global.LocalStorage.SaveCharacter(Rachel);
            Global.LocalStorage.SaveCharacter(Monica);
            Global.LocalStorage.SaveCharacter(Phoebe);
            Global.LocalStorage.SaveCharacter(Joey);
            Global.LocalStorage.SaveCharacter(Chandler);
            Global.LocalStorage.SaveCharacter(Ross);

            Global.LocalStorage.SaveStorage();
        }
    }
}