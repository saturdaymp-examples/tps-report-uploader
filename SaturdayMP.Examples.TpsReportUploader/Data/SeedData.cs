using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SaturdayMP.Examples.TpsReportUploader.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext dbContext)
    {
        if (dbContext.Users.Any())
        {
            return;
        }

        CreateUser("chris122", $"password", false, dbContext);
        CreateUser("tom", $"qwerty", false, dbContext);
        CreateUser("bill84", $"dxR`Bg4E9k(U/n", true, dbContext);
        CreateUser("peter46", $"y?:F&6E4M*b2", true, dbContext);
        CreateUser("michael32", $"Txet9[W#cR7{{", true, dbContext);
        CreateUser("samir", $"A,N2`qLcxZ;54n", true, dbContext);

        await dbContext.SaveChangesAsync();
        
        for (var i = -10; i < 0; i++)
        {
            CreateReportProcessRun(DateTime.Today.AddDays(i), dbContext);    
        }

        await dbContext.SaveChangesAsync();

        CreateUnprocessedFiles();
        CreateProcessedFiles(dbContext);
    }

    private static void CreateUnprocessedFiles()
    {
        if (Directory.Exists("wwwroot/uploads"))
        {
            Directory.Delete("wwwroot/uploads", true);    
        }
        Directory.CreateDirectory("wwwroot/uploads");
        
        File.WriteAllText(Path.Combine("wwwroot/uploads", UnprocessedReportName("samir")), "TPS Report for samir this not yet processed.  FIRST_FLAG!!!");
    }

    private static void CreateProcessedFiles(ApplicationDbContext dbContext)
    {
        if (Directory.Exists("processed_reports"))
        {
            Directory.Delete("processed_reports", true);    
        }
        Directory.CreateDirectory("processed_reports");

        var processItems = dbContext.ReportProcessRunItems
            .Include(reportProcessRunItem => reportProcessRunItem.ApplicationUser)
            .Include(reportProcessRunItem => reportProcessRunItem.ReportProcessRun).ToList();
        
        foreach (var item in processItems)
        {
            var fileName = Path.Combine("processed_reports", ProcessedReportName(item.ApplicationUser.UserName, item.ReportProcessRun.RunDate));

            var textToWrite = $"TPS Report for {item.ApplicationUser.UserName} that was processed on {item.ReportProcessRun.RunDate:yyyy-MM-dd}";
            if (item.ApplicationUser.UserName == "samir" && DateTime.Today.AddDays(-4).Date == item.ReportProcessRun.RunDate.Date)
            {
                textToWrite += "  SAMIR_SECOND_FLAG!!!";
            }
            
            File.WriteAllText(fileName, textToWrite);
        }
    }

    private static void CreateReportProcessRun(DateTime date, ApplicationDbContext dbContext)
    {
        var processRun = new ReportProcessRun()
        {
            RunDate = date
        };

        var users = dbContext.Users.ToList();
        foreach (var user in users)
        {
            var processItem = new ReportProcessRunItem()
            {
                ApplicationUser = user,
                ReportProcessRun = processRun,
                ReportName = ProcessedReportName(user.UserName, date)
            };

            dbContext.Add(processItem);
        }

        dbContext.Add(processRun);
    }

    private static void CreateUser(string userName, string password, bool lockoutEnabled, ApplicationDbContext dbContext)
    {
        var user = new ApplicationUser()
        {
            UserName = userName,
            NormalizedUserName = userName.ToUpper(),
            Email = $"{userName}@initech.com",
            NormalizedEmail = $"{userName}@initech.com".ToUpper(),
            EmailConfirmed = true,
            LockoutEnabled = lockoutEnabled
        };

        dbContext.Users.Add(user);

        var hasher = new PasswordHasher<ApplicationUser>();
        user.SecurityStamp = Guid.NewGuid().ToString();
        user.PasswordHash = hasher.HashPassword(user, password);
    }

    private static string UnprocessedReportName(string? userName)
    {
        return $"{userName}-tpsreport.txt";
    }
    
    private static string ProcessedReportName(string? userName, DateTime date)
    {
        return $"{userName}-tpsreport-{date:yyyy-MM-dd}.txt";
    }
}