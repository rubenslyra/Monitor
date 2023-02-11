using System.Diagnostics;
using System.Net.Http.Headers;
using System.IO;
using System.Net;
using Monitor.Models;
using Monitor.Helpers;

namespace Monitor;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly Sites _sites;

    public Worker(ILogger<Worker> logger, IConfiguration _config)
    {
        _logger = logger;
        _sites = _config.GetSection("Sites").Get<Sites>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            HttpStatusCode status = await Requesters.GetStatusFromUrl(_sites.Url);
            if(status != HttpStatusCode.OK){

                string nameFile = string.Format("logFile_{0:dd-MM-yyyy}.txt", DateTime.Now);
                string path = Path.Combine(@"G:\Shared\dotnet_system_logs\logsReg\", nameFile);
                //string path = Path.Combine(Directory.GetCurrentDirectory(), "logs", nameFile);//

                StreamWriter logFile = new StreamWriter(path, true);
                var getUser = "WMIC /NODE:. COMPUTERSYSTEM GET USERNAME";  
                var user = Process.Start(new ProcessStartInfo("cmd", "/c " + getUser)  
                {  
                    RedirectStandardOutput = true,  
                    UseShellExecute = false,  
                    CreateNoWindow = true  
                }).StandardOutput.ReadToEnd().Trim();
              

                string message = string.Format($"O site {_sites.Url } está com problemas. Status: {status} - {DateTime.Now} | usuário: {user}");
                logFile.WriteLine(message);
                logFile.Close();                
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(3000, stoppingToken);
        }
    }
}
