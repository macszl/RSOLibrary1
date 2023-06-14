using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RsoLib
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        Task<List<Book>> GetBooks(string query);

        [OperationContract]
        [FaultContract(typeof(NoSuchBookFault))]
        Task<Book> GetBookDetails(string id);
        // TODO: Add your service operations here

        [OperationContract]
        [FaultContract(typeof(NoSuchBookFault))]
        Task<List<Book>> GetBooksByYear(string year);
        // TODO: Add your service operations here

        [OperationContract]
        [FaultContract(typeof(NoSuchBookFault))]
        Task<List<Book>> GetBooksByAuthor(string author);
        // TODO: Add your service operations here
    }

    [DataContract]
    public class NoSuchBookFault
    {
        [DataMember]
        public string Operation
        {
            get;
            set;
        }
        [DataMember]
        public string ProblemType
        {
            get;
            set;
        }

    }

    [DataContract]
    public class Book
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<string> Authors { get; set; }

        [DataMember]
        public string publishedDate { get; set; }
    }

}