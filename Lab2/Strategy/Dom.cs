using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lab2
{
    class DomRead : ISearchStrategy
    {
        private Dictionary<string, Book> _books = new Dictionary<string, Book>();

        public Dictionary<string, Book> GetBooks()
        {
             XmlElement xRoot = Library.XmlDocument.DocumentElement;
             XmlNodeList xmlBookList = xRoot.SelectNodes("book");

            foreach (XmlNode xmlBook in xmlBookList)
            {
                var attributes = xmlBook.Attributes;
                string id = attributes[0].Value;
                SetBook(id, xmlBook);
            }

            return _books;
        }


        private void SetBook(string id, XmlNode xmlBook)
        {
            _books[id] = new Book();
            _books[id].Author = xmlBook.SelectSingleNode("author").InnerText;
            _books[id].Title = xmlBook.SelectSingleNode("title").InnerText;
            _books[id].Genre = xmlBook.SelectSingleNode("genre").InnerText;
            _books[id].Price = Convert.ToDouble(xmlBook.SelectSingleNode("price").InnerText);
            _books[id].PublishYear = Convert.ToInt32(xmlBook.SelectSingleNode("publishYear").InnerText);
            _books[id].Description = xmlBook.SelectSingleNode("description").InnerText;
        }
    }
}
