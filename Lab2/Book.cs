using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2
{
    public class Book
    {
        public string Author;
        public string Title;
        public string Genre;
        public double Price;
        public int PublishYear;
        public string Description;

        public Book()
        {
            Author = "";
            Title = "";
            Genre = "";
            Price = 0;
            PublishYear = 0;
            Description = "";
        }
    }
}
