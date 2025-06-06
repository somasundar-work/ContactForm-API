using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;

var builder = WebApplication.CreateBuilder(args);

#region Configure AWS Options
var awsOptions = builder.Configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
#endregion

builder
    .Services.AddFastEndpoints()
    .AddVersioning(o =>
    {
        o.DefaultApiVersion = new(1.0);
        o.AssumeDefaultVersionWhenUnspecified = true;
        o.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
    });

VersionSets.CreateApi(">>ContactForm<<", v => v.HasApiVersion(new(1.0)));
builder.Services.AddEndpointsApiExplorer();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapFastEndpoints(options =>
{
    options.Endpoints.RoutePrefix = "api";
    options.Versioning.Prefix = "v";
    options.Versioning.DefaultVersion = 1;
});

app.MapGet("/", () => "Welcome to the Contact Form API!");

app.Run();
