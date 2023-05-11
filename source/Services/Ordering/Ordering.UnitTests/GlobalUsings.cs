global using Xunit;

global using Moq;

global using MediatR;

global using System.Security.Claims;

global using Ordering.Api.Controllers;
global using Ordering.Api.Application.Commands;
global using Ordering.Api.Application.Queries;
global using Ordering.Api.Application.Queries.Records;

global using Ordering.Domain.SeedWork;
global using Ordering.Domain.Exceptions;
global using Ordering.Domain.Models.OrderAggregate;
global using Ordering.Domain.Models.BuyerAggregate;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Http;

global using Microsoft.Extensions.Logging;

global using Ordering.UnitTests.Domain.Models.Builders;