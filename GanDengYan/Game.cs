using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GanDengYan
{
    [Serializable]
    struct Game
    {
        public DateTime Time { get; set; }
        public int Winner { get; set; }
        public int NumCards { get; set; }
        public int NumBomb { get; set; }
    }

    [Serializable]
    struct ScoreResult
    {
        public DateTime Time { get; set; }
        public int NumBomb { get; set; }
        public int Score0 { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public int Score3 { get; set; }
        public int Score4 { get; set; }
        public int Score5 { get; set; }
    }

    [Serializable]
    struct Dinner
    {
        public DateTime Time { get; set; }
        public int TotalCost { get; set; }
        public int Cost0 { get; set; }
        public int Cost1 { get; set; }
        public int Cost2 { get; set; }
        public int Cost3 { get; set; }
        public int Cost4 { get; set; }
        public int Cost5 { get; set; }
    }

    [Serializable]
    class GameWorld
    {
        private const int CARD_BIT_COUNT = 5;
        private const int SCORE_BIT_COUNT = 10;

        public int FAN { get; set; }
        public List<Game> Games { get; private set; }
        public List<Player> Players { get; private set; }
        public List<ScoreResult> ScoreResults { get; private set; }
        public List<Dinner> Dinners { get; private set; }

        public GameWorld(List<string> playerNames, int fan)
        {
            FAN = fan;
            Games = new List<Game>();
            Players = new List<Player>();
            ScoreResults = new List<ScoreResult>();
            Dinners = new List<Dinner>();

            for (int i = 0; i < playerNames.Count; ++i)
            {
                Player player = new Player();
                player.Name = playerNames[i];
                player.ID = i;
                player.Score = 0;
                Players.Add(player);
            }
        }

        public void AddGame(int numBomb, List<int> numCards)
        {
            if (numCards.Count != Players.Count)
                return;

            Game game = new Game();
            game.Time = DateTime.Now;
            game.NumBomb = numBomb;
            game.Winner = 0;
            game.NumCards = 0;

            int numWinner = 0;
            for (int i = 0; i < Players.Count; ++i)
            {
                game.NumCards |= numCards[i] << (i * CARD_BIT_COUNT);
                if (numCards[i] == 0)
                {
                    game.Winner = i;
                    numWinner++;
                }
            }

            if (numWinner == 1)
                AddGame(game);
        }

        private void AddGame(Game game)
        {
            ScoreResult scoreResult = new ScoreResult();
            scoreResult.Time = game.Time;
            scoreResult.NumBomb = game.NumBomb;

            Object scoreResultBox = scoreResult;
            int winnerScore = 0;
            for (int i = 0; i < Players.Count; ++i)
            {
                int score = ComputeScore(game.NumBomb, GetNumCard(game.NumCards, i));
                winnerScore -= score;
                scoreResult.GetType().GetProperty(string.Format("Score{0}", i)).SetValue(scoreResultBox, score);
                Players[i].Score += score;
            }
            scoreResult.GetType().GetProperty(string.Format("Score{0}", game.Winner)).SetValue(scoreResultBox, winnerScore);
            Players[game.Winner].Score += winnerScore;

            Games.Add(game);
            ScoreResults.Add((ScoreResult)scoreResultBox);
        }

        public void RemoveGame(int index)
        {
            Game game = Games[index];
            int winnerScore = 0;
            for (int i = 0; i < Players.Count; ++i)
            {
                int score = ComputeScore(game.NumBomb, GetNumCard(game.NumCards, i));
                winnerScore -= score;
                Players[i].Score -= score;
            }
            Players[game.Winner].Score -= winnerScore;

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

        public void AddDinner(int cost)
        {
            Dinner dinner = new Dinner();
            dinner.Time = DateTime.Now;
            dinner.TotalCost = cost;

            Object dinnerBox = dinner;

            int totalScore = 0;
            for (int i = 0; i < Players.Count; ++i)
            {
                if (Players[i].Score < 0)
                    totalScore -= Players[i].Score;
            }

            if (totalScore >= cost)
            {
                for (int i = 0; i < Players.Count; ++i)
                {
                    if (Players[i].Score < 0)
                    {
                        double t = (double)(-Players[i].Score) / (double)totalScore;
                        Players[i].Score += (int)(cost * t);
                    }
                    else if (Players[i].Score > 0)
                    {
                        double t = (double)(Players[i].Score) / (double)totalScore;
                        Players[i].Score -= (int)(cost * t);
                    }
                }
            }
            else
            {
                cost = (cost - totalScore) / Players.Count;
                for (int i = 0; i < Players.Count; ++i)
                {
                    Players[i].Score = 0;
                    dinner.GetType().GetProperty(string.Format("Cost{0}", i)).SetValue(dinnerBox, -cost);
                }
            }

            Dinners.Add((Dinner)dinnerBox);
        }
    }
}
