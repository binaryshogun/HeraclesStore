global using Catalog.Api.Exceptions;
global using Catalog.Api.Models;
global using Catalog.Api.Data;
global using Catalog.Api.Data.EntityConfigurations;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Data.SqlClient;

global using Polly;
global using Polly.Retry;

global using Serilog;