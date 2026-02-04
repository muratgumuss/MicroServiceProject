using Aspire.Hosting;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = DistributedApplication.CreateBuilder(args);
#region RabbtitMQ
var rabbitMqUserName = builder.AddParameter("RABBITMQ-USERNAME");
var rabbitMqPassword = builder.AddParameter("RABBITMQ-PASSWORD");
var rabbitMq = builder.AddRabbitMQ("rabbitMQ", rabbitMqUserName, rabbitMqPassword, 5672)
    .WithManagementPlugin(15675);
#endregion

#region Keycloak
var postgresUserName = builder.AddParameter("POSTGRES-USERNAME");
var postgresPassword = builder.AddParameter("POSTGRES-PASSWORD");
var postgresKeycloakDb = "keycloak-db";
var postgresDb = builder
    .AddPostgres("postgres-db-keycloak", postgresUserName, postgresPassword, 5432)
    .WithImage("postgres", "16.2")
    .WithDataVolume("postgres.db.keycloak.volume")
    .AddDatabase(postgresKeycloakDb);

var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "25.0")
.WithEnvironment("KEYCLOAK_ADMIN", "admin")
.WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "password")
.WithEnvironment("KC_DB", "postgres")
.WithEnvironment("KC_DB_URL", $"jdbc:postgresql://postgres-db-keycloak/keycloak-db")
.WithEnvironment("KC_DB_USERNAME", postgresUserName)
.WithEnvironment("KC_DB_PASSWORD", postgresPassword)
.WithEnvironment("KC_HOSTNAME_PORT", "8080")
.WithEnvironment("KC_HTTP_ENABLED", "true")
.WithEnvironment("KC_HOSTNAME_STRICT_HTTPS", "false")
.WithEnvironment("KC_HOSTNAME_STRICT", "false")
.WithEnvironment("KC_HEALTH_ENABLED", "true")
.WithArgs("start")
.WaitFor(postgresDb)
.WithHttpEndpoint(8080, 8080, "http");

var keycloakEndpoint = keycloak.GetEndpoint("keycloak-http-endpoint");

#endregion

#region Catalog-Api
var mongoUser = builder.AddParameter("MONGO-USERNAME");
var mongoPassword = builder.AddParameter("MONGO-PASSWORD");
var mongoCatalogDb = builder.AddMongoDB("mongo-db-catalog", 27030, mongoUser, mongoPassword)
    .WithImage("mongo:8.0-rc")
    .WithDataVolume("mongo.db.catalog.volume")
    .AddDatabase("catalog-db");

var catalogApi = builder.AddProject<Projects.MicroServiceApp_Catalog_Api>("microserviceapp-catalog-api");
catalogApi
    .WithReference(mongoCatalogDb)
    .WaitFor(mongoCatalogDb)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(keycloakEndpoint);
#endregion

#region Basket-Api
var redisPassword = builder.AddParameter("REDIS-PASSWORD");
var redisBasketDb = builder.AddRedis("redis-db-basket", 6379, redisPassword)
    .WithImage("redis:7.0-alpine")
    .WithDataVolume("redis.db.basket.volume")
    .WithPassword(redisPassword);

var basketApi = builder.AddProject<Projects.MicroServiceApp_Basket_Api>("microserviceapp-basket-api");
basketApi
    .WithReference(redisBasketDb)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(keycloakEndpoint);
#endregion

#region Discount-Api

var mongoDiscountDb = builder.AddMongoDB("mongo-db-discount", 27034, mongoUser, mongoPassword)
    .WithImage("mongo:8.0-rc")
    .WithDataVolume("mongo.db.discount.volume")
    .AddDatabase("discount-db");

var discountApi = builder.AddProject<Projects.MicroServiceApp_Discount_Api>("microserviceapp-discount-api");
discountApi
    .WithReference(mongoDiscountDb)
    .WaitFor(mongoDiscountDb)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(keycloakEndpoint);
#endregion

#region File-Api
var fileApi = builder
    .AddProject<Projects.MicroServiceApp_File_Api>("microserviceapp-file-api")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(keycloakEndpoint);
#endregion

#region Payment-Api
var paymentApi = builder
    .AddProject<Projects.MicroServiceApp_Payment_Api>("microserviceapp-payment-api")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(keycloakEndpoint);
#endregion

#region Order-Api
var sqlServerPassword = builder.AddParameter("SQLSERVER-SA-PASSWORD");
var sqlServerOrderDb = builder.AddSqlServer("sqlserver-db-order")
    .WithDataVolume("sqlserver.db.order.volume")
    .WithPassword(sqlServerPassword)
    .AddDatabase("order-db-aspire");

var orderApi = builder.AddProject<Projects.MicroServiceApp_Order_Api>("microserviceapp-order-api");
orderApi
    .WithReference(sqlServerOrderDb)
    .WaitFor(sqlServerOrderDb)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(keycloakEndpoint);
#endregion

#region Gateway-Api
builder.AddProject<Projects.MicroServiceApp_Gateway>("microserviceapp-gateway").WithReference(keycloakEndpoint);
#endregion

#region Web-Api
var web = builder.AddProject<Projects.MicroServiceApp_Web>("microserviceapp-web");
web.WithReference(catalogApi)
    .WithReference(basketApi)
    .WithReference(discountApi)
    .WithReference(fileApi)
    .WithReference(paymentApi)
    .WithReference(orderApi)
    .WithReference(keycloakEndpoint);
#endregion

builder.Build().Run();
