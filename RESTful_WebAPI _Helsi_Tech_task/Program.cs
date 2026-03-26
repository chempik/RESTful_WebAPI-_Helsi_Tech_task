using Infrastructure;
using Infrastructure.Repositories;
using Domain.Interfaces;
using Application.Services;
using Application.Mappings;
using FluentValidation;
using Application.DTOs;
using RESTful_WebAPI__Helsi_Tech_task.Middleware;
using Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB
builder.Services.AddMongoDb(builder.Configuration);

// Repositories
builder.Services.AddScoped<ITaskListRepository, MongoTaskListRepository>();
builder.Services.AddScoped<ITaskListShareRepository, MongoTaskListShareRepository>();

// Services
builder.Services.AddScoped<ITaskListService, TaskListService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(TaskListProfile));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<TaskListCreateDtoValidator>();

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();