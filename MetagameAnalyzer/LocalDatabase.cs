using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MetagameAnalyzer
{
    class LocalDatabase
    {
        private Dictionary<string, int> decks;
        private List<Card> cards;
        private List<string> cardNameList = new List<string>();
        private List<string> deckNameList = new List<string>();
        private List<CustomDeck> customDecks = new List<CustomDeck>(); 
        public LocalDatabase(Dictionary<string, int> decks, List<Card> cards)
        {
            this.cards = cards;
            this.decks = decks;
            foreach (Card card in this.cards)
            {
                this.cardNameList.Add(card.name);
            }
        }
        public LocalDatabase(Dictionary<string, int> decks, List<Card> cards, List<CustomDeck> cdecks)
        {
            this.cards = cards;
            this.decks = decks;
            this.customDecks = cdecks;
            foreach (Card card in this.cards)
            {
                this.cardNameList.Add(card.name);
            }
        }
        public List<Card> GetCards()
        {
            return this.cards;
        }
        public Dictionary<string,int> GetDecks()
        {
            return this.decks;
        }
        public List<string> GetCardNames()
        {
            return this.cardNameList;
        }
        public List<CustomDeck> GetCustomDecks()
        {
            return this.customDecks;
        }
        public List<string> GetDeckNames()
        {
            return this.deckNameList;
        }
        public bool LoadCard(string cardName)
        {
            bool isExists = System.IO.Directory.Exists(@"C:\Users\Public\Documents\MetagameAnalyzer");

            if (!isExists)
            {
                Console.WriteLine("MetagameAnalyzer folder not found");
                return false;
            }
            else if (!File.Exists(@"C:\Users\Public\Documents\MetagameAnalyzer\Cards.txt"))
            {
                Console.WriteLine("Cards.txt file not found");
                return false;
            }
            else
            {
                if (File.ReadAllText(@"C:\Users\Public\Documents\MetagameAnalyzer\Cards.txt").Contains(cardName))
                {
                    if (!cardNameList.Contains(cardName))
                    {
                        bool first = true;
                        string name = "Error";
                        List<string> liveList = new List<string>();
                        foreach (string line in File.ReadLines(@"C:\Users\Public\Documents\MetagameAnalyzer\Cards.txt").SkipWhile(line => !line.Contains(cardName)).TakeWhile(line => !line.Contains("*")))
                        {
                            if (first)
                            {
                                name = line;
                                first = false;
                            }
                            else
                            {
                                liveList.Add(line);
                            }
                        }

                        this.cards.Add(new Card(name, liveList, this.decks));
                        this.cardNameList.Add(name);
                        return true;
                    }
                    else
                    {
                        bool first = true;
                        List<string> liveList = new List<string>();
                        foreach (string line in File.ReadLines(@"C:\Users\Public\Documents\MetagameAnalyzer\Cards.txt").SkipWhile(line => !line.Contains(cardName)).TakeWhile(line => !line.Contains("*")))
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                liveList.Add(line);
                            }
                        }
                        this.cards[this.cardNameList.IndexOf(cardName)].liveList = liveList;
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        public bool LoadDeck(string deckName)
        {
            bool isExists = System.IO.Directory.Exists(@"C:\Users\Public\Documents\MetagameAnalyzer");

            if (!isExists)
            {
                Console.WriteLine("MetagameAnalyzer folder not found");
                return false;
            }
            else if (!File.Exists(@"C:\Users\Public\Documents\MetagameAnalyzer\Decks.txt"))
            {
                Console.WriteLine("Cards.txt file not found");
                return false;
            }
            else
            {
                if (File.ReadAllText(@"C:\Users\Public\Documents\MetagameAnalyzer\Decks.txt").Contains(deckName))
                {
                    if (!deckNameList.Contains(deckName))
                    {
                        bool first = true;
                        string name = "Error";
                        List<int> quantityList = new List<int>();
                        List<string> cardList = new List<string>();
                        foreach (string line in File.ReadLines(@"C:\Users\Public\Documents\MetagameAnalyzer\Decks.txt").SkipWhile(line => !line.Contains(deckName)).TakeWhile(line => !line.Contains("*")))
                        {
                            if (first)
                            {
                                name = line;
                                first = false;
                            }
                            else
                            {
                                string quantity = "";
                                foreach (char n in line)
                                {
                                    if (char.IsDigit(n))
                                    {
                                        quantity += n;
                                    }
                                    else if (n==' '){}
                                    else
                                    {
                                        cardList.Add(line.Substring(line.IndexOf(n)));
                                        break;
                                    }
                                }
                                quantityList.Add(int.Parse(quantity));
                            }
                        }
                        List<Card> cardList1 = new List<Card>();
                        foreach(string s in cardList)
                        {
                            if (this.GetCardNames().Contains(s))
                            {
                                cardList1.Add(this.cards[this.cardNameList.IndexOf(s)]);
                            }
                            else if (this.LoadCard(s))
                            {
                                cardList1.Add(this.cards[this.cardNameList.IndexOf(s)]);
                            }
                            else
                            {
                                Console.WriteLine("Card " + s + " not found. Please enter information for it.");
                                this.SaveCard(s);
                                this.LoadCard(s);
                                cardList1.Add(this.cards[this.cardNameList.IndexOf(s)]);
                            }
                        }
                        this.customDecks.Add(new CustomDeck(name, cardList1, quantityList, this));
                        this.deckNameList.Add(name);
                        return true;
                    }
                        //left off fixing this part
                    else
                    {
                        bool first = true;
                        List<int> quantityList = new List<int>();
                        List<string> cardList = new List<string>();
                        foreach (string line in File.ReadLines(@"C:\Users\Public\Documents\MetagameAnalyzer\Decks.txt").SkipWhile(line => !line.Contains(deckName)).TakeWhile(line => !line.Contains("*")))
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                string quantity = "";
                                foreach (char n in line)
                                {
                                    if (char.IsDigit(n))
                                    {
                                        quantity += n;
                                    }
                                    else if (n == ' ') { }
                                    else
                                    {
                                        cardList.Add(line.Substring(line.IndexOf(n)));
                                        break;
                                    }
                                }
                                quantityList.Add(int.Parse(quantity));
                            }
                        }
                        List<Card> cardList1 = new List<Card>();
                        foreach (string s in cardList)
                        {
                            if (this.GetCardNames().Contains(s))
                            {
                                cardList1.Add(this.cards[this.cardNameList.IndexOf(s)]);
                            }
                            else if (this.LoadCard(s))
                            {
                                cardList1.Add(this.cards[this.cardNameList.IndexOf(s)]);
                            }
                            else
                            {
                                this.SaveCard(s);
                                this.LoadCard(s);
                                cardList1.Add(this.cards[this.cardNameList.IndexOf(s)]);
                            }
                        }
                        this.customDecks.Add(new CustomDeck(deckName, cardList1, quantityList, this));
                        this.cardNameList.Add(deckName);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        public void SaveCard(string cardName)
        {
            if (!System.IO.Directory.Exists(@"C:\Users\Public\Documents\MetagameAnalyzer"))
            {
                System.IO.Directory.CreateDirectory(@"C:\Users\Public\Documents\MetagameAnalyzer");
            }
            if (!File.Exists(@"C:\Users\Public\Documents\MetagameAnalyzer\Cards.txt"))
            {
                File.Create(@"C:\Users\Public\Documents\MetagameAnalyzer\Cards.txt");
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\Documents\MetagameAnalyzer\Cards.txt", true))
            {
                file.WriteLine(cardName);
                bool enter = true;
                while (enter)
                {
                    Console.WriteLine("Enter the name of a deck that " + cardName + " is live against.");
                    Console.WriteLine();
                    Console.WriteLine("Enter '*' to submit.");
                    Console.WriteLine();
                    string input = Console.ReadLine();
                    file.WriteLine(input);
                    if (input == "*")
                    {
                        enter = false;
                    }
                    Console.Clear();
                }
                Console.WriteLine(cardName + " successfully added to card database.");
            }
        }
        public void SaveDeck(string deckName)
        {
            if (!System.IO.Directory.Exists(@"C:\Users\Public\Documents\MetagameAnalyzer"))
            {
                System.IO.Directory.CreateDirectory(@"C:\Users\Public\Documents\MetagameAnalyzer");
            }
            if (!File.Exists(@"C:\Users\Public\Documents\MetagameAnalyzer\Decks.txt"))
            {
                File.Create(@"C:\Users\Public\Documents\MetagameAnalyzer\Decks.txt");
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\Documents\MetagameAnalyzer\Decks.txt", true))
            {
                file.WriteLine(deckName);
                bool enter = true;
                while (enter)
                {
                    Console.WriteLine("Enter the name of a card that" + deckName + " contains, preceded by the quantity.");
                    Console.WriteLine();
                    Console.WriteLine("Enter '*' to submit.");
                    Console.WriteLine();
                    string input = Console.ReadLine();
                    file.WriteLine(input);
                    if (input == "*")
                    {
                        enter = false;
                    }
                    Console.Clear();
                }
                Console.WriteLine(deckName + " successfully added to deck database.");
            }
        }
    }
}