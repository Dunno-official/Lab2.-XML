using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Lab2
{
    class SaxRead : ISearchStrategy
    {
        private Dictionary<string, Book> _books = new Dictionary<string, Book>();

        public Dictionary<string, Book> GetBooks()
        {
            XmlReader reader = XmlReader.Create(Library.XmlDocument.BaseURI);
            reader.MoveToContent();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "book")
                    {
                        string id = reader.GetAttribute("id");
                        _books[id] = new Book();

                        SetBook(reader, id);
                    }   
                }
            }

            return _books;
        }

        private void SetBook(XmlReader reader , string id) // если поменять порядок вызова метода, все поломается :)
        {
            _books[id].Author = GoToNextText(reader); 
            _books[id].Title  = GoToNextText(reader);
            _books[id].Genre  = GoToNextText(reader);
            _books[id].Price = Convert.ToDouble(GoToNextText(reader));
            _books[id].PublishYear = Convert.ToInt32(GoToNextText(reader));
            _books[id].Description = GoToNextText(reader);
        }


        private string GoToNextText(XmlReader reader) // читаем, пока не находим текст
        {
            do
            {
                reader.Read();

            } while (reader.NodeType != XmlNodeType.Text);


            return reader.Value;
        }
    }
}
