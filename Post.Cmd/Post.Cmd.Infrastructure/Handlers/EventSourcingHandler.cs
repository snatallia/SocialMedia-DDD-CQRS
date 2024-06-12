using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        private readonly IEventStore eventStore;

        public EventSourcingHandler(IEventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
        {
            var aggregate = new PostAggregate();
            var events = await eventStore.GetEventsAsync(aggregateId);

            if (events == null || !events.Any())
                return aggregate;

            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(e => e.Version).Max();
            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregate)
        {
            await eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version);
            aggregate.MarkChangesAsCommitted();
        }
    }
}
