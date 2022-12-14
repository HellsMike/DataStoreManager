# DataStoreManager

![**C#\ASP.NET Core**](https://img.shields.io/badge/C%23-ASP.NET%20Core-brightgreen)

DataStoreManager emulate a warehouse management software with input/output orders, respective pallets and items details.
You can interface with the different services through the API Gateway on port 7219.
This project is done with the purpose of learning microservices architecture; although it works this project is not suitable for a real warehouse because it is developed in a too semplistic way for a real situation.

### Requirements
- A locally deployed MongoDB instance on port 27017
- A locally deployed SQLServer instance
- A locally deployed Apache Kafka instance on port 9092
- Microsoft.Extensions.Hosting 6.0.0
- Microsoft.EntityFrameworkCore 6.0.0
- Microsoft.EntityFrameworkCore.Tools 6.0.0
- Microsoft.EntityFrameworkCore.SqlServer 6.0.0
- Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.14.0
- Swashbuckle.AspNetCore 6.2.3
- MongoDB.Driver 2.14.1
- Confluent.Kafka 1.8.2
- Ocelot 17.0.0
- Serilog.AspNetCore 4.1.0
- Serilog.Expressions 3.2.1
- Serilog.Settings.Configuration 3.3.0
- Serilog.Sinks.Console 4.0.1
- Serilog.Sinks.File 5.0.0
