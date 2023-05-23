global using Serilog;

global using Grpc.Core;
global using Grpc.Core.Interceptors;

global using System.Text;

global using System.Net.Http.Headers;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.JwtBearer;

global using Microsoft.IdentityModel.Tokens;

global using Microsoft.Extensions.Diagnostics.HealthChecks;

global using Microsoft.OpenApi.Models;

global using Bff.Web.Grpc;
global using Bff.Web.Infrastructure;
global using Bff.Web.Services.Catalog;
global using Bff.Web.Services.Basket;
global using Bff.Web.Services.Ordering;
global using Bff.Web.Models.Catalog;
global using Bff.Web.Models.Basket;
global using Bff.Web.Models.Ordering;