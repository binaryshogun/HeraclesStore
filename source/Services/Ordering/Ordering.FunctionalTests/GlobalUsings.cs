global using Xunit;

global using System.Net;
global using System.Net.Http.Json;
global using System.Text.Encodings.Web;
global using System.Reflection;
global using System.Security.Claims;

global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.AspNetCore.Mvc.Testing;

global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;

global using Microsoft.EntityFrameworkCore;

global using Ordering.Api.Application.Commands;
global using Ordering.Infrastructure;

global using Ordering.FunctionalTests.Auth;
global using Ordering.FunctionalTests.Data;
global using Ordering.FunctionalTests.Extensions;
global using Ordering.FunctionalTests.Services;