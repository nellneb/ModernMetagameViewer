using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using System.Windows.Forms;
namespace MetagameAnalyzer
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window());
        }
        public static void Start(string[] args)
        {

            Dictionary<string, int> decks = Get_Decks();           
            List<Card> cardList = new List<Card> {};
            LocalDatabase Database = new LocalDatabase(decks, cardList);
            Console.WriteLine("Typing 'decks' will give the current metagame breakdown.");
            Console.WriteLine();
            Console.WriteLine("Typing 'sideboard' will give the current percentage of live matchups for common sideboard cards.");
            Console.WriteLine();
            Console.WriteLine("Typing the name of a common sideboard card with a dash (-) in front of it will give data on it's use and effectiveness.");
            Console.WriteLine();
            Console.WriteLine("Typing the name of a deck with a period (.) in front of it will give data on it's current position in the metagame.");
            Console.WriteLine();
            Console.WriteLine("Typing 'quit' will exit the program");
            bool run = true;
            while (run)
            {
                string input = Console.ReadLine();
                Console.Clear();
                if (input=="decks")
                {
                    PrintDecks(Database.GetDecks());
                }
                else if (input[0]=='-')
                {
                    if (Database.GetCardNames().Contains(input.Substring(1)))
                    {
                        Database.GetCards()[Database.GetCardNames().IndexOf(input.Substring(1))].giveData();
                    }
                    else if (Database.LoadCard(input.Substring(1)))
                    {
                        Database.GetCards()[Database.GetCardNames().IndexOf(input.Substring(1))].giveData();
                    }
                    else
                    {
                        bool choosing = true;
                        bool yes = true;
                        while(choosing)
                        {
                            Console.Clear();
                            Console.WriteLine("Card not found. Would you like to add an entry for it?");
                            if(yes)
                            {
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.WriteLine("Yes");
                                Console.ResetColor();
                                Console.WriteLine("No");
                            }
                            else
                            {
                                Console.WriteLine("Yes");
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.WriteLine("No");
                                Console.ResetColor();
                            }
                            var keypress = Console.ReadKey(false);
                            if (keypress.Key.ToString()==("Enter"))
                            {
                                if (yes)
                                {
                                    Console.Clear();
                                    Database.SaveCard(input.Substring(1));
                                }
                                else
                                {
                                    choosing = false;
                                    break;
                                }
                                choosing = false;
                            }
                            else if (keypress.Key.ToString()=="UpArrow")
                            {
                                yes = !yes;
                            }
                            else if (keypress.Key.ToString()=="DownArrow")
                            {
                                yes = !yes;
                            }
                        }
                    }
                }
                else if (input[0] == '.')
                {
                    if (Database.GetDeckNames().Contains(input.Substring(1)))
                    {
                        Database.GetCustomDecks()[Database.GetDeckNames().IndexOf(input.Substring(1))].printMatchups();
                    }
                    else if (Database.LoadDeck(input.Substring(1)))
                    {
                        Console.WriteLine(Database.GetDeckNames().IndexOf(input.Substring(1)));
                        Database.GetCustomDecks()[Database.GetDeckNames().IndexOf(input.Substring(1))].printMatchups();
                    }
                    else
                    {
                        bool choosing = true;
                        bool yes = true;
                        while (choosing)
                        {
                            Console.Clear();
                            Console.WriteLine("Deck not found. Would you like to add an entry for it?");
                            if (yes)
                            {
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.WriteLine("Yes");
                                Console.ResetColor();
                                Console.WriteLine("No");
                            }
                            else
                            {
                                Console.WriteLine("Yes");
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.WriteLine("No");
                                Console.ResetColor();
                            }
                            var keypress = Console.ReadKey(false);
                            if (keypress.Key.ToString() == ("Enter"))
                            {
                                if (yes)
                                {
                                    Console.Clear();
                                    Database.SaveDeck(input.Substring(1));
                                }
                                choosing = false;
                            }
                            else if (keypress.Key.ToString() == "UpArrow")
                            {
                                yes = !yes;
                            }
                            else if (keypress.Key.ToString() == "DownArrow")
                            {
                                yes = !yes;
                            }
                        }
                    }
                }
                else if(input=="sideboard")
                {
                    PrintCards(Database.GetCards());
                }
                else if(input == "quit")
                {
                    run = false;
                }
                else
                {
                    Console.WriteLine("Typing 'decks' will give the current metagame breakdown.");
                    Console.WriteLine();
                    Console.WriteLine("Typing 'sideboard' will give the current percentage of live matchups for common sideboard cards.");
                    Console.WriteLine();
                    Console.WriteLine("Typing the name of a common sideboard card with a dash (-) in front of it will give data on it's use and effectiveness.");
                    Console.WriteLine();
                    Console.WriteLine("Typing 'quit' will exit the program");
                }
            }
        }
        protected static Dictionary<string, int> Get_Decks()
        {
            string Url = "http://www.mtgtop8.com/format?f=MO";
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(Url);

            HtmlNodeCollection nodesA = doc.DocumentNode.SelectNodes("//table[@width='100%']");
            HtmlNodeCollection nodesB = nodesA[0].ChildNodes;
            HtmlNodeCollection nodesC = nodesB[1].ChildNodes;
            HtmlNodeCollection nodesD = nodesC[1].ChildNodes;
            HtmlNodeCollection nodes = nodesD[1].ChildNodes;
            HtmlNodeCollection deckNodes= new HtmlNodeCollection(nodes[11]);
            for (int n = 11; n<nodes.Count; n++)
            {
                if(nodes[n].InnerHtml.Contains("meta"))
                {
                    deckNodes.Add(nodes[n]);
                }
            }
            Dictionary<string, int> decks = new Dictionary<string,int>();
            foreach (HtmlNode node in deckNodes)
            {
                decks.Add(node.ChildNodes[1].InnerText, int.Parse(node.ChildNodes[5].InnerText.Substring(0,node.ChildNodes[5].InnerText.Length-1)));
            }
            return decks;
        }
        static void PrintDecks(Dictionary<string, int> decks)
        {
            foreach (KeyValuePair<string, int> deck in decks)
            {
                Console.Write(deck.Key);
                Console.WriteLine(": " + deck.Value + "%");
            }
        }
        static void PrintCards(List<Card> cardList)
        {
            foreach (Card card in cardList)
            {
                Console.WriteLine(card.name + ": " + card.livePercent + "%");
            }
        }
    }
}
