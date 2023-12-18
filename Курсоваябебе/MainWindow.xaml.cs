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
using System.Text.Json;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using System.Net;
using System.Drawing.Imaging;



namespace Курсоваябебе
{
    public partial class MainWindow : Window
    {
        List<Mem> memes = new List<Mem>();
        List<Mem> temp_memes = new List<Mem>();

        static string fileName = "MemesCatalog.json";

        public MainWindow()
        {
            InitializeComponent();

            meme_categories.Items.Add("all");

            if (File.Exists(fileName))
            {
                List<Mem> readed_memes = JsonSerializer.Deserialize<List<Mem>>(File.ReadAllText(fileName));

                foreach (Mem mem in readed_memes)
                {
                    memes.Add(mem);

                    meme_list.Items.Add(mem.Name);

                    if (!(meme_categories.Items.Contains(mem.Category)))
                        meme_categories.Items.Add(mem.Category);

                }
            }
        }

        private void memedown_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (!(bool)dlg.ShowDialog())
                return;

            Uri fileUri = new Uri(dlg.FileName);

            addmeme add_mem_wnd = new addmeme();

            if (add_mem_wnd.ShowDialog() == true)
            {
                byte[] imageArray = System.IO.File.ReadAllBytes(dlg.FileName);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                Mem mem = new Mem(add_mem_wnd.add_name_meme.Text, base64ImageRepresentation, add_mem_wnd.add_tag_meme.Text);

                memes.Add(mem);
                meme_list.Items.Add(mem.Name);

                if (!(meme_categories.Items.Contains(mem.Category)))
                    meme_categories.Items.Add(mem.Category);
            }


        }

        static ImageSource ByteToImage(byte[] imageData)
        {
            var bitmap = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();

            return (ImageSource)bitmap;
        }

        private void memesave_Click(object sender, RoutedEventArgs e)
        {
            string jsonString = JsonSerializer.Serialize(memes);
            File.WriteAllText(fileName, jsonString);
        }

        private void meme_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((meme_categories.SelectedIndex == -1 || meme_categories.SelectedIndex == 0) && (meme_find.Text.Length == 0 && meme_find_by_tag.Text.Length == 0))
            {
                if (meme_list.SelectedIndex != -1)
                    meme_img.Source = ByteToImage(Convert.FromBase64String(memes[meme_list.SelectedIndex].Img));
            }
            else
                if (meme_list.SelectedIndex != -1)
                meme_img.Source = ByteToImage(Convert.FromBase64String(temp_memes[meme_list.SelectedIndex].Img));
        }

        private void memedel_Click(object sender, RoutedEventArgs e)
        {
            if (meme_list.SelectedIndex != -1 && meme_list.Items.Count == memes.Count)
            {
                memes.Remove(memes[meme_list.SelectedIndex]);
                meme_list.Items.Clear();

                foreach (Mem mem in memes)
                    meme_list.Items.Add(mem.Name);
            }
            else
                MessageBox.Show("Select category all");
        }

        private void find_mem_Click(object sender, RoutedEventArgs e)
        {
            meme_list.Items.Clear();
            temp_memes.Clear();
            foreach (Mem mem in memes)
            {
                if (mem.Name.ToLower().Contains(meme_find.Text.ToLower()))
                {
                    meme_list.Items.Add(mem.Name);
                    temp_memes.Add(mem);
                }
            }
        }

        private void meme_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (meme_categories.SelectedIndex != -1)
            {
                meme_list.Items.Clear();
                if (meme_categories.SelectedItem.ToString().Equals("all"))
                {
                    foreach (Mem mem in memes)
                        meme_list.Items.Add(mem.Name);
                    return;
                }

                temp_memes.Clear();

                foreach (Mem mem in memes)
                {
                    if (mem.Category == meme_categories.SelectedItem.ToString())
                    {
                        meme_list.Items.Add(mem.Name);
                        temp_memes.Add(mem);
                    }
                }
            }
        }

       

        private void findmemtag_Click(object sender, RoutedEventArgs e)
        {
            meme_list.Items.Clear();
            temp_memes.Clear();
            foreach (Mem mem in memes)
            {
                foreach (string tag in mem.Tags)
                {
                    if (tag.ToLower().Equals(meme_find_by_tag.Text.ToLower()))
                    {
                        meme_list.Items.Add(mem.Name);
                        temp_memes.Add(mem);
                    }
                }
            }
        }

        private void addtag_Click(object sender, RoutedEventArgs e)
        {
            if (meme_list.SelectedIndex != -1 && meme_list.Items.Count == memes.Count && meme_add_tag.Text.Length > 0)
            {
                memes[meme_list.SelectedIndex].add_tag(meme_add_tag.Text);
            }
            else
                MessageBox.Show("выбери категорию или добавь тег");
        }
    }
}

