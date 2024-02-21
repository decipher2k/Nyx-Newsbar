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
using static System.Net.Mime.MediaTypeNames;


namespace Nyx_Appbar
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        String fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Nyx Newsbar\\settings.conf";
        public Settings()
        {
            InitializeComponent();
            String[] lines = new string[] { };
            if (System.IO.File.Exists(fileName))
                lines = System.IO.File.ReadAllLines(fileName);

            foreach (String line in lines)
            {
                lbData.Items.Add(line);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {       
            lbData.Items.Add(txtData.Text);
            String[] lines = new String[] { };
            if (System.IO.File.Exists(fileName))
                lines = System.IO.File.ReadAllLines(fileName);
            lines=lines.Append(txtData.Text).ToArray();
            txtData.Text = "";
            System.IO.File.WriteAllLines(fileName, lines);         
        }
    

        private void bnRemove_Click(object sender, RoutedEventArgs e)
        {           
            if(lbData.SelectedItem != null)
            {
                String text=lbData.SelectedItem.ToString();
                lbData.Items.Remove(lbData.SelectedItem.ToString());
                String[] lines = new String[] { };
                String[] lines_new = new String[] { };
                if (System.IO.File.Exists(fileName))
                    lines = System.IO.File.ReadAllLines(fileName);
                    
                foreach (String line in lines)
                {
                    if(line!=text)
                        lines_new= lines_new.Append(line).ToArray();
                }
                System.IO.File.WriteAllLines(fileName, lines_new);               
            }
        }
    }
}
