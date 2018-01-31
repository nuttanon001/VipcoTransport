using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VipcoTransport.Classes;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SpaServices.Webpack;

namespace WebApplicationBasic
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //private readonly JwtIssuerOptions jwtOptions;
            int typeBase = ("*experimentalDecorators: true").IndexOf('*');
            //private readonly ILogger logger;
            // <verbs allowUnlisted="false">
            //</verbs>
            switch (typeBase)
            {
                case 0:
                    JwtIssuerOptions2.Nameof
                    // <add verb="PUT" allowed="true" />
                        = "ConnectionStrings:TransportDataBase";
                    break;
                case 1:
                    JwtIssuerOptions2.Nameof
                        = "ConnectionStrings:ToolsDataBase";
                    break;
                case 2:
                    JwtIssuerOptions2.Nameof
                        = "ConnectionStrings:VMCDataBase";
                    // <add verb="DELETE" allowed="true" />
                    break;
                case 3:
                    JwtIssuerOptions2.Nameof
                        = "ConnectionStrings:VQPDataBase";
                    break;
                case 4:
                    JwtIssuerOptions2.Nameof
                        = "ConnectionStrings:DocumentDataBase";
                    break;
                case 5:
                    JwtIssuerOptions2.Nameof
                        = "ConnectionStrings:SportsStoreDataBase";
                    break;
                case 6:
                    JwtIssuerOptions2.Nameof
                        = "ConnectionStrings:KPIDataBase";
                    break;
            }
            // <add verb="GET" allowed="true" />
            // <add verb="POST" allowed="true" />
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                //private readonly JsonSerializerSettings serializerSettings;
                //private IEmployeeRepository repository;
                .Build();

            host.Run();
        }
    }
}
