using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
                
                var matchesBooks = Library.SearchByCriteria(criteria);

                ShowMatchBooks(matchesBooks);
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

        private void ToHTMLButton_Click(object sender, EventArgs e)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();

            string copyFrom = Library.XmlDocument.BaseURI;
            string copyTo = Directory.GetCurrentDirectory() + "\\Book.html";

            File.Copy(copyFrom, copyTo);

            xslt.Load("https://portal.rosreestr.ru/xsl/EGRP_FIR/Reestr_Extract_Object/03/Common.xsl");
            xslt.Transform("Books.xml", "Books.html");
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