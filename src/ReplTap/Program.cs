using System;
using Microsoft.Extensions.DependencyInjection;
using ReplTap;
using ReplTap.ConsoleHost;

if (Console.IsInputRedirected)
{
    Console.WriteLine("Error! Either input is redirected from standard input or app doesn't have console. Unable to run console app.");

    return;
}

var version = typeof(Configuration)
    .Assembly
    .GetName()
    .Version?
    .ToString()[..^2];

Console.WriteLine($"repltap - C# interactive repl - v{version}");

var provider = Configuration.GetServiceProvider();
var interactiveLoop = provider.GetRequiredService<IInteractiveLoop>();

await interactiveLoop.Run();
