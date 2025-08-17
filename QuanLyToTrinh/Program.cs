using System.Text.Json.Serialization;
using AutoMapper;
using Database;
using Database.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repositories;
using Services;
using Services.MappingProfile;
using System.IO;
using Microsoft.Extensions.FileProviders;
using QuanLyToTrinh.SMSService;
using QuanLyToTrinh.AutoApprovingDocument;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddDbContext<QLTTrContext>(option => {
    option.UseSqlServer(builder.Configuration.GetConnectionString("QLToTrinhConnectionString"));
});

/*builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});*/

//builder.Services.AddCors();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(
                "https://totrinh.yenbai.net.vn", 
                "https://totrinh.yenbai.net.vn/", 
                "https://bcsd.yenbai.gov.vn", 
                "http://bcsd.yenbai.gov.vn",
                "https://btv.blueskytech.vn",
                "http://btv.blueskytech.vn",
                "http://localhost:4200",
                "https://btv.yenbai.net.vn",
                "https://btv.yenbai.net.vn/",
                "https://xinykien.yenbai.dcs.vn",
                "https://xinykien.yenbai.dcs.vn/",
                "http://xinykien.yenbai.dcs.vn",
                "http://xinykien.yenbai.dcs.vn/",
                "http://localhost:4200/")
            .AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new AutoMapping());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
    //builder.AddFile();
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserInRoleRepository, UserInRoleRepository>();
builder.Services.AddScoped<IDocumentFileRepository, DocumentFileRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentReviewRepository, DocumentReviewRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IFieldRepository, FieldRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IDocumentHistoryRepository, DocumentHistoryRepository>();
builder.Services.AddScoped<ISMSLogRepository, SMSLogRepository>();
builder.Services.AddScoped<IStatusesRepository, StatusesRepository>();
builder.Services.AddScoped<IDocumentTypesRepository, DocumentTypesRepository>();
builder.Services.AddScoped<IDocumentReviewRepository, DocumentReviewRepository>();
builder.Services.AddScoped<ICfgWorkFlowRepository, CfgWorkFlowRepository>();
builder.Services.AddScoped<ICfgWorkFlowGroupRepository, CfgWorkFlowGrouplRepository>();
builder.Services.AddScoped<ICfgWorkFlowUser, CfgWorkFlowUserRepository>();
builder.Services.AddScoped<ITblDepartmentRepository, TblDepartmentRepository>();
builder.Services.AddScoped<ITblGroupRepository, TblGroupRepository>();
builder.Services.AddScoped<ITblGroupDetailRepository, TblGroupDetailRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();


builder.Services.AddScoped<IToTrinhService, ToTrinhService>();
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IDocumentFileService, DocumentFileService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IDocumentReviewService, DocumentReviewService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IFieldService, FieldService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IRoleSerice,RoleService>();
builder.Services.AddScoped<IDocumentHistoryService, DocumentHistoryService>();
builder.Services.AddScoped<ISMSService, SMSService>();
builder.Services.AddScoped<ICategoryStatusesService, CategoryStatusesService>();
builder.Services.AddScoped<ITblDocumentTypesService, TblDocumentTypesService>();
builder.Services.AddScoped<ICfgWorkFlowService, CfgWorkFlowService>();
builder.Services.AddScoped<ISMSLogService, SMSLogService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUnitService, UnitService>();

builder.Services.AddHostedService<AutoSMSReminderService>();
builder.Services.AddHostedService<AutoApprovingService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files")),
    RequestPath = new PathString("/Files")
});

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
