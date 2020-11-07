using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Lab2
{
    static class Library
    {
        public static XmlDocument XmlDocument; // будем обращатся к xml документу из Sax, Dom, Linq

        public static Dictionary<string, Book> Books = new Dictionary<string, Book>(); // у каждой книги есть уникальный id

        public static int PriceRangeFrom;
        public static int PriceRangeTo;


        public static bool TryOpenXml(string filepath)
        {
            XmlDocument xml = new XmlDocument();
            bool noEror = true;

            try
            {
                xml.Load(filepath);
                XmlDocument = xml;
            }

            catch { MessageBox.Show("Something wrong with your file :c"); noEror = false; }

            return noEror;
        }

        public static void SetPriceRange(string priceRangeString)
        {
            if (priceRangeString != null) // если строка не пустая
            {
                Regex regex = new Regex("[0-9]+");
                MatchCollection priceRangeNumbers = regex.Matches(priceRangeString);

                PriceRangeFrom = int.Parse(priceRangeNumbers[0].Value);
                PriceRangeTo = int.Parse(priceRangeNumbers[1].Value);
            }

            else
                PriceRangeFrom = -1; // флаг, который показывет, что цена не критерий
        }

        static public List<string> SearchByCriteria (Book criteria) // возращает список id
        {
            List<string> answer = new List<string>();

            foreach (var bookWithId in Books)
            {
                Book book = bookWithId.Value;

                bool matches = CompareParameters(book.Author, criteria.Author) &&
                               CompareParameters(book.Title, criteria.Title) &&
                               CompareParameters(book.Genre, criteria.Genre) &&
                               CompareParameters(book.PublishYear, criteria.PublishYear);

                if (PriceRangeFrom != -1)
                    matches = matches && (PriceRangeFrom <= book.Price && book.Price <= PriceRangeTo);

                if (matches)
                    answer.Add(bookWithId.Key);
            }

            return answer;
        }

        private static bool CompareParameters(object parameter , object criteriaParameter)
        {
            if (criteriaParameter != null)
            {
                criteriaParameter = criteriaParameter.ToString(); // у нас есть PublishYear типа int 
                parameter = parameter.ToString();

                if ((string)criteriaParameter != "0") // если не критерий (0 по умолчанию)
                {
                    return Equals(criteriaParameter, parameter);
                }    

                else
                    return true;
            }

            else
                return true; // если нет критерия (null по умолчанию)
        }

    }
}


