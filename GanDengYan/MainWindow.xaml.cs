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
using System.Xml.Linq;
using System.Xml;
using System.Windows.Markup;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GanDengYan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameWorld gameWorld;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateGameWorld(List<string> players, int FAN)
        {
            int numPlayer = players.Count;

            gameWorld = new GameWorld(players, FAN);

            UpdateScoreListView();

            UpdateDinnerListView();

            UpdateNameLabels();

            for (int i = 0; i < players.Count; ++i)
            {
                Label scoreLabel = ScoreStackPanel.FindName(string.Format("Score{0}Label", i)) as Label;
                scoreLabel.Content = string.Format("{0}: 0", players[i]);
            }
            for (int i = players.Count; i < 6; ++i)
            {
                Label scoreLabel = ScoreStackPanel.FindName(string.Format("Score{0}Label", i)) as Label;
                scoreLabel.Content = "";
            }
        }

        private void UpdateNameLabels()
        {
            for (int i = 0; i < gameWorld.Players.Count; ++i)
            {
                Label nameLabel = CardGrid.FindName(string.Format("Plyer{0}NameLable", i)) as Label;
                if (nameLabel == null)
                    return;
                nameLabel.Content = gameWorld.Players[i].Name;

                IntegerUpDown cardUpDown = CardGrid.FindName(string.Format("Player{0}UpDown", i)) as IntegerUpDown;
                if (cardUpDown == null)
                    return;
                cardUpDown.IsReadOnly = false;
            }

            for (int i = gameWorld.Players.Count; i < 6; ++i)
            {
                Label nameLabel = CardGrid.FindName(string.Format("Plyer{0}NameLable", i)) as Label;
                if (nameLabel == null)
                    return;
                nameLabel.Content = "";

                IntegerUpDown cardUpDown = CardGrid.FindName(string.Format("Player{0}UpDown", i)) as IntegerUpDown;
                if (cardUpDown == null)
                    return;
                cardUpDown.IsReadOnly = true;
            }
        }

        private void UpdateScoreListView()
        {
            for (int i = 0; i < ScoreGridView.Columns.Count; ++i)
            {
                GridViewColumn column = ScoreGridView.Columns[i];
                string header = column.Header as string;
                if (header.CompareTo("日期") == 0)
                    continue;
                if (header.CompareTo("炸弹") == 0)
                    continue;
                if (header.CompareTo("操作") == 0)
                    continue;
                ScoreGridView.Columns.RemoveAt(i);
                i = i - 1;
            }

            for (int i = gameWorld.Players.Count - 1; i >= 0; --i)
            {
                GridViewColumn column = new GridViewColumn();
                column.Header = gameWorld.Players[i].Name;
                column.Width = 100;
                column.DisplayMemberBinding = new Binding(string.Format("Score{0}", i));
                ScoreGridView.Columns.Insert(2, column);
            }

            ScoreListViewer.ItemsSource = gameWorld.ScoreResults;
            ScoreListViewer.Items.Refresh();
        }

        private void UpdateDinnerListView()
        {
            for (int i = 0; i < DinnerGridView.Columns.Count; ++i)
            {
                GridViewColumn column = DinnerGridView.Columns[i];
                string header = column.Header as string;
                if (header.CompareTo("日期") == 0)
                    continue;
                if (header.CompareTo("饭钱") == 0)
                    continue;
                DinnerGridView.Columns.RemoveAt(i);
                i = i - 1;
            }

            for (int i = gameWorld.Players.Count - 1; i >= 0; --i)
            {
                GridViewColumn column = new GridViewColumn();
                column.Header = gameWorld.Players[i].Name;
                column.Width = 100;
                column.DisplayMemberBinding = new Binding(string.Format("Cost{0}", i));
                DinnerGridView.Columns.Insert(2, column);
            }

            DinnerListViewer.ItemsSource = gameWorld.Dinners;
            DinnerListViewer.Items.Refresh();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmToDel = System.Windows.MessageBox.Show("确认要删除所选行吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmToDel == MessageBoxResult.Yes)
            {
                ScoreListViewer.SelectedItem = ((Button)sender).DataContext;
                gameWorld.RemoveGame(ScoreListViewer.SelectedIndex);
                UpdatePlayerScoreLabels();
                ScoreListViewer.Items.Refresh();
            }
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
                cardUpDown.Value = 0;
            }
            gameWorld.AddGame((int)BombUpDown.Value, cards);
            BombUpDown.Value = 0;

            UpdatePlayerScoreLabels();
            ScoreListViewer.Items.Refresh();
        }

        private void UpdatePlayerScoreLabels()
        {
            for (int i = 0; i < gameWorld.Players.Count; ++i)
            {
                Label scoreLabel = ScoreStackPanel.FindName(string.Format("Score{0}Label", i)) as Label;
                scoreLabel.Content = string.Format("{0}: {1}", gameWorld.Players[i].Name, gameWorld.Players[i].Score);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (gameWorld != null)
                if (!Save())
                    return;

            NewGame newGameWindow = new NewGame();
            if (newGameWindow.ShowDialog() == true)
            {
                CreateGameWorld(newGameWindow.PlayerNames, newGameWindow.FAN);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (gameWorld == null)
                return;

            int cost = 0;
            if (int.TryParse(DinnerCostTextBox.Text, out cost) == false)
                return;

            gameWorld.AddDinner(cost);

            UpdatePlayerScoreLabels();
            DinnerListViewer.Items.Refresh();
        }

        private bool Save()
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.InitialDirectory = @"D:\";
            saveFileDialog.Filter = "gdy|*.gdy";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    FileStream fs = new FileStream(saveFileDialog.FileName.ToString(), FileMode.Create);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, gameWorld);
                    fs.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                return true;
            }
            return false;
        }

        private void Load()
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.InitialDirectory = @"D:\";
            openFileDlg.Filter = "gdy|*.gdy";
            if (openFileDlg.ShowDialog() == true)
            {
                try
                {
                    FileStream fs = new FileStream(openFileDlg.FileName.ToString(), FileMode.Open);
                    BinaryFormatter bf = new BinaryFormatter();
                    gameWorld = (GameWorld)bf.Deserialize(fs);
                    fs.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                UpdateScoreListView();

                UpdateDinnerListView();

                UpdateNameLabels();

                for (int i = 0; i < gameWorld.Players.Count; ++i)
                {
                    Label scoreLabel = ScoreStackPanel.FindName(string.Format("Score{0}Label", i)) as Label;
                    scoreLabel.Content = string.Format("{0}: 0", gameWorld.Players[i].Name);
                }
                for (int i = gameWorld.Players.Count; i < 6; ++i)
                {
                    Label scoreLabel = ScoreStackPanel.FindName(string.Format("Score{0}Label", i)) as Label;
                    scoreLabel.Content = "";
                }

                UpdatePlayerScoreLabels();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Load();
        }
    }
}
