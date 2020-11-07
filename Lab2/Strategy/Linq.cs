using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Lab2
{
    class LinqRead : ISearchStrategy
    {
        private Dictionary<string, Book> _books = new Dictionary<string, Book>();

        public Dictionary<string, Book> GetBooks()
        {
            XDocument xdocument = XDocument.Load(Library.XmlDocument.BaseURI);
            IEnumerable<XElement> xmlBooks = xdocument.Root.Elements();

            foreach (var xmlBook in xmlBooks)
                SetBook(xmlBook);

            return _books;
        }

        private void SetBook(XElement xmlBook)
        {
            string id = xmlBook.Attribute("id").Value;

            _books[id] = new Book();
            _books[id].Author = xmlBook.Element("author").Value;
            _books[id].Title = xmlBook.Element("title").Value;
            _books[id].Genre = xmlBook.Element("genre").Value;
            _books[id].Price = Convert.ToDouble(xmlBook.Element("price").Value);
            _books[id].PublishYear = Convert.ToInt32(xmlBook.Element("publishYear").Value);
            _books[id].Description = xmlBook.Element("description").Value;
        }
    }
}
