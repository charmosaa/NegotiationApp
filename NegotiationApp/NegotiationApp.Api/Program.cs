using MediatR;
using FluentValidation;
using NegotiationApp.Application.Behaviors;
using NegotiationApp.Application.Features.Products.Commands;
using NegotiationApp.Application.Features.Products.Queries;
using NegotiationApp.Application.Features.Auth;
using NegotiationApp.Application.DTOs;
using NegotiationApp.Domain.Repositories;
using NegotiationApp.Infrastructure.Persistence.InMemory;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using NegotiationApp.Application.Validators;
using NegotiationApp.Application.Features.Negotiations.Commands;
using NegotiationApp.Application.Features.Negotiations.Queries;
using NegotiationApp.Domain.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NegotiationApp.Application.Services;
using NegotiationApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);


// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())); 
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly)); 

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateProductCommandValidator).Assembly); 
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); 

// Repozytoria w pamiêci
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<INegotiationRepository, InMemoryNegotiationRepository>();
builder.Services.AddSingleton<ISecurityService, SecurityService>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmployeePolicy", policy => policy.RequireRole("Employee"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// API Endpoints Product
app.MapPost("/products", async ([FromBody] CreateProductDto request, IMediator mediator) =>
{
    try
    {
        var command = new CreateProductCommand(request.Name, request.BasePrice);
        var product = await mediator.Send(command);
        return Results.Created($"/products/{product.Id}", product);
    }
    catch (ValidationException ex)
    {
        return Results.BadRequest(ex.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }));
    }
    catch (Exception)
    {
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithName("CreateProduct")
.WithOpenApi();

app.MapGet("/products/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var query = new GetProductByIdQuery(id);
    var product = await mediator.Send(query);
    return product != null ? Results.Ok(product) : Results.NotFound();
})
.WithName("GetProductById")
.WithOpenApi();

app.MapGet("/products", async (IMediator mediator) =>
{
    var query = new GetProductsListQuery();
    var products = await mediator.Send(query);
    return Results.Ok(products);
})
.WithName("GetProductsList")
.WithOpenApi();

// API Endpoints for Negotiations
app.MapPost("/negotiations/start", async ([FromBody] StartNegotiationCommand command, IMediator mediator) =>
{
    try
    {
        var negotiation = await mediator.Send(command);
        return Results.Created($"/negotiations/{negotiation.Id}", negotiation);
    }
    catch (ValidationException ex)
    {
        return Results.BadRequest(ex.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }));
    }
    catch (ProductNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (Exception)
    {
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithName("StartNegotiation")
.WithOpenApi();

app.MapPost("/negotiations/{id:guid}/propose-price", async (Guid id, [FromBody] ProposePriceDto request, IMediator mediator) =>
{
    try
    {
        var command = new ProposePriceCommand(id, request.ProposedPrice);
        var negotiation = await mediator.Send(command);
        return Results.Ok(negotiation);
    }
    catch (ValidationException ex)
    {
        return Results.BadRequest(ex.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }));
    }
    catch (NegotiationNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (InvalidNegotiationStateException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (InvalidProposedPriceException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (NegotiationAttemptsExceededException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (ClientResponseTimeExceededException ex)
    {
        return Results.Conflict(ex.Message); // conflict - negotiation cancelled
    }
    catch (Exception)
    {
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithName("ProposePrice")
.WithOpenApi();

app.MapGet("/negotiations/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var query = new GetNegotiationByIdQuery(id);
    var negotiation = await mediator.Send(query);
    return negotiation != null ? Results.Ok(negotiation) : Results.NotFound();
})
.WithName("GetNegotiationById")
.WithOpenApi();

app.MapGet("/products/{productId:guid}/negotiations", async (Guid productId, IMediator mediator) =>
{
    var query = new GetNegotiationsByProductIdQuery(productId);
    var negotiations = await mediator.Send(query);
    return Results.Ok(negotiations);
})
.WithName("GetNegotiationsByProductId")
.WithOpenApi();


app.MapPost("/negotiations/{id:guid}/accept", async (Guid id, IMediator mediator) =>
{
    try
    {
        var command = new AcceptNegotiationCommand(id);
        var negotiation = await mediator.Send(command);
        return Results.Ok(negotiation);
    }
    catch (NegotiationNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (InvalidNegotiationStateException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception)
    {
        return Results.Problem("An unexpected error occurred.");
    }
})
.RequireAuthorization("EmployeePolicy") 
.WithName("AcceptNegotiation")
.WithOpenApi();


app.MapPost("/negotiations/{id:guid}/reject", async (Guid id, IMediator mediator) =>
{
    try
    {
        var command = new RejectNegotiationCommand(id);
        var negotiation = await mediator.Send(command);
        return Results.Ok(negotiation);
    }
    catch (NegotiationNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (InvalidNegotiationStateException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception)
    {
        return Results.Problem("An unexpected error occurred.");
    }
})
.RequireAuthorization("EmployeePolicy")
.WithName("RejectNegotiation")
.WithOpenApi();

app.MapPost("/negotiations/{id:guid}/cancel", async (Guid id, IMediator mediator) =>
{
    try
    {
        var command = new CancelNegotiationCommand(id);
        var negotiation = await mediator.Send(command);
        return Results.Ok(negotiation);
    }
    catch (NegotiationNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (InvalidNegotiationStateException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception)
    {
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithName("CancelNegotiation")
.WithOpenApi();

// Authentication
app.MapPost("/login", ([FromBody] LoginRequest request, ISecurityService securityService) =>
{
    // uproszczona wersja auth
    if (request.Username == "employee" && request.Password == "password")
    {
        var token = securityService.GenerateJwtToken(request.Username, "Employee");
        return Results.Ok(new LoginResponse { Token = token });
    }
    return Results.Unauthorized();
})
.WithName("Login")
.WithOpenApi();

app.Run();