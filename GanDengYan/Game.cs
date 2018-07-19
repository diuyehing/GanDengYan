using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanDengYan
{
    [Serializable]
    struct Game
    {
        public DateTime dateTime;
        public int winner;
        public int numCards;
        public int numBomb;
    }

    struct ScoreResult
    {
        public DateTime dateTime;
        public int numBomb;
        public int score0;
        public int score1;
        public int score2;
        public int score3;
        public int score4;
        public int score5;
    }

    class GameWorld
    {
        private const int CARD_BIT_COUNT = 5;
        private const int SCORE_BIT_COUNT = 10;

        public int FAN { get; set; }
        public List<Game> Games { get; private set; }
        public List<Player> Players { get; private set; }
        public List<ScoreResult> ScoreResults { get; private set; }

        public GameWorld(List<string> playerNames, int fan)
        {
            FAN = fan;
            Games = new List<Game>();
            Players = new List<Player>();
            ScoreResults = new List<ScoreResult>();

            for (int i = 0; i < playerNames.Count; ++i)
            {
                Player player = new Player();
                player.Name = playerNames[i];
                player.ID = i;
                player.Score = 0;
            }
        }

        public void AddGame(int numBomb, List<int> numCards)
        {
            if (numCards.Count != Players.Count)
                return;

            Game game = new Game();
            game.dateTime = DateTime.Now;
            game.numBomb = numBomb;
            game.winner = 0;
            game.numCards = 0;

            for (int i = 0; i < Players.Count; ++i)
            {
                game.numCards |= numCards[i] << (i * CARD_BIT_COUNT);
                if (numCards[i] == 0)
                    game.winner = i;
            }

            AddGame(game);
        }

        private void AddGame(Game game)
        {
            ScoreResult scoreResult = new ScoreResult();
            scoreResult.dateTime = game.dateTime;
            scoreResult.numBomb = game.numBomb;

            int winnerScore = 0;
            for (int i = 0; i < Players.Count; ++i)
            {
                int score = ComputeScore(game.numBomb, GetNumCard(game.numCards, i));
                winnerScore -= score;
                scoreResult.GetType().GetField(string.Format("score{0}", i)).SetValue(scoreResult, score);
                Players[i].Score += score;
            }
            scoreResult.GetType().GetField(string.Format("score{0}", game.winner)).SetValue(scoreResult, winnerScore);
            Players[game.winner].Score += winnerScore;

            Games.Add(game);
            ScoreResults.Add(scoreResult);
        }

        public void RemoveGame(int index)
        {
            Game game = Games[index];
            int winnerScore = 0;
            for (int i = 0; i < Players.Count; ++i)
            {
                int score = ComputeScore(game.numBomb, GetNumCard(game.numCards, i));
                winnerScore -= score;
                Players[i].Score -= score;
            }
            Players[game.winner].Score -= winnerScore;

            Games.RemoveAt(index);
            ScoreResults.RemoveAt(index);
        }

        private int ComputeScore(int numBomb, int numCard)
        {
            if (numCard >= FAN)
                return -numCard * 2 * (numBomb + 1);
            return -numCard * (numBomb + 1);
        }

        private int GetNumCard(int numCards, int index)
        {
            return (numCards >> (index * CARD_BIT_COUNT)) & ((1 << CARD_BIT_COUNT) - 1);
        }
    }
}
