using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parser
{
    public partial class ParseWindow : Form
    {
        private readonly TextBox url;
        private readonly Panel results;
        private readonly Button parse;
        private readonly Button printStatistics;
        private readonly Button saveStatistics;
        private readonly Button download;
        private readonly Button delete;

        private readonly Label search;

        private Page page;

        public ParseWindow()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            ClientSize = new Size(800, 600);

            search = new Label
            {
                Text = "Result: ",
                Location = new Point(0, 0),
                Size = new Size(100, 20)
            };
            url = new TextBox
            {
                Text = @"https://www.simbirsoft.com/",
                Location = new Point(100, 0),
                Size = new Size(700, 20)
            };
            parse = new Button
            {
                Text = "Parse!",
                Location = new Point(0, 20),
                Size = new Size(100, 20)
            };
            saveStatistics = new Button
            {
                Text = "Save Statistics",
                Location = new Point(100, 20),
                Size = new Size(100, 20)
            };
            printStatistics = new Button
            {
                Text = "Print Statistics",
                Location = new Point(200, 20),
                Size = new Size(100, 20)
            };
            download = new Button
            {
                Text = "dowload History",
                Location = new Point(300, 20),
                Size = new Size(100, 20)
            };
            delete = new Button
            {
                Text = "delete",
                Location = new Point(400,20),
                Size = new Size(100, 20)
            };
            results = new Panel
            {
                Location = new Point(0, 40),
                Size = new Size(800, 560)
            };

            parse.Click += PrintPage;
            saveStatistics.Click += SaveStatistics;
            printStatistics.Click += PrintStatistics;
            download.Click += DowloadHistory;
            delete.Click += DeleteStatistics;

            
            url.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    PrintPage(sender, e);
            };

            Controls.Add(search);
            Controls.Add(url);
            Controls.Add(parse);
            Controls.Add(results);
            Controls.Add(printStatistics);
            Controls.Add(saveStatistics);
            Controls.Add(download);
            Controls.Add(delete);
        }

        private void DeleteStatistics(object sender, EventArgs e)
        {
            if (!Change())
                return;
            var deleteUrl = url.Text;
            StatisticsDbTask.RemoveStatistics(deleteUrl);
        }

        private void PrintPage(object sender, EventArgs e)
        {
            if (!Change())
                return;
            RichTextBox box = CreateDefaultBox();
            foreach (var line in page.Words)
                box.Text += line + "\n";
            results.Controls.Add(box);
        }

        private void PrintStatistics(object sender, EventArgs e)
        {
            if (!Change())
                return;
            var box = CreateDefaultBox();
            foreach (var word in page.Statistics)
                box.Text += word.Word +"\t" + word.Count + "\n";
            results.Controls.Add(box);
        }

        private void SaveStatistics(object sender, EventArgs e)
        {
            if (!Change())
                return;
            StatisticsDbTask.SaveStatistics(page);
        }

        private void DowloadHistory(object sender, EventArgs e)
        {
            var history = StatisticsDbTask.DownloadHistory();
            var box = CreateDefaultBox();
            foreach (var elem in history)
                box.Text += elem.Url + "\t" + elem.Word + "\t" + elem.Count + "\n";
            results.Controls.Add(box);
        }

        private RichTextBox CreateDefaultBox()
        {
            var box = new RichTextBox();
            box.ReadOnly = true;
            box.Size = new Size(results.Width, results.Height);
            box.Location = new Point(0, 0);
            return box;
        }

        private bool Change()
        {
            if(!Page.IsUrl(url.Text))
            {
                var message = new Label{ Text = "please write url for work app" };
                message.Size = new Size(200, 20);
                results.Controls.Add(message);
                return false;
            }
            else
            { 
                page = new Page(url.Text);
                results.Controls.Clear();
                return true;
            }
        }
    }
}
