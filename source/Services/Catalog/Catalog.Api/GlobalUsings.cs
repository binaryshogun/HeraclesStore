global using Polly;
global using Polly.Retry;

global using Serilog;

global using AutoMapper;

global using System.IO;
global using System.Text;
global using System.ComponentModel.DataAnnotations;

global using Catalog.Api;
global using Catalog.Api.Filters;
global using Catalog.Api.Exceptions;
global using Catalog.Api.Models;
global using Catalog.Api.Data;
global using Catalog.Api.Data.Dtos;
global using Catalog.Api.Data.EntityConfigurations;

global using Microsoft.Data.SqlClient;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;

global using Microsoft.OpenApi.Models;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;

global using Microsoft.IdentityModel.Tokens;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication.JwtBearer;

global using HealthChecks.UI.Client;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Diagnostics.HealthChecks;