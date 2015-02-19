using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetagameAnalyzer
{
    class Card
    {
        public string name;
        public int livePercent;
        public List<string> liveList;
        public Card(string name, List<string> liveList, Dictionary<string, int> deckList)
        {
            this.name = name;
            int percent = 0;
            this.liveList = liveList;
            foreach (KeyValuePair<string, int> deck in deckList)
            {
                if(this.liveList.Contains(deck.Key))
                {
                    percent += deck.Value;
                }
            }
            this.livePercent = percent;
        }
        public void giveData()
        {
            Console.WriteLine(this.name + " is live against:");
            foreach (string deck in this.liveList)
            {
                Console.WriteLine("  "+deck);
            }
            Console.WriteLine("This constitutes " + this.livePercent + "% of the meta.");
        }
    }
}
