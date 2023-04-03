global using Serilog;

global using Identity.Api;
global using Identity.Api.Data;
global using Identity.Api.Services;
global using Identity.Api.Models;
global using Identity.Api.Exceptions;
global using Identity.Api.Filters;

global using Microsoft.IdentityModel.Tokens;

global using Microsoft.EntityFrameworkCore;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;

global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using HealthChecks.UI.Client;

global using System.Text;
global using System.Globalization;
global using System.Security.Claims;
global using System.IdentityModel.Tokens.Jwt;
global using System.ComponentModel.DataAnnotations;