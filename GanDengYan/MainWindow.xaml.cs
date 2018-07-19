using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace GanDengYan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        private GameWorld gameWorld;
        private List<ScoreResult> scoreResultList;
        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateGameWorld(List<string> players, int FAN)
        {
            int numPlayer = players.Count;

            gameWorld = new GameWorld(players, FAN);

            for (int i = 0; i < players.Count; ++i)
            {
                ScoreGridView.Columns[i + 2].Header = players[i];
            }
            ScoreListViewer.ItemsSource = gameWorld.ScoreResults;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            ScoreListViewer.SelectedItem = ((Button)sender).DataContext;
            gameWorld.RemoveGame(ScoreListViewer.SelectedIndex);
            //ScoreListViewer.Items.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (gameWorld == null)
                return;

            if (BombUpDown.Value == null)
                return;

            List<int> cards = new List<int>();
            int numPlayer = gameWorld.Players.Count;
            for (int i = 0; i < numPlayer; ++i)
            {
                IntegerUpDown cardUpDown = CardGrid.FindName(string.Format("Player{0}UpDown", i)) as IntegerUpDown;
                if (cardUpDown == null || cardUpDown.Value == null)
                    return;
                cards.Add((int)cardUpDown.Value);
            }
            gameWorld.AddGame((int)BombUpDown.Value, cards);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NewGame newGameWindow = new NewGame();
            if (newGameWindow.ShowDialog() == true)
            {
                CreateGameWorld(null, 0);
            }
        }
    }
}
