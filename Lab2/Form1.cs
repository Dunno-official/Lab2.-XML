using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;

namespace Lab2
{
    public partial class Form1 : Form
    {
        private List<RadioButton> _radioButtons;
        private List<ComboBox> _comboBoxes;
        private List<CheckBox> _checkBoxes;
        private List<string> _matchesBooks = new List<string>();
        public Form1()
        {
            InitializeComponent();
            InitializeLists();

            SaxRadioButton.Checked = true; // по умолчанию
        }

        private void InitializeLists()
        {
            _radioButtons = new List<RadioButton>
            { SaxRadioButton , DomRadioButton , LinqRadioButton};

            _comboBoxes = new List<ComboBox>
            { AuthorComboBox , TitleComboBox , GenreComboBox , PriceComboBox , PublishYearComboBox};

            _checkBoxes = new List<CheckBox>
            { AuthorCheckBox , TitleCheckBox , GenreCheckBox , PriceCheckBox , PublishYearCheckBox};
        }

        private void OpenXML_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TXT File|*.xml";
            openFileDialog.Title = "XML opening";
            openFileDialog.RestoreDirectory = true;


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (Library.TryOpenXml(openFileDialog.FileName))
                {
                    Library.FileName = openFileDialog.SafeFileName;

                    Context context = new Context();
                    int method = DefineMethod();
                    ISearchStrategy concreteStrategy = Context.Strategies[method];

                    context.SetStrategy(concreteStrategy);

                    Library.Books = context.GetBooks();

                    AddItemsToComboBox();
                }
            }
        }
        
        private void ClearButton_Click(object sender, EventArgs e)
        {
            richTextBox.Clear();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (CheckIfXmlOpened() && Check_CheckBoxes()) 
            {
                richTextBox.Clear();
                Book criteria = GetCriteria();

                _matchesBooks = Library.SearchByCriteria(criteria);

                ShowMatchBooks(_matchesBooks);
            }
        }

        private bool Check_CheckBoxes()
        {
            bool checkBoxSelected = false;

            for (int i = 0; i < _checkBoxes.Count && !checkBoxSelected; ++i)
                checkBoxSelected = _checkBoxes[i].Checked && _comboBoxes[i].SelectedItem != null;

            if (!checkBoxSelected)
                MessageBox.Show("Choose at least one criterion");

            return checkBoxSelected;
        }

        private bool CheckIfXmlOpened()
        {
            bool XmlSelected= !(Library.Books.Count == 0);

            if (!XmlSelected)
                MessageBox.Show("Open your XML first");

            return XmlSelected;
        }

        private int DefineMethod()
        {
            int answer;
            int i;
            bool buttonSelected = false;

            for (i = 0; i < _radioButtons.Count && !buttonSelected; ++i) // если найдена кнопка true, цикл прервется
                buttonSelected = _radioButtons[i].Checked;

            answer = --i; // в цикле сначала происходит инкремент, потом проверка. Следовательно, отнимаем 1

            return answer;
        }

        private Book GetCriteria()
        {
            Book answer = new Book();

            string AddCriteria (ComboBox comboBox , CheckBox checkBox)
            {
                if (checkBox.Checked && comboBox.SelectedItem != null)
                    return (string)comboBox.SelectedItem;
                else
                    return null;
            }

            answer.Author = AddCriteria(AuthorComboBox, AuthorCheckBox);
            answer.Title = AddCriteria(TitleComboBox, TitleCheckBox);
            answer.Genre = AddCriteria(GenreComboBox, GenreCheckBox);
            answer.PublishYear = Convert.ToInt32(AddCriteria(PublishYearComboBox, PublishYearCheckBox));

            if (PriceCheckBox.Checked)
                Library.SetPriceRange((string)PriceComboBox.SelectedItem);
            else
                Library.PriceRangeFrom = -1;

            return answer;
        }

        private void ShowMatchBooks(List<string> booksID)
        {
            foreach (var id in booksID)
            {
                richTextBox.Text += 

                    id + '\n' + '\n' +
                    Library.Books[id].Author + ". "+ Library.Books[id].Title + '\n' +
                    Library.Books[id].Description + '\n';
            }

            if (booksID.Count == 0)
                MessageBox.Show("No matching books :c");
            
        }


        private void CreateFilteredBooksXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("catalog");
            xmlDoc.AppendChild(rootNode);


            foreach (string ID in _matchesBooks)
            {
                XmlNode bookNode = xmlDoc.CreateElement("book");
                XmlAttribute attrib = xmlDoc.CreateAttribute("id");
                attrib.Value = ID;
                bookNode.Attributes.Append(attrib);

                XmlNode authorNode = xmlDoc.CreateElement("author");
                XmlNode titleNode = xmlDoc.CreateElement("title");
                XmlNode genreNode = xmlDoc.CreateElement("genre");
                XmlNode priceNode = xmlDoc.CreateElement("price");
                XmlNode publishYearNode = xmlDoc.CreateElement("publishYear");
                XmlNode descriptionNode = xmlDoc.CreateElement("description");

                authorNode.InnerText = Library.Books[ID].Author;
                titleNode.InnerText = Library.Books[ID].Title;
                genreNode.InnerText = Library.Books[ID].Genre;
                priceNode.InnerText = Library.Books[ID].Price.ToString();
                publishYearNode.InnerText = Library.Books[ID].PublishYear.ToString();
                descriptionNode.InnerText = Library.Books[ID].Description;

                bookNode.AppendChild(authorNode);
                bookNode.AppendChild(titleNode);
                bookNode.AppendChild(genreNode);
                bookNode.AppendChild(priceNode);
                bookNode.AppendChild(publishYearNode);
                bookNode.AppendChild(descriptionNode);

                rootNode.AppendChild(bookNode);
            }

            xmlDoc.Save("FilteredBooks.xml");
        }

        private void ToHTMLButton_Click(object sender, EventArgs e)
        {
            if (CheckIfXmlOpened())
            {
                CreateFilteredBooksXml();

                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load("Books.xslt");
                xslt.Transform("FilteredBooks.xml", "FilteredBooks.html");

                MessageBox.Show("Your file was successfully converted :)");
            }
        }

        private void AddItemsToComboBox()
        {
            void AddItem (ComboBox comboBox , string identifier)
            {
                if (!comboBox.Items.Contains(identifier))
                    comboBox.Items.Add(identifier);
            }

            foreach (Book book in Library.Books.Values)
            {
                AddItem(AuthorComboBox, book.Author);
                AddItem(TitleComboBox, book.Title);
                AddItem(GenreComboBox, book.Genre);
                AddItem(PublishYearComboBox, book.PublishYear.ToString());
            }

            SetPriceRange();
        }

        private void SetPriceRange()
        {
            PriceComboBox.Items.Add("0 - 10");
            PriceComboBox.Items.Add("10 - 30");
            PriceComboBox.Items.Add("30 - 50");
        }

        private void OpenHTML_Click(object sender, EventArgs e)
        {
            if (CheckIfXmlOpened())
            {
                string path = Directory.GetCurrentDirectory() + "\\books.html";
                
                try { Process.Start(path); }
                catch { MessageBox.Show("Something wrong with your html :c"); }
            }
        }
    }
}


