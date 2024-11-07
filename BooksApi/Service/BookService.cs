using BooksApi.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace BooksApi.Service
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _booksCollection;

        public BookService (IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration["MongoDBSettings:MongoDBConnectionString"]);
            var mongoDatabase = mongoClient.GetDatabase(configuration["MongoDBSettings:DatabaseName"]);
            _booksCollection = mongoDatabase.GetCollection<Book>(configuration["MongoDBSettings:CollectionName"]);
        }

        public async Task CreateAsync(Book book) =>
            await _booksCollection.InsertOneAsync(book);

        public async Task<List<Book>> GetAsync(int skip, int pageSize = 5) =>
        await _booksCollection
            .Find(_ => true)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();

        public async Task<List<Book>> GetAsync() =>
            await _booksCollection
            .Find(_ => true)
            .ToListAsync();

        public async Task<Book> GetAsync(string id) =>
            await _booksCollection.Find(book => book.Id == id).FirstOrDefaultAsync();

        public async Task UpdateAsync(string id, Book updatedBook) =>
            await _booksCollection.ReplaceOneAsync(book => book.Id == id, updatedBook);

        public async Task DeleteAsync(string id) =>
            await _booksCollection.DeleteOneAsync(book => book.Id == id);
    }
}
