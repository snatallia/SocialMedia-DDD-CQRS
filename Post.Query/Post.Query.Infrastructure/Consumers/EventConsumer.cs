using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig config;
        private readonly IEventHandler eventHandler;

        public EventConsumer(IOptions<ConsumerConfig> options, IEventHandler eventHandler)
        {
            config = options.Value;
            this.eventHandler = eventHandler;
        }
        public void Consume(string topic)
        {
            using var consumer = new ConsumerBuilder<string,string>(config)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();
            consumer.Subscribe(topic);

            while (true)
            {
                var result = consumer.Consume();
                if (result?.Message == null) continue;

                var options = new JsonSerializerOptions
                {
                    Converters = { new EventJsonConverter()}
                };
                
                var @event = JsonSerializer.Deserialize<BaseEvent>(result.Message.Value,options);
                var handlerMethod = eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });

                if (handlerMethod == null) 
                {
                    throw new ArgumentNullException(nameof(handlerMethod),"Could not find event handler method!");
                }

                handlerMethod.Invoke(eventHandler, new object[] { @event });
                consumer.Commit(result);

            }
        }
    }
}
