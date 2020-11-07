using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    public enum Method
    {
        SAX,
        DOM,
        LINQ
    }


    public interface ISearchStrategy
    {
        Dictionary<string, Book> GetBooks(); // книги с их id 
    }


    public class Context
    {
        private ISearchStrategy _strategy;

        public static List<ISearchStrategy> Strategies = new List<ISearchStrategy>
        {
            new SaxRead() ,
            new DomRead() ,
            new LinqRead() ,
        };


        public void SetStrategy(ISearchStrategy strategy)
        {
            _strategy = strategy;;
        }

        public Dictionary<string , Book> GetBooks()
        {
            return _strategy.GetBooks();
        }
    }
}

