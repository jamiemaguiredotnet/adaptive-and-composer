// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//


using Dialogs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Microsoft.Bot.Builder.Dialogs.Declarative.Types;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


    public class Startup
    {
    private IConfiguration Configuration { get; set; }
    private IHostingEnvironment HostingEnvironment { get; set; }

    public Startup(IConfiguration configuration, IHostingEnvironment env)
    {
        this.Configuration = configuration;
        this.HostingEnvironment = env;

        // TODO get rid of this dependency
        TypeFactory.Configuration = Configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
        {
        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        // Create the Bot Framework Adapter with error handling enabled.
        services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

        // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
        services.AddSingleton<IStorage, MemoryStorage>();

        // Create the User state. (Used in this bot's Dialog implementation.)
        services.AddSingleton<UserState>();

        // Create the Conversation state. (Used by the Dialog system itself.)
        services.AddSingleton<ConversationState>();

        var resourceExplorer = ResourceExplorer.LoadProject(this.HostingEnvironment.ContentRootPath);
        services.AddSingleton(resourceExplorer);

        // The Dialog that will be run by the bot.
        services.AddSingleton<RootDialog>();

        // Add this so settings memory scope is populated correctly.
        services.AddSingleton<IConfiguration>(this.Configuration);

        // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
        services.AddTransient<IBot, DialogBot<RootDialog>>();
    }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }

