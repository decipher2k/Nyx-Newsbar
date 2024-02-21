using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
using static System.Net.Mime.MediaTypeNames;

namespace Nyx_Appbar
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int numFeeds = 10;
        String prevText = "";
        bool blocked = false;
        bool running = true;       

        Canvas panel = new Canvas();
        Canvas textCanvas = new Canvas();
        Canvas textCanvas1 = new Canvas();
        StackPanel stackPanel1 = new StackPanel();
        StackPanel stackPanel2 = new StackPanel();        
        Details wndDetails = new Details();

        List<RSS> rss = new List<RSS>();
        List<RSS> rss1 = new List<RSS>();

        public class RSS
        {
            public String title = "";
            public String link = "";
            public String description = "";
        }

        public MainWindow()
        {
            InitializeComponent();
            MouseRightButtonDown += MainWindow_MouseRightButtonDown;
            MouseLeave += TextBlock_MouseLeave;
        }

        private void MainWindow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            cm.IsOpen = true;
        }

        List<String> rssFeeds = new List<String>()
        {
            "https://rss.nytimes.com/services/xml/rss/nyt/World.xml"
        };

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Nyx Newsbar\\settings.conf";
            if(!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Nyx Newsbar\\"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Nyx Newsbar\\");

            if(File.Exists(fileName))
            {
                if(System.IO.File.ReadAllLines(fileName).Count() > 0)
                    rssFeeds.Clear();

                foreach(String line in System.IO.File.ReadAllLines(fileName))
                {
                    rssFeeds.Add(line);
                }
            }

            Rect workspace = SystemParameters.WorkArea;
            this.Top = workspace.Top;
            this.Left = workspace.Left;
            this.Width = workspace.Width;
            this.Height = 40;
            this.Background = Brushes.Black;

            panel.Width = this.Width;

            stackPanel1.Orientation = Orientation.Horizontal;
            stackPanel2.Orientation = Orientation.Horizontal;

            textCanvas.Children.Add(stackPanel1);
            textCanvas1.Children.Add(stackPanel2);

            panel.Children.Add(textCanvas);
            panel.Children.Add(textCanvas1);
            Canvas.SetLeft(textCanvas, this.Width);
            Canvas.SetTop(textCanvas, (this.Height / 4));

            Canvas.SetLeft(textCanvas1, this.Width);
            Canvas.SetTop(textCanvas1, (this.Height / 4));

            grid.Children.Add(panel);

            WpfAppBar.AppBarFunctions.SetAppBar(this, WpfAppBar.ABEdge.Top);
            new System.Threading.Thread(Scroll).Start();
            new System.Threading.Thread(reloadTexts).Start();
        }
        List<RSS> newRSS = new List<RSS>();
        void Scroll()
        {
            while (running)
            {

                textCanvas1.Dispatcher.Invoke(() =>
                {
                    Canvas.SetLeft(textCanvas1, Canvas.GetLeft(textCanvas1) - 1);
                });
                textCanvas.Dispatcher.Invoke(() =>
                {
                    Canvas.SetLeft(textCanvas, Canvas.GetLeft(textCanvas) - 1);
                });

                textCanvas1.Dispatcher.Invoke(() =>
                {
                    if (Canvas.GetLeft(textCanvas) < 0)
                        Canvas.SetLeft(textCanvas1, Canvas.GetLeft(textCanvas) + stackPanel1.ActualWidth);
                });

                textCanvas.Dispatcher.Invoke(() =>
                {
                    if (Canvas.GetLeft(textCanvas1) < 0)
                        Canvas.SetLeft(textCanvas, Canvas.GetLeft(textCanvas1) + stackPanel2.ActualWidth);
                });

                System.Threading.Thread.Sleep(10);
            }
        }

        void updateTexts()
        {
            while (true)
            {
                textCanvas.Dispatcher.Invoke(() =>
                {
                    if (Canvas.GetLeft(textCanvas) < 0)
                        rss1 = newRSS;
                    else
                        rss = newRSS;
                });

                textCanvas.Dispatcher.Invoke(() =>
                {
                    if (Canvas.GetLeft(textCanvas) < 0)
                        stackPanel2.Children.Clear();
                    else
                        stackPanel1.Children.Clear();

                    List<RSS> mrss = new List<RSS>();
                    if (Canvas.GetLeft(textCanvas) < 0)
                        mrss = rss1;
                    else
                        mrss = rss;

                    foreach (RSS r in mrss)
                    {
                        if (Canvas.GetLeft(textCanvas) < 0)
                        {
                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = r.title;
                            textBlock.Margin = new Thickness(5, 0, 5, 0);
                            textBlock.Foreground = Brushes.White;
                            textBlock.MouseEnter += TextBlock_MouseEnter;
                            textBlock.MouseDown += TextBlock_MouseDown;
                            textBlock.MouseRightButtonDown += MainWindow_MouseRightButtonDown;

                            stackPanel2.Children.Add(textBlock);
                            stackPanel2.Width += textBlock.Width + 15;

                            TextBlock textBlock1 = new TextBlock();
                            textBlock1.Text = "|";
                            textBlock1.Margin = new Thickness(5, 0, 5, 0);
                            textBlock1.Foreground = Brushes.White;
                            stackPanel2.Children.Add(textBlock1);
                            stackPanel2.Width += textBlock1.Width + 15;
                        }
                        else
                        {
                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = r.title;
                            textBlock.Margin = new Thickness(5, 0, 5, 0);
                            textBlock.Foreground = Brushes.White;
                            textBlock.MouseEnter += TextBlock_MouseEnter;
                            textBlock.MouseDown += TextBlock_MouseDown;
                            textBlock.MouseRightButtonDown += MainWindow_MouseRightButtonDown;
                            stackPanel1.Children.Add(textBlock);
                            stackPanel1.Width += textBlock.Width + 15;

                            TextBlock textBlock1 = new TextBlock();
                            textBlock1.Text = "|";
                            textBlock1.Margin = new Thickness(5, 0, 5, 0);
                            textBlock1.Foreground = Brushes.White;
                            stackPanel1.Children.Add(textBlock1);
                            stackPanel1.Width += textBlock1.Width + 15;
                        }
                    }
                });
                System.Threading.Thread.Sleep(1000);
            }
        }

        void reloadTexts()
        {
            new System.Threading.Thread(updateTexts).Start();
            while (true)
            {              
                textCanvas.Dispatcher.Invoke(() =>
                {
                    if (Canvas.GetLeft(textCanvas) < 0)
                        rss1.Clear();
                    else
                        rss.Clear();
                    newRSS.Clear();

                    foreach (String url in rssFeeds)
                    {
                        List<String> titles = new List<String>();
                        List<String> links = new List<String>();
                        List<String> descriptions = new List<String>();

                        try
                        {
                            String feed = new WebClient().DownloadString(url);
                            feed=feed.Replace("&gt;", ">").Replace("&lt;", "<").Replace("&quot;", "\"").Replace("&apos;", "'");
                            feed = Encoding.UTF8.GetString(Encoding.Default.GetBytes(feed));
                            String regexTitle = "\\<title\\>(.*?)\\<\\/title\\>";
                            String regexLink = "\\<link\\>(.*?)\\<\\/link\\>";
                            String regexDescription = "\\<description\\>(.*?)\\<\\/description\\>";

                            Regex rgTitle = new Regex(regexTitle);
                            Regex rgLink = new Regex(regexLink);
                            Regex rgDescription = new Regex(regexDescription);

                            MatchCollection matchedTitle = rgTitle.Matches(feed);
                            MatchCollection matchedLink = rgLink.Matches(feed);
                            MatchCollection matchedDescription = rgDescription.Matches(feed);

                            for (int count = 0; count < (matchedTitle.Count >= numFeeds ? numFeeds : matchedTitle.Count); count++)
                            {
                                titles.Add(matchedTitle[count].Value);
                            }

                            for (int count = 0; count < (matchedLink.Count >= numFeeds ? numFeeds : matchedLink.Count); count++)
                            {
                                links.Add(matchedLink[count].Value);
                            }

                            for (int count = 0; count < (matchedDescription.Count >= numFeeds ? numFeeds : matchedDescription.Count); count++)
                            {
                                descriptions.Add(matchedDescription[count].Value);
                            }

                            for (int i = 0; i < titles.Count; i++)
                            {
                                RSS lrss = new RSS()
                                {
                                    title = titles[i].Replace("<title>", "").Replace("</title>", ""),
                                    link = links.Count > i + 1 ? links[i].Replace("<link>", "").Replace("</link>", "") : "",
                                    description = descriptions.Count > i + 1 ? descriptions[i].Replace("<description>", "").Replace("</description>", "").Replace("<![CDATA[", "").Replace("]]>", "") : ""
                                };

                                if ((Canvas.GetLeft(textCanvas) > 0 && Canvas.GetLeft(textCanvas) > 0))
                                {
                                    rss1.Add(lrss);
                                    rss.Add(lrss);
                                }
                                newRSS.Add(lrss);
                            }
                        }
                        catch { }
                    }            
                });
                System.Threading.Thread.Sleep(1000*60*5);
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Released)
            {
                TextBlock txt = (TextBlock)sender;

                String title = txt.Text;
                String link = "";
                if (rss.Where(a => a.title == title).Count() > 0)
                {
                    link = rss.Where(a => a.title == title).First().link;

                }
                else if (rss1.Where(a => a.title == title).Count() > 0)
                {
                    link = rss1.Where(a => a.title == title).First().link;

                }
                if (link != "")
                    Process.Start(link, "");
            }
        }
        
        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {            
            Cursor = Cursors.Arrow;
            wndDetails.Hide();
            blocked=false;            
        }
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock txt = (TextBlock)sender;
            if (!blocked || txt.Text!=prevText)
            {
                prevText = txt.Text;
                blocked = true;
                
                Cursor = Cursors.Hand;
                String title = txt.Text;
                if (rss.Where(a => a.title == title).Count() > 0)
                {
                    String link = rss.Where(a => a.title == title).First().link;
                    String description = rss.Where(a => a.title == title).First().description;
                    wndDetails.setDetails(description);
                }
                else if (rss1.Where(a => a.title == title).Count() > 0)
                {
                    String link = rss1.Where(a => a.title == title).First().link;
                    String description = rss1.Where(a => a.title == title).First().description;
                    wndDetails.setDetails(description);
                }
                wndDetails.Left = (double)Mouse.GetPosition(this).X;
                if (wndDetails.Left > SystemParameters.WorkArea.Width - wndDetails.Width)
                    wndDetails.Left = SystemParameters.WorkArea.Width - wndDetails.Width;
                wndDetails.Top = this.Height;
                wndDetails.Show();
                wndDetails.Topmost = true;
           }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings wndSettings = new Settings();
            wndSettings.ShowDialog();
            
            var fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Nyx Newsbar\\settings.conf";
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Nyx Newsbar\\"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Nyx Newsbar\\");

            if (File.Exists(fileName))
            {
                if (System.IO.File.ReadAllLines(fileName).Count() > 0)
                    rssFeeds.Clear();

                foreach (String line in System.IO.File.ReadAllLines(fileName))
                {
                    rssFeeds.Add(line);
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            running = false;
            Process.GetCurrentProcess().Kill(); 
        }
    }
}
