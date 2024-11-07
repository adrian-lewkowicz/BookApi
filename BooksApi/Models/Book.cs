using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BooksApi.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("author")]
        public string Author { get; set; }

        [BsonElement("publicationDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PublicationDate { get; set; }

        [BsonElement("pages")]
        public int Pages { get; set; }

        [BsonElement("genre")]
        public string Genre { get; set; }

        [BsonElement("rating")]
        public double Rating { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("isbn")]
        public string ISBN { get; set; }

        [BsonElement("isAvailable")]
        public bool IsAvailable { get; set; }
    }
}
