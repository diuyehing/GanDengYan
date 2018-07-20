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
using System.Windows.Shapes;

namespace GanDengYan
{
    /// <summary>
    /// Interaction logic for NewGame.xaml
    /// </summary>
    public partial class NewGame : Window
    {
        public List<string> PlayerNames = new List<string>();
        public int FAN = 15;

        public NewGame()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 6; ++i)
            {
                TextBox nameTextBox = NewGameGrid.FindName(string.Format("Player{0}NamaTextBox", i)) as TextBox;
                if (nameTextBox == null || nameTextBox.Text == null)
                    break;
                if (nameTextBox.Text == "")
                    break;
                PlayerNames.Add(nameTextBox.Text);
            }
            if (int.TryParse(FANTextBox.Text, out FAN) == false)
                return;
            
            this.DialogResult = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
