using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;//Dosya İşlemleri için İsim Uzayı
using System.Reflection;//Adres Ulaşımı için İsim Uzayı
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yaz_Lab2_Proje_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap original_picture;
        Bitmap[] all_p_picture;
        int p_width, p_height, button_click_c = 0, f_button = -1, mix_button_c = 0,global_true=0, misguided=0,skor_c=0, txt_count = 0;
        double puan=0;
        StreamReader rd;
        StreamWriter wr;
        string txt_path="",d_path="";
        double[] skor;
        private void picture_select()
        {
            bool cnt = false;
            OpenFileDialog file;
            do
            {
                file = new OpenFileDialog();
                string uzanti = "";
                file.Title = "Resim Ara";
                file.Filter = "Tüm Dosyalar | *.*";
                if (file.ShowDialog() == DialogResult.OK)
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(file.FileName);
                    uzanti = info.Extension;
                }
                if (uzanti != ".jpg" && uzanti != ".JPG" && uzanti != ".jpeg" && uzanti != ".JPEG" && uzanti != ".png" && uzanti != ".PNG" && uzanti != ".tiff" && uzanti != ".TIFF")
                { MessageBox.Show("Uygun Dosya Formatı Kullanmalısınız !!!", "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information); cnt = true; }
                else cnt = false;
            } while (cnt);
            Image img = Image.FromFile(file.FileName);
            original_picture = new Bitmap(img);
            p_width = img.Width/4;
            p_height = img.Height/4;
            button17.Text = "Resim Yüklü ("+file.FileName.ToString()+")";           
        }     

        private void cut_to_picture() {
            int r = 0, x_ek=0, y_ek=0;
            bool cnt = false;
            all_p_picture = new Bitmap[16];
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    Bitmap part = new Bitmap(p_width, p_height);
                    for (int x = 0; x < p_height; x++) {
                        for (int y = 0; y < p_width; y++) {
                            part.SetPixel(y, x, original_picture.GetPixel(y+y_ek,x+x_ek));
                        }
                    }
                    y_ek += p_width;
                    Random rst = new Random();
                    do {
                        r = rst.Next(16);
                        if (all_p_picture[r] == null) cnt = false;
                        else cnt = true;
                    } while (cnt);
                    all_p_picture[r] = part;

                }
                x_ek += p_height;
                y_ek = 0;
            }
            part_pic_show();
        }

        private void part_pic_show() {
            for (int i = 0; i < 16; i++) { this.Controls["button" + (i + 1)].BackgroundImage = all_p_picture[i]; }
        }

        private void mix()
        {
            bool cnt = false;
            int r = 0;
            Bitmap[] temp = new Bitmap[16];
            for (int i = 0; i < 16; i++) { temp[i] = all_p_picture[i]; }
            Random rst = new Random();
            all_p_picture = new Bitmap[16];
            for (int i = 0; i < 16; i++)
            {
                do
                {
                    r = rst.Next(16);
                    if (all_p_picture[r] == null) cnt = false;
                    else cnt = true;
                } while (cnt);
                all_p_picture[r] = temp[i];
            }
            part_pic_show();
        }

        private void control() {
            int d_pixel = 0, i_add = 0, j_add=0,count=0,instant_true=0;
            borderColor_clear();

            for (int x = 0; x < 4; x++) {
                for (int y = 0; y < 4; y++) {
                    for (int i = 0; i < p_height; i++) {
                        for (int j = 0; j < p_width; j++) {
                            if (all_p_picture[count].GetPixel(j, i) != original_picture.GetPixel(j+j_add, i+i_add)) { d_pixel++;break; }                            
                        }
                        if (d_pixel != 0) break;
                    }
                    if (d_pixel == 0) {button_borderColor(count+1);instant_true++; }
                    d_pixel = 0;                    
                    j_add += p_width;
                    count++; 
                }
                i_add += p_height;
                j_add = 0;
            }
            if (instant_true == 0)
            {
                button_locked(false);
            }
            else
            {
                button_locked(true);
                if (instant_true <= global_true) { misguided++; global_true = instant_true; }
                else { global_true = instant_true; }
                if (global_true == 16) {
                    puan = global_true * 6.25 - (misguided * 6.25);
                    if (puan < 0) puan = 0;
                     MessageBox.Show("Puzzle Tamamlanmıştır :)\nHatalı Hamle Sayınız:"+misguided+"\n   Puanınız:"+puan,"Oyun Mesaj Sistemi");
                    button_locked(false);
                    if (skor_c == 1)
                    {
                        skor = new double[1];
                        skor[txt_count] = puan;                        
                    }
                    else { skor[txt_count-1] = puan; }
                    DialogResult sc = MessageBox.Show("Tekrar Oynamak İster misiniz?", "Oyun Mesaj Sistemi", MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                    if (sc == DialogResult.Yes)
                    {   skor_save();                     
                        skor_read();
                        for (int i = 1; i <= 16; i++) { this.Controls["button" + i].BackgroundImage = null;}
                        button_locked(false);
                        borderColor_clear();
                        button_click_c = 0; f_button = -1; mix_button_c = 0; global_true = 0; misguided = 0; skor_c = 0;puan = 0;
                        button17.Text = "Resim Seç";
                        button18.Text = "Karıştır ve Dağıt";
                        button18.Enabled = false;
                    }
                    else { this.Close();}
                } 
            }
                     
        }

        private void button_borderColor(int button) {
            switch (button)
            {
                case 1: { button1.FlatAppearance.BorderColor = Color.Green; break; }
                case 2: { button2.FlatAppearance.BorderColor = Color.Green; break; }
                case 3: { button3.FlatAppearance.BorderColor = Color.Green; break; }
                case 4: { button4.FlatAppearance.BorderColor = Color.Green; break; }
                case 5: { button5.FlatAppearance.BorderColor = Color.Green; break; }
                case 6: { button6.FlatAppearance.BorderColor = Color.Green; break; }
                case 7: { button7.FlatAppearance.BorderColor = Color.Green; break; }
                case 8: { button8.FlatAppearance.BorderColor = Color.Green; break; }
                case 9: { button9.FlatAppearance.BorderColor = Color.Green; break; }
                case 10: { button10.FlatAppearance.BorderColor = Color.Green; break; }
                case 11: { button11.FlatAppearance.BorderColor = Color.Green; break; }
                case 12: { button12.FlatAppearance.BorderColor = Color.Green; break; }
                case 13: { button13.FlatAppearance.BorderColor = Color.Green; break; }
                case 14: { button14.FlatAppearance.BorderColor = Color.Green; break; }
                case 15: { button15.FlatAppearance.BorderColor = Color.Green; break; }
                case 16: { button16.FlatAppearance.BorderColor = Color.Green; break; }
            }
        }

        private void borderColor_clear()
        {
            button1.FlatAppearance.BorderColor = Color.Black;
            button2.FlatAppearance.BorderColor = Color.Black;
            button3.FlatAppearance.BorderColor = Color.Black;
            button4.FlatAppearance.BorderColor = Color.Black;
            button5.FlatAppearance.BorderColor = Color.Black;
            button6.FlatAppearance.BorderColor = Color.Black;
            button7.FlatAppearance.BorderColor = Color.Black;
            button8.FlatAppearance.BorderColor = Color.Black;
            button9.FlatAppearance.BorderColor = Color.Black;
            button10.FlatAppearance.BorderColor = Color.Black;
            button11.FlatAppearance.BorderColor = Color.Black;
            button12.FlatAppearance.BorderColor = Color.Black;
            button13.FlatAppearance.BorderColor = Color.Black;
            button14.FlatAppearance.BorderColor = Color.Black;
            button15.FlatAppearance.BorderColor = Color.Black;
            button16.FlatAppearance.BorderColor = Color.Black;

        }

        private void button_change(int s_button) {
            Bitmap temp = all_p_picture[s_button];
            all_p_picture[s_button]= all_p_picture[f_button];
            all_p_picture[f_button]=temp;
            part_pic_show();
            f_button = -1;
            button_click_c = 0;
            control();
        }

        private void button_locked(bool state) {
            if (!state)
            {
                for (int i = 0; i < 16; i++) { this.Controls["button" + (i + 1)].Enabled = false; }
            }
            else
            {
                for (int i = 0; i < 16; i++) { this.Controls["button" + (i + 1)].Enabled = true; }
            }
        }

        private void skor_read() {
            d_path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            DirectoryInfo drt = new DirectoryInfo(d_path);
            txt_path = d_path + "\\enyuksekskor.txt";
            if (File.Exists(txt_path))
            {
                skor_c = 0;             
                rd = new StreamReader(txt_path);

                TextReader trd = new StreamReader(txt_path);
                string read = trd.ReadToEnd();
                trd.Close();

                string[] spt = read.Split('\n'); txt_count = spt.Length;
                this.Text = txt_count.ToString();
                skor = new double[txt_count];
                for (int i = 0; i < txt_count; i++)
                {
                    string text = rd.ReadLine();
                    skor[i] = Convert.ToDouble(text);
                }
                rd.Close();
                label2.Text = skor[0].ToString();
            }
            else { skor_c = 1; label2.Text = " ??? "; }
        }

        private void skor_save() {
            try
            {
                if (txt_count != 0)
                {
                    for (int i = 0; i < txt_count; i++)
                    {
                                for (int j = i + 1; j < txt_count; j++)
                                {
                                    if (skor[i] < skor[j])
                                    {
                                        double kap = skor[j];
                                        skor[j] = skor[i];
                                        skor[i] = kap;
                                    }
                                }
                    }

                            wr = new StreamWriter(txt_path);
                            for (int i = 0; i < txt_count; i++)
                            {
                                wr.WriteLine(skor[i]);
                            }
                            wr.Flush();
                            wr.Close();
                }
                else
                {
                    wr = new StreamWriter(txt_path);                    
                    wr.WriteLine(skor[0]);                   
                    wr.Flush();
                    wr.Close();
                }
          
            }
            catch (Exception){}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 0; }
            else { button_change(0); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 1; }
            else { button_change(1); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 2; }
            else { button_change(2); }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 3; }
            else { button_change(3); }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 4; }
            else { button_change(4); }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 5; }
            else { button_change(5); }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 6; }
            else { button_change(6); }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 7; }
            else { button_change(7); }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 8; }
            else { button_change(8); }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 9; }
            else { button_change(9); }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 10; }
            else { button_change(10); }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 11; }
            else { button_change(11); }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 12; }
            else { button_change(12); }
        }    

        private void button14_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 13; }
            else { button_change(13); }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 14; }
            else { button_change(14); }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (button_click_c == 0) { button_click_c = 1; f_button = 15; }
            else { button_change(15); }
        }        

        private void button17_Click(object sender, EventArgs e)//Resim Seçme Butonu
        {
            picture_select();
            button18.Enabled = true;
        }

        private void button18_Click(object sender, EventArgs e)//Karıştır Butonu
        {
            if (mix_button_c == 0)
            {
                mix_button_c++;
                cut_to_picture();
                control();
            }
            else { mix(); control(); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            skor_read();
            button_locked(false);          
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            skor_save();
        }
    }
}
