using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetagameAnalyzer
{
    class CustomDeck
    {
        public string name;
        public List<Card> cards;
        public List<int> quantities;
        public int deckSize;
        private Dictionary<string, float> matchups = new Dictionary<string,float>();
        public float effectiveness;
        public CustomDeck(string name, List<Card> cards, List<int> quantities, LocalDatabase database)
        {
            this.name = name;
            this.cards = cards;
            this.quantities = quantities;
            this.deckSize = 0;
            foreach (int n in quantities)
            {
                this.deckSize += n;
            }
            foreach (KeyValuePair<string,int> deck in database.GetDecks())
            {
                float effectiveness = 0.0F;
                foreach(Card card in cards)
                {
                    if (card.liveList.Contains(deck.Key))
                    {
                        effectiveness+=quantities[cards.IndexOf(card)];
                    }
                }
                effectiveness /= this.deckSize;
                this.matchups.Add(deck.Key, effectiveness);
            }
            this.effectiveness = 0;
            foreach (KeyValuePair<string, float> matchup in this.matchups)
            {
                //fix this shit
                this.effectiveness += matchup.Value*database.GetDecks()[matchup.Key];
            }
        }
        public void printMatchups()
        {
            foreach (KeyValuePair<string,float> matchup in this.matchups)
            {
                Console.WriteLine(matchup.Key + ": " + matchup.Value*100+"%");
            }
            Console.WriteLine("Overall effectiveness: "+this.effectiveness);
        }
    }
}
