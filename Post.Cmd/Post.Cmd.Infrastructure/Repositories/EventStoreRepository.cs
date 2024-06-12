using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> eventStoreCollection;
        public EventStoreRepository(IOptions<MongoDbConfig> config)
        {
            var mongoCLient = new MongoClient(config.Value.ConnectionString);
            var mongoDatabase = mongoCLient.GetDatabase(config.Value.Database);
            eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
        }
        public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        {
            return await eventStoreCollection.Find(x=>x.AggregateIdentifier == aggregateId).ToListAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(EventModel eventModel)
        {
            await eventStoreCollection.InsertOneAsync(eventModel).ConfigureAwait(false);
        }
    }
}
