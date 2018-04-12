using FlatMapper;
using System;

namespace SampleApp
{
    class Program
    {
        public class Author
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Cover
        {
            public string Color { get; set; }
            public string FontFamily { get; set; }
        }

        public class Book
        {
            public string Title { get; set; }
            public Author BookAuthor { get; set; }
            public Cover BookCover { get; set; }
        }

        public class BookDTO
        {
            public string Title { get; set; }

            [NestedMap(TargetProperty = "BookAuthor", Path = "BookAuthor.FirstName")]
            public string AuthorFName { get; set; }
            [NestedMap(TargetProperty = "BookAuthor", Path = "BookAuthor.LastName")]
            public string AuthorLName { get; set; }

            [NestedMap(TargetProperty = "BookCover", Path = "BookCover.Color")]
            public string BookColor { get; set; }

            [NestedMap(TargetProperty = "BookCover", Path = "BookCover.FontFamily")]
            public string BookFontFamily { get; set; }
        }

        static void Main(string[] args)
        {
            var book = new Book()
            {
                BookAuthor = new Author() { FirstName = "Colin", LastName = "Wallambery" },
                BookCover = new Cover() { FontFamily = "Calibri", Color = "Orange" },
                Title = "Fire and Claws"
            };

            var mapper = new Mapper();

            var result =  mapper.Map<BookDTO>(book);

            Console.WriteLine(result.Title);
            Console.WriteLine(result.BookColor);
            Console.WriteLine(result.BookFontFamily);
            Console.WriteLine(result.AuthorFName + " " + result.AuthorLName);
            Console.ReadLine();
        }
    }
}
