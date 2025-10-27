using MongoDB.Bson.Serialization.Attributes;

namespace MicroServiceApp.Catalog.Api.Repositories
{
    public class BaseEntity
    {
        //Snowflake ID generation can be implemented here if needed
        [BsonElement("_id")]
        public Guid Id { get; set; }
    }
}
