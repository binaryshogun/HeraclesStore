global using Polly;
global using Polly.Retry;

global using MediatR;
global using MediatR.Extensions.Autofac.DependencyInjection;
global using MediatR.Extensions.Autofac.DependencyInjection.Builder;

global using Autofac;
global using Autofac.Extensions.DependencyInjection;

global using Dapper;

global using FluentValidation;

global using Serilog;
global using Serilog.Context;

global using RabbitMQ.Client;

global using System.Text;

global using System.Data;
global using System.Data.Common;

global using System.Runtime.Serialization;

global using System.ComponentModel.DataAnnotations;

global using Microsoft.OpenApi.Models;

global using Microsoft.AspNetCore.SignalR;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;

global using System.Security.Claims;

global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;

global using HealthChecks.UI.Client;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;

global using Microsoft.Data.SqlClient;

global using Microsoft.EntityFrameworkCore;

global using Ordering.Domain.SeedWork;
global using Ordering.Domain.Exceptions;
global using Ordering.Domain.Events;
global using Ordering.Domain.Models.BuyerAggregate;
global using Ordering.Domain.Models.OrderAggregate;

global using Ordering.Infrastructure;
global using Ordering.Infrastructure.Repositories;
global using Ordering.Infrastructure.Idempotency;

global using Ordering.Api;
global using Ordering.Api.SignalR;
global using Ordering.Api.Infrastructure;
global using Ordering.Api.Infrastructure.Modules;
global using Ordering.Api.Infrastructure.Filters;
global using Ordering.Api.Infrastructure.Services;
global using Ordering.Api.Application.Commands;
global using Ordering.Api.Application.Commands.Records;
global using Ordering.Api.Application.Models;
global using Ordering.Api.Application.Validators;
global using Ordering.Api.Application.Behaviors;
global using Ordering.Api.Application.Queries;
global using Ordering.Api.Application.Queries.Records;
global using Ordering.Api.Application.Events.Domain;
global using Ordering.Api.Application.Events.Records;
global using Ordering.Api.Application.Events.Integration;
global using Ordering.Api.Application.Events.Integration.Services;

global using EventBus.Abstractions;
global using EventBus.Abstractions.Events;
global using EventBus.Abstractions.Extensions;
global using EventBus.EventLogs;
global using EventBus.EventLogs.Services;
global using EventBus.RabbitMQ;