using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository eventStoreRepo;
        private readonly IEventProducer eventProducer;

        public EventStore(IEventStoreRepository eventStoreRepo, IEventProducer eventProducer)
        {
            this.eventStoreRepo = eventStoreRepo;
            this.eventProducer = eventProducer;
        }

        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await eventStoreRepo.FindByAggregateId(aggregateId);
            if (eventStream == null|| !eventStream.Any())
            {
                throw new AggregateNotFoundException("Not found aggregate. Incorrect post ID provided.");
            }

            return eventStream.OrderBy(e=>e.Version).Select(e=>e.EventData).ToList();
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await eventStoreRepo.FindByAggregateId(aggregateId);
            if (eventStream == null || !eventStream.Any())
            {
                throw new AggregateNotFoundException("Not found aggregate. Incorrect post ID provided.");
            }

            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
            {
                throw new ConcurrencyException();
            }
            var version = expectedVersion;

            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    AggregateIdentifier = aggregateId,
                    EventType = eventType,
                    Version = version,
                    AggregateType = nameof(PostAggregate),
                    EventData = @event,
                    TimeStamp = DateTime.Now
                };

                await eventStoreRepo.SaveAsync(eventModel);

                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                await eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}
