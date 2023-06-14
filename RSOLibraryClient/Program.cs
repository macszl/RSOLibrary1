using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceReference1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceClient libraryServiceClient = new ServiceClient();
                Console.WriteLine("1 - by title \n2 - by year\n3 - by author name");
                string type = Console.ReadLine();
                Book[] books;
                string years;
                string author;
                if (type == "1")
                {
                    Console.WriteLine("Set book name");
                    string bookTitle = Console.ReadLine();
                    books = libraryServiceClient.GetBooks(new GetBooksRequest(bookTitle)).GetBooksResult;
                }
                else if (type == "2")
                {
                    Console.WriteLine("Set year");
                    years = Console.ReadLine();
                    books = libraryServiceClient.GetBooksByYear(new GetBooksByYearRequest(years)).GetBooksByYearResult;
                }
                else if (type == "3")
                {
                    Console.WriteLine("Set author name");
                    author = Console.ReadLine();
                    books = libraryServiceClient.GetBooksByAuthor(new GetBooksByAuthorRequest(author)).GetBooksByAuthorResult;
                }
                else
                {
                    Console.WriteLine("Wrong type");
                    Console.ReadLine();
                    return;
                }

                List<Book> SortedList = books.OrderBy(o => o.publishedDate).ToList();
                foreach (Book book in SortedList)
                {
                    Console.WriteLine("Title = {0} | Id = {1}, Published Date = {2}", book.Title, book.Id, book.publishedDate);
                }
                Console.WriteLine("Set book id");
                Book newBook;
                string bookId = Console.ReadLine();
                newBook = libraryServiceClient.GetBookDetails(new GetBookDetailsRequest(bookId)).GetBookDetailsResult;
                Console.WriteLine("Title = {0} \n Id = {1} \n Description = {2} \n Authors =", newBook.Title, newBook.Id, newBook.Description);

                foreach (string newBookAuthor in newBook.Authors)
                {
                    Console.Write(" ,{0}", newBookAuthor);
                }

                Console.ReadLine();
                libraryServiceClient.Close();
            }
            catch (FaultException<NoSuchBookFault> ex)
            {
                Console.WriteLine("No such book: " + ex.Detail.ProblemType);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}
