﻿using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using WebsiteValidator.BL.Classes;
using WebsiteValidator.BL.ExtensionMethods;
using WebsiteValidator.BL.Interfaces;

namespace WebsiteValidator
{
    class Program
    {
        static int Main(string[] args)
        {
            var urlOption = new Option<string>(new[] { "--url", "-u" }, description: "The url of the website you would like to crawl.");
            urlOption.IsRequired = true;

            var linksOption = new Option<bool>(new[] { "--links", "-l" }, description: "List all links that you can find.");
            var sslOption = new Option<bool>(new[] { "--ignore-ssl" }, description: "Ignores SSL certificate");
            var humanOption = new Option<bool>(new[] {"--human", "-h"}, "Human readable output (instead of json)");

            // Create a root command with some options
            var rootCommand = new RootCommand
            {
                urlOption,
                linksOption,
                sslOption,
                humanOption
            };

            rootCommand.Description = "WebsiteValidator, a tool to crawl a website and validate it";

            // Note that the parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create<string, bool, bool, bool>(ProcessCommand);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }

        private static void ProcessCommand(string url, bool links, bool ignoreSsl, bool human)
        {
            var outputHelper = new OutputHelperFactory().Get(human);
            
            if (links) ListLinksForUrl(url, ignoreSsl, outputHelper);
        }

        private static void ListLinksForUrl(string url, bool ignoreSsl, IOutputHelper outputHelper)
        {
            IDownloadAWebpage downloadWebpage = new DownloadAWebpage(ignoreSsl);

            var links =
                downloadWebpage
                    .Download(url)
                    .ExtractUrls()
                    .ToAbsoluteUrls(url);
            
            outputHelper.Write("links", links);
        }
    }
}
