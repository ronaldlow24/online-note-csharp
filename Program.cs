using Microsoft.EntityFrameworkCore;
using OnlineNote.Common;
using OnlineNote.Cron;
using OnlineNote.Entities;
using OnlineNote.Hubs;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.GetSection("ConnectionStrings").Bind(ApplicationSetting.ConnectionStrings);
builder.Configuration.GetSection("EmailConfiguration").Bind(ApplicationSetting.EmailConfiguration);

CustomMapper.ConstructMapper();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSignalR();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.MaxValue; 
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    // Just use the name of your job that you created in the Jobs folder.
    var jobKey = new JobKey("TriggerReminderCronJob");
    q.AddJob<TriggerReminderCronJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("TriggerReminderCronJob-trigger")
        .WithCronSchedule("0/30 * * * * ? *")
    );
});

// Quartz.Extensions.Hosting hosting
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapHub<NoteHub>("/noteHub");

app.Run();
