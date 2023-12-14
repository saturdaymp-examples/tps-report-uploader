# TPS Report Uploader

A demonstration of how to exploit some vulnerabilities and how to fix them in a .NET 8 [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) application that I originally presented at Edmonton .NET User Group ([EDMUG](edmug.net)) in [December 2023](https://www.meetup.com/edmonton-net-user-group/events/297820544/).

The application is designed as a mini capture the flag ([CTF](https://en.wikipedia.org/wiki/Capture_the_flag_(cybersecurity))).  Your goal is to get all the TPS reports for Simar.  There is a detailed [walk through](WALKTHROUGH.md) that is similar to my in-person presentation.

## Setup ##

To run the application you need to have .NET 8 SDK installed.  To complete the Walk Through you need the following tools:

- Burp Suite (with Blazor Traffic Processor [extension](https://portswigger.net/bappstore/8a87b0d9654944ccbdf6ae8bdd18e1d4))
- GoBuster or Docker (more below)

All the tools above have free versions that are sufficient for this demo.

### Application ###

Since this is a demo about fixing vulnerabilities it is recommended to run the TPS Report Uploader application via a IDE (e.g. [Rider](https://www.jetbrains.com/rider/), [Visual Studio](https://visualstudio.microsoft.com/), or [Visual Studio Code](https://code.visualstudio.com/)).  That way you can find the venerability then fix it on the fly.

The steps to get the app running in development are:

1) Install .NET 8 and Entity Framework 8.
2) Clone the repo.
3) Open the SaturdayMP.Examples.TpsReportUploader.sln in your IDE of choice.
4) If running on Windows update the SqlLite database path in [appsettings.json](SaturdayMP.Examples.TpsReportUploader/appsettings.json) from `DataSource=Data/app.db;Cache=Shared` to `DataSource=Data\\app.db;Cache=Shared`. 
5) Create the DB: `dotnet ef database update`.
4) Run the application.

The first time you run the application it will create the SqlLite database at `Data/app.data`.  It will also create some folders at `wwwroot/uploads` and `processed_reports` fill them example TPS reports.  You can see the seed logic in [Data/SeedData.cs](SaturdayMP.Examples.TpsReportUploader/Data/SeedData.cs).

To reset the DB and files:

1) Delete the SqlLite DB.
2) Run `dotnet ef database update` to recreate the database.
3) RUn the app.  The initialization will re-seed the database, delete and recreate the `wwwroot/uploads` and `processed_reports` folders.

### Burp Suite ###

You can download Burp Suite Community edition [here](https://portswigger.net/burp/communitydownload).  You don't need to enter a email address, just click the "Go straight to downloads" link.

After you have Burp Suite installed you need to install the Blazor extension.  The easiest way from Burp Suite is:

1) Click Extensions->BApp Store.
2) Search for [Blazor Traffic Processor](https://portswigger.net/bappstore/8a87b0d9654944ccbdf6ae8bdd18e1d4) and install it.

### GoBuster ###

If you have Docker installed then GoBuster can be run using Docker Compose ([docker-compose.yml](docker-compose.yml)):

```
docker compose run --rm gobuster
```

The first time you run the above command it will pull the GoBuster Docker image.  Future runs will used the cached image.  Note: the `--rm` will remove the GoBuster container after each run.  If you want it to exist after each run remove the `--rm`.

You can also install [GoBuster](https://github.com/OJ/gobuster) using the various ways outlined in the [Easy Install]((https://github.com/OJ/gobuster?tab=readme-ov-file#easy-installation)) section on their README.

## Feedback ##

If you spot an issue, an improvement, or constructive criticism please open an [issue](https://github.com/saturdaymp-examples/tps-report-uploader/issues) or [pull request](https://github.com/saturdaymp-examples/tps-report-uploader/pulls).

## Acknowlegements ##

Thank to you [EDMUG](https://edmug.net/) for the opportunity to present.  Also thank you to those involved in creating https://en.wikipedia.org/wiki/Office_Space and [TPS reports](https://en.wikipedia.org/wiki/TPS_report).
