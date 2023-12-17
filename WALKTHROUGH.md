# Walkthrough # 

The goal is to find all the [TPS reports](https://en.wikipedia.org/wiki/TPS_report) for [Samir](https://en.wikipedia.org/wiki/Office_Space) (SAMIR_FIRST_FLAG!!!, SAMIR_SECOND_FLAG!!!). 

## Find ChangeLog ## 

Run GoBuster to find any interesting files or directories: 

```shell 
docker compose run --rm gobuster dir -u http://host.docker.internal:5262 -w /wordlists/common-web-directories.txt 
``` 

Should see something like: 

```shell
===============================================================
Gobuster v3.6 by OJ Reeves (@TheColonial) & Christian Mehlmauer (@firefart) 
=============================================================== 
[+] Url: http://host.docker.internal:5262
[+] Method: GET
[+] Threads: 10 
[+] Wordlist: /wordlists/common-web-directories.txt 
[+] Negative Status codes: 404
[+] User Agent: gobuster/3.6
[+] Timeout: 10s 
=============================================================== 
Starting gobuster in directory enumeration mode 
=============================================================== 
/ChangeLog (Status: 200) [Size: 2974] /changelog (Status: 200) [Size: 2974] Progress: 4727 / 4727 (100.00%) =============================================================== 
Finished
=============================================================== 
``` 

Seems like the Change Log is accessible without a login.  I wonder if it has some useful information?

### Fix ### 

The fix is to put the Change Log behind authentication.

```csharp 
// Components/Pages/ChangeLog.razor 

@* To hide the Change Log page. *@ 
@attribute [Authorize] 
``` 

## Brute Force Login ## 

Based on the Change Log we can guess that Tom does not have secure password.Open up Burp Suite and do the following: 

1) Navigate to the Proxy tab and click open browser. 
2) In the Burp Suite browser go to the login page (http://localhost:5262/Account/Login). 
3) Try to login using username `tom` and any password. 
4) Check the HTTP History in Burp Suite and find the `/Account/Login` POST. Right click and choose Send to Intruder. 
5) In the Intruder tab select the `password` part of `Input.Password=password` at the bottom and click the Add $ button to wrap it as a parameter. 
6) Go to the Payloads tab and load the `WordLists/10k-most-common-password.txt`. 
7) Click Start Attack 

Let the attack run for a couple seconds, we don't need to use the entire file for the example. Pause the attack from the Attack->Pause menu item. Look at the Status Code and Length columns and notice one is a 302 instead of 200. This is the password you want to use. 

### Fix ### 

Lockout is enabled by the code but when some of the early users where created with the lockout disabled in the user record. To fix this open up the DB and change the `LockoutEnabled` to true. 

Other fixes outside this demo include requiring passwords of length 12 or greater. Forcing the password to have special characters, numbers, etc reduces it's entropy which is bad. On the plus side it does help prevent the password from being a dictionary word(s). 
 
When upgrading password requirements remember to update existing users. Also make sure you change default passwords. Slightly different but LastPass didn't update their [password iterations](https://palant.info/2022/12/28/lastpass-breach-the-significance-of-these-password-iterations/) for past customers. 
 
In production some additional form of rate limiting should be used. Most web servers have rate limiting feature and you can combine it with something like Fail2Ban such as outlined this example [video](https://www.youtube.com/watch?v=gR4w9trH9pA) and GitHub [repo](https://github.com/saturdaymp-examples/rate-limiting-with-nginx-fail2ban). 
 
Finally most cloud providers have some sort of rate limiting and firewalls you can use. 

## Files in Publicly Accessible Folder ## 

After logging in go and upload a file. Pick any text file and notice that after uploading it gets assigned a name. That likely means we can't abuse the upload feature to upload any file we want to any folder. 

That said when the link of the upload file is checked it appears to be a standard format of `{userName}-tpsreport.txt`. An example link is: http://localhost:5262/uploads/tom-tpsreport.txt. Try changing the url to Samir's login name: http://localhost:5262/uploads/samir-tpsreport.txt. Whoot! We just found the first flag, assuming everything worked I didn't screw up this writeup. 

### Fix ### 

In this case the files are uploaded to `wwwroot/uploads`. The `wwwroot` directory should only contain public files that anyone can view such as CSS, Javascript, PDFs like privacy policy, etc. Even users who are not logged but know the path can download any files in the folder. 

The fix is not implemented in this demo but requires moving the files to a non-public folder. Then change the link to call a endpoint that checks the user has access to the file in question and if so lets them download it. 

Most web servers and/or frameworks have a folder where public static files stored. Make sure sensitive files are not put in this folder. Make sure the web server is configured correctly. 

## File Accessible to Anyone Logged In ##  

In this case the TPS file not accessible by anonymous users but can be accessed by anyone who is logged in. 
1) Open Burp Suite and install the Blazor Traffic Processor [extension](https://portswigger.net/bappstore/8a87b0d9654944ccbdf6ae8bdd18e1d4). 
2) Navigate to the Processed Reports page in the app. 
3) In Burp Suite turn on intercept. 
4) Click to download a report. 
5) In Burp Suite notice Blazor socket data at the bottom of the request. It will look something like: `·BeginInvokeDotNetFromJS�¡2Ù%SaturdayMP.Examples.TpsReportUploader¬DownloadFileÙ#["tom-tpsreport-2023-12-03.txt"]`.
6) Copy the data to the BTP tab and deserialize it. 
7) Update the file path from `["tom-tpsreport-2023-12-03.txt"]` to `["samir-tpsreport-2023-12-03.txt"]`. The dates might be different then my example but keep them the same. 
8) Deserialize the JSON back to Blazor. 
9) Copy the Blazor socket data back into the request and click forward. 

Notice a file was downloaded in the Burp Browser, it can be quick. It will have the wrong name of `tom` but if opened it will say "samir". Try different ones until you find the second flag.

What other files could you download using the above?

### Fix ###

Use Blazor's [event handling](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/event-handling?view=aspnetcore-8.0) to wire up the onclick event.  This way the file name is never sent to the browser, just information about the button being clicked.

```csharp
// Components/Pages/ProcessedReports.razor

@* Secure Download *@
<TemplateColumn Title="Download">
  <button class="btn btn-sm" @onclick="@(() => OnDownloadClicked(context.ReportName))" >Download</button>
</TemplateColumn>

@* Secure download *@
<script>
  window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
  }
</script>

// Secure download
private async Task OnDownloadClicked(string fileName)
{
    var stream = File.OpenRead(FileDownloadPath(FileDownloadPath(fileName)));
    using var streamRef = new DotNetStreamReference(stream: stream);

    await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
}
```

Additional details on how to download files using Blazor can be found [here](https://learn.microsoft.com/en-us/aspnet/core/blazor/file-downloads?view=aspnetcore-8.0).

Another way to fix the problem is too first check that the complete path where the file is to be downloaded form is in the `process_reports` folder.  Additionally a check to DB will need to be made to confirm that the file being downloaded is owned by the user.

## Feedback ##

If you spot an issue, an improvement, or constructive criticism with this walkthrough please open an [issue](https://github.com/saturdaymp-examples/tps-report-uploader/issues) or [pull request](https://github.com/saturdaymp-examples/tps-report-uploader/pulls).
