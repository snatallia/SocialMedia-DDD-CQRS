using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Producers;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

Action<DbContextOptionsBuilder> configeDbContext = (o => o.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
builder.Services.AddDbContext<DatabaseContext>(configeDbContext);
builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configeDbContext));

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddScoped<IEventConsumer, EventConsumer>();

builder.Services.AddControllers();
builder.Services.AddHostedService<ConsumerHostedService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Create database tables from code 
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
dbContext.Database.EnsureCreated();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();
