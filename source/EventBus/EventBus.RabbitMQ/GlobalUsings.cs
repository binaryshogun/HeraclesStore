global using RabbitMQ.Client;
global using RabbitMQ.Client.Events;
global using RabbitMQ.Client.Exceptions;

global using Polly;
global using Polly.Retry;

global using System.Text;
global using System.Text.Json;
global using System.Net.Sockets;

global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.DependencyInjection;

global using EventBus.Abstractions;
global using EventBus.Abstractions.Events;
global using EventBus.Abstractions.Extensions;