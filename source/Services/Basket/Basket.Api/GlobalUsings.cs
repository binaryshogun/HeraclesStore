global using Basket.Api;
global using Basket.Api.Integration;
global using Basket.Api.Models;
global using Basket.Api.Exceptions;
global using Basket.Api.Data;
global using Basket.Api.Filters;
global using Basket.Api.Network;
global using Basket.Api.Grpc;
global using Basket.Api.IntegrationEvents.Events;
global using Basket.Api.IntegrationEvents.EventHandlers;

global using System.Net;

global using System.Text;

global using System.Text.Json;
global using System.ComponentModel.DataAnnotations;

global using System.Security.Claims;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;

global using Microsoft.AspNetCore.Server.Kestrel.Core;

global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;

global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using HealthChecks.UI.Client;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;

global using Microsoft.OpenApi.Models;

global using StackExchange.Redis;

global using Serilog;
global using Serilog.Context;

global using Grpc.Core;

global using RabbitMQ.Client;

global using EventBus.RabbitMQ;
global using EventBus.Abstractions.Events;
global using EventBus.Abstractions;