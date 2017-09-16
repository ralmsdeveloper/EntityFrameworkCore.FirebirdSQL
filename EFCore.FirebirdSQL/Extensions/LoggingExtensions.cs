/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSQL
 *
 * THIS MATERIAL IS PROVIDED AS IS, WITH ABSOLUTELY NO WARRANTY EXPRESSED
 * OR IMPLIED.  ANY USE IS AT YOUR OWN RISK.
 * 
 * Permission is hereby granted to use or copy this program
 * for any purpose,  provided the above notices are retained on all copies.
 * Permission to modify the code and to distribute modified code is granted,
 * provided the above notices are retained, and a notice that the code was
 * modified is included with the above copyright notice.
 *
 */

 namespace Microsoft.Extensions.Logging
{
    internal static class LoggingExtensions
    {
        public const string CommandsLoggerName = "EntityFramework.Commands";

        public static ILogger CreateCommandsLogger(this ILoggerFactory loggerFactory)
            => loggerFactory.CreateLogger(CommandsLoggerName);

        public static ILogger CreateCommandsLogger(this ILoggerProvider loggerProvider)
            => loggerProvider.CreateLogger(CommandsLoggerName);
    }
}
