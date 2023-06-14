using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;

namespace RsoLib
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Service : IService
    {
        public async Task<List<Book>> GetBooks(string query)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://www.googleapis.com/books/v1/volumes?q=intitle:{query}&fields=items(id,volumeInfo/title)";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject(responseBody);
                if (data.items != null)
                {
                    List<Book> result = new List<Book>();
                    foreach (dynamic item in data.items)
                    {
                        Book book = ExtractBookDetails(item);
                        Console.WriteLine($"Book Title: {book.Title}");
                        Console.WriteLine($"Book Description: {book.Description}");
                        Console.WriteLine($"Authors: {string.Join(", ", book.Authors)}");
                        Console.WriteLine();
                        result.Add(book);
                    }
                    return result;
                }
                else
                {
                    Console.WriteLine("No books found.");
                    return null;
                }
            }
        }

        public async Task<Book> GetBookDetails(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://www.googleapis.com/books/v1/volumes/{id}";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject(responseBody);

                if (data != null)
                {
                    Book book = ExtractBookDetails(data);
                    Console.WriteLine($"Book Title: {book.Title}");
                    Console.WriteLine($"Book Description: {book.Description}");
                    Console.WriteLine($"Authors: {string.Join(", ", book.Authors)}");
                    Console.WriteLine();
                    return book;
                }
                else
                {
                    NoSuchBookFault nsbf = new NoSuchBookFault();
                    nsbf.Operation = "Search";
                    nsbf.ProblemType = "Book not found";
                    throw new FaultException<NoSuchBookFault>(nsbf);
                }
            }
        }

        public async Task<List<Book>> GetBooksByYear(string year)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://www.googleapis.com/books/v1/volumes?q=intitle:{year}%20edition&maxResults=40";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject(responseBody);
                if (data != null && data.items != null && data.items.Count > 0)
                {
                    List<Book> result = new List<Book>();
                    foreach (dynamic item in data.items)
                    {
                        Book book = ExtractBookDetails(item);
                        if( book.publishedDate != null && book.publishedDate.Length > 3 && book.publishedDate.Substring(0, 4) == year)
                        {
                            result.Add(book);
                            Console.WriteLine('a');
                        }
                    }
                    return result;
                }
                else
                {
                    throw new FaultException<NoSuchBookFault>(new NoSuchBookFault
                    {
                        Operation = "GetBooksByYear",
                        ProblemType = "No book found for the specified year"
                    });
                }
            }
        }

        public async Task<List<Book>> GetBooksByAuthor(string author)
        {
            using (HttpClient client = new HttpClient())
            {
                string encodedAuthor = Uri.EscapeDataString(author);
                string url = $"https://www.googleapis.com/books/v1/volumes?q=inauthor:{encodedAuthor}";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject(responseBody);
                if (data != null && data.items != null && data.items.Count > 0)
                {
                    List<Book> result = new List<Book>();
                    foreach (dynamic item in data.items)
                    {
                        Book book = ExtractBookDetails(item);
                        result.Add(book);
                    }
                    return result;
                }
                else
                {
                    throw new FaultException<NoSuchBookFault>(new NoSuchBookFault
                    {
                        Operation = "GetBooksByAuthor",
                        ProblemType = "No book found for the specified author"
                    });
                }
            }
        }

        private static Book ExtractBookDetails(dynamic bookData)
        {
            List<string> authorList = new List<string>();
            if (bookData.volumeInfo.authors != null)
            {
                foreach (var author in bookData.volumeInfo.authors)
                {
                    authorList.Add(author.ToString());
                }
            }

            Book book = new Book
            {
                Id = bookData.id,
                Title = bookData.volumeInfo.title,
                Description = bookData.volumeInfo.description,
                Authors = authorList,
                publishedDate = bookData.publishedDate,
            };

            Console.WriteLine($"Book Title: {book.Title}");
            Console.WriteLine($"Book Description: {book.Description}");
            Console.WriteLine($"Authors: {string.Join(", ", book.Authors)}");
            Console.WriteLine($"Date: {book.publishedDate}");

            return book;
        }
    }
}