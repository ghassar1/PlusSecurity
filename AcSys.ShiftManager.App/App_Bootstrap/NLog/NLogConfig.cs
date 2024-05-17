//using NLog;
//using NLog.Config;
//using NLog.Layouts;
//using NLog.Targets;

//namespace AcSys.ShiftManager.App
//{
//    public class NLogConfig
//    {
//        public static void Configure()
//        {
//            //CreateLogTable();

//            LoggingConfiguration config = new LoggingConfiguration();
//            //LoggingConfiguration config = LogManager.Configuration;
//            //var factory = new LogFactory(LogManager.Configuration);
//            //LoggingConfiguration config = factory.Configuration;


//            CreateDatabaseTarget(config);
//            CreateColoredConsoleTarget(config);
//            CreateFileTarget(config);

//            LogManager.Configuration = config;

//            // Example usage
//            Logger logger = LogManager.GetLogger("Example");
//            logger.Trace("trace log message");
//            logger.Debug("debug log message");
//            logger.Info("info log message");
//            logger.Warn("warn log message");
//            logger.Error("error log message");
//            logger.Fatal("fatal log message");
//        }

//        private static void CreateDatabaseTarget(LoggingConfiguration config)
//        {
//            //var entityFrameworkConnection = ConfigurationManager.ConnectionStrings["NLogDb"].ConnectionString;
//            //var builder = new EntityConnectionStringBuilder(entityFrameworkConnection);
//            //var connectionString = builder.ProviderConnectionString;
//            string connectionString = "NLogDb";

//            var target = new DatabaseTarget()
//            {
//                ConnectionString = connectionString,
//                CommandText = @"insert into dbo.Log (Application, Logged, Level, Message,Username,ServerName, Port, Url, Https,ServerAddress, RemoteAddress,Logger, CallSite, Exception) 
//                            values (@Application, @Logged, @Level, @Message,@Username,@ServerName, @Port, @Url, @Https,@ServerAddress, @RemoteAddress,@Logger, @Callsite, @Exception);",
//                Parameters = {
//                    new DatabaseParameterInfo("@dateTime", new SimpleLayout("${date}")),
//                    new DatabaseParameterInfo("@message", new SimpleLayout("${message}")),

//                    new DatabaseParameterInfo("@dateTime", new SimpleLayout("${date}")),
//                    new DatabaseParameterInfo("@application", new SimpleLayout("${appsetting:name=AppName:default=Unknown\\: set AppName in appSettings}")),
//                    new DatabaseParameterInfo("@logged", new SimpleLayout("${date}")),
//                    new DatabaseParameterInfo("@level", new SimpleLayout("${level}")),
//                    new DatabaseParameterInfo("@message", new SimpleLayout("${message}")),

//                    new DatabaseParameterInfo("@username", new SimpleLayout("${identity}")),

//                    new DatabaseParameterInfo("@serverName", new SimpleLayout("${aspnet-request:serverVariable=SERVER_NAME}")),
//                    new DatabaseParameterInfo("@port", new SimpleLayout("${aspnet-request:serverVariable=SERVER_PORT}")),
//                    new DatabaseParameterInfo("@url", new SimpleLayout("${aspnet-request:serverVariable=HTTP_URL}")),
//                    new DatabaseParameterInfo("@https", new SimpleLayout("${when:inner=1:when='${aspnet-request:serverVariable=HTTPS}' == 'on'}${when:inner=0:when='${aspnet-request:serverVariable=HTTPS}' != 'on'}")),

//                    new DatabaseParameterInfo("@serverAddress", new SimpleLayout("${aspnet-request:serverVariable=LOCAL_ADDR}")),
//                    new DatabaseParameterInfo("@remoteAddress", new SimpleLayout("${aspnet-request:serverVariable=REMOTE_ADDR}:${aspnet-request:serverVariable=REMOTE_PORT}")),

//                    new DatabaseParameterInfo("@logger", new SimpleLayout("${logger}")),
//                    new DatabaseParameterInfo("@callSite", new SimpleLayout("${callsite}")),
//                    new DatabaseParameterInfo("@exception", new SimpleLayout("${exception:tostring}")),
//                }
//            };

//            config.AddTarget("database", target);
//            config.LoggingRules.Add(new LoggingRule("*", target));
//        }

//        private static void CreateColoredConsoleTarget(LoggingConfiguration config)
//        {
//            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
//            config.AddTarget("console", consoleTarget);
//            consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";

//            LoggingRule rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
//            config.LoggingRules.Add(rule1);
//        }

//        private static void CreateFileTarget(LoggingConfiguration config)
//        {
//            FileTarget fileTarget = new FileTarget();
//            fileTarget.FileName = "${basedir}/file.txt";
//            fileTarget.Layout = "${message}";
//            config.AddTarget("file", fileTarget);

//            LoggingRule rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
//            config.LoggingRules.Add(rule2);
//        }

//        public static void CreateLogTable()
//        {
//            //string logTableCreateStatement = @"
//            //    SET ANSI_NULLS ON
//            //    SET QUOTED_IDENTIFIER ON
//            //    CREATE TABLE [dbo].[Log] ([Id] [int] IDENTITY(1,1) NOT NULL,[Application] [nvarchar](50) NOT NULL,[Logged] [datetime] NOT NULL,
//            //        [Level] [nvarchar](50) NOT NULL,[Message] [nvarchar](max) NOT NULL,[UserName] [nvarchar](250) NULL,[ServerName] [nvarchar](max) NULL,
//            //        [Port] [nvarchar](max) NULL,[Url] [nvarchar](max) NULL,[Https] [bit] NULL,[ServerAddress] [nvarchar](100) NULL,
//            //        [RemoteAddress] [nvarchar](100) NULL,[Logger] [nvarchar](250) NULL,[Callsite] [nvarchar](max) NULL,[Exception] [nvarchar](max) NULL,
//            //        CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED ([Id] ASC)
//            //        WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
//            //    ) ON [PRIMARY]";
//        }
//    }
//}