/*
 * void ConvertControlToType <T> (List<Control> listToConvert , List<T> array) where T : Control
            {
                foreach (Control control in listToConvert)
                {
                    array.Add(control as T);
                }
            }
            
            List<Control> radioButtonsControl = Controls.OfType<RadioButton>().Cast<Control>().ToList(); // находим все Control типа RadioButton
            List<Control> comboBoxControl = Controls.OfType<ComboBox>().Cast<Control>().ToList();
            List<Control> checkBoxControl = Controls.OfType<CheckBox>().Cast<Control>().ToList();

            ConvertControlToType(radioButtonsControl , _radioButtons); // конвертируем Contol в RadioButton
            ConvertControlToType(comboBoxControl, _comboBoxes);
            ConvertControlToType(checkBoxControl, _checkBoxes);

            _radioButtons.Reverse(); // хз почему, но Controls.OfType находит их в обратном порядке
            _comboBoxes.Reverse();
            _checkBoxes.Reverse();
 */




/*
 * XslCompiledTransform xslt = new XslCompiledTransform();

                string copyFrom = Library.XmlDocument.BaseURI.Replace("file:///", "");
                string copyTo = copyFrom.Replace(Library.FileName, "bin/Debug/" + Library.FileName);

                File.Copy(copyFrom, copyTo, true); // копируем xml в папку к нашему exe. Это нужно, чтобы правильно сработал Transform

                xslt.Load("Books.xslt");
                xslt.Transform("books.xml", "books.html");

                MessageBox.Show("Your file was successfully converted :)");
 */