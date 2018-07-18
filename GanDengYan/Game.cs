using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanDengYan
{
    struct Result
    {
        public int winner;
        public int numCards;
        public int numBomb;
    }

    class Game
    {
        private const int CARD_BIT_COUNT = 5;
        public uint DoubleCardCount { get; set; }
        public List<Result> Results { get; private set; }
        public List<Player> Players { get; private set; }

        public void AddRecord(int numBomb, List<int> numCards)
        {
            if (numCards.Count != Players.Count)
                return;

            int numCardsBitFormat = 0;
            int winnerScore = 0;
            int winner = 0;
            for (int i = 0; i < Players.Count; ++i)
            {
                int score = ComputeScore(numBomb, numCards[i]);

                if (score == 0)
                {
                    winner = i;
                    continue;
                }

                winnerScore -= score;
                numCardsBitFormat = (numCardsBitFormat << CARD_BIT_COUNT) | numCards[i];
                Players[i].ScoreRecord.Add(score);
            }
            Players[winner].ScoreRecord.Add(winnerScore);

            Result result = new Result();
            result.winner = winner;
            result.numCards = numCardsBitFormat;
            result.numBomb = numBomb;
            Results.Add(result);
        }

        private int ComputeScore(int numBomb, int numCard)
        {
            if (numCard >= DoubleCardCount)
                return -numCard * 2 * (numBomb + 1);
            return -numCard * (numBomb + 1);
        }
    }
}
