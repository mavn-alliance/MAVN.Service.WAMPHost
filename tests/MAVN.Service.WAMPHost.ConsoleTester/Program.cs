using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using WampSharp.V2;
using WampSharp.V2.Authentication;
using WampSharp.V2.Client;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Rpc;

namespace MAVN.Service.WAMPHost.ConsoleTester
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var appArguments = TryGetAppArguments(args);
            
            if (appArguments == null)
            {
                return;
            }
            
            var outputWriter = TryGetOutputWriter(appArguments);
            var channel = await ConnectAsync(appArguments);
            var realmProxy = channel.RealmProxy;
            
            Console.WriteLine($"Connected to the server {appArguments.Uri}");;
            
            var subscription = Subscribe(realmProxy, appArguments, outputWriter);
            
            Console.WriteLine("Initializing...");
            
            var proxy = realmProxy.Services.GetCalleeProxy<IRpcService>();
            
            proxy.Initialize(appArguments.Topic, appArguments.AuthId);
            
            Console.WriteLine("Initialized.");
            
            Console.WriteLine("Press any key to disconnect");
            
            Console.ReadKey(true);
            
            subscription.Dispose();
            channel.Close();

            if (outputWriter != null)
            {
                await outputWriter.FlushAsync();
                outputWriter.Close();
                outputWriter.Dispose();
            }
        }

        private static IDisposable Subscribe(IWampRealmProxy realmProxy, AppArguments appArguments, StreamWriter outputWriter)
        {
            var subscription = realmProxy.Services
                .GetSubject<dynamic>(appArguments.Topic)
                .Subscribe(message =>
                {
                    Console.WriteLine(message);

                    outputWriter?.WriteLine(message);
                });

            return subscription;
        }

        private static async Task<IWampChannel> ConnectAsync(AppArguments appArguments)
        {
            var factory = new DefaultWampChannelFactory();
            IWampClientAuthenticator clientAuthenticator = GetClientAuthenticator(appArguments.AuthMethod, appArguments.AuthId);
            
            var channel = clientAuthenticator == null
                ? factory.CreateJsonChannel(GetUri(appArguments), appArguments.Realm)
                : factory.CreateJsonChannel(GetUri(appArguments), appArguments.Realm, clientAuthenticator);

            channel.RealmProxy.Monitor.ConnectionEstablished +=
                (sender, args) =>
                {
                    Console.WriteLine("connected session with ID " + args.SessionId);

                    dynamic details = args.WelcomeDetails.OriginalValue.Deserialize<dynamic>();

                    Console.WriteLine("authenticated using method '{0}' and provider '{1}'", details.authmethod,
                        details.authprovider);

                    Console.WriteLine("authenticated with authid '{0}' and authrole '{1}'", details.authid,
                        details.authrole);
                };
            
            while (!channel.RealmProxy.Monitor.IsConnected)
            {
                try
                {
                    Console.WriteLine($"Trying to connect to the server {appArguments.Uri}...");

                    channel.Open().Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to connect: {e.Message}");
                    Console.WriteLine("Retrying in 5 sec...");

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }

            return channel;
        }

        private static IWampClientAuthenticator GetClientAuthenticator(string authMethod, string authId)
        {
            if (!string.IsNullOrEmpty(authMethod) && !string.IsNullOrEmpty(authId))
            {
                switch (authMethod)
                {
                    case "ticket":
                        return new TicketAuthenticator(authId);
                    default:
                        Console.WriteLine("Unknown auth method. Anonymous authentication will be used");
                        break;
                }
            }

            return null;
        }

        private static string GetUri(AppArguments appArguments)
        {
            var uri = appArguments.Uri.ToString();

            if (uri.EndsWith('/'))
            {
                return uri;
            }

            return $"{uri}/";
        }

        private static StreamWriter TryGetOutputWriter(AppArguments appArguments)
        {
            if (string.IsNullOrWhiteSpace(appArguments.OutputFilePath))
            {
                return null;
            }

            var fileStream = File.Open(
                appArguments.OutputFilePath,
                appArguments.AppendOutput ? FileMode.Append : FileMode.Create,
                FileAccess.Write,
                FileShare.Read);

            return new StreamWriter(fileStream, Encoding.UTF8, bufferSize: 16, leaveOpen: false);
        }

        private static AppArguments TryGetAppArguments(string[] args)
        {
            var parser = new FluentCommandLineParser<AppArguments>();

            parser.SetupHelp("?", "help")
                .Callback(text => Console.WriteLine(text));

            parser.Setup(x => x.Uri)
                .As('u')
                .Required()
                .WithDescription("-u <uri>. Wamp host URI. Required");

            parser.Setup(x => x.Realm)
                .As('r')
                .Required()
                .WithDescription("-r <realm>. Realm name. Required");

            parser.Setup(x => x.Topic)
                .As('t')
                .Required()
                .WithDescription("-t <topic>. Topic name. Required");

            parser.Setup(x => x.OutputFilePath)
                .As('o')
                .SetDefault(null)
                .WithDescription("-o <file>. Output file path. Optional, default is empty");

            parser.Setup(x => x.AuthMethod)
                .As('m')
                .SetDefault(null)
                .WithDescription("-m <auth method>. Auth method name. Optional, default is empty");
            
            parser.Setup(x => x.AuthId)
                .As('i')
                .SetDefault(null)
                .WithDescription("-i <authentication id>. Authentication id. Optional, default is empty");

            parser.Setup(x => x.AppendOutput)
                .As('a')
                .SetDefault(false)
                .WithDescription("-a. Append output file. Optional, default is false");
            
            var parsingResult = parser.Parse(args);

            if (!parsingResult.HasErrors)
            {
                return parser.Object;
            }

            Console.WriteLine("Falcon Wamp Reader (c) 2019");
            Console.WriteLine("Usage:");

            parser.HelpOption.ShowHelp(parser.Options);

            return null;
        }
    }
    
    public interface IRpcService
    {
        [WampProcedure("init")]
        void Initialize(string topic, string token);
    }
    
    internal class AppArguments
    {
        public Uri Uri { get; set; }
        public string Realm { get; set; }
        public string Topic { get; set; }
        public string OutputFilePath { get; set; }

        public bool AppendOutput { get; set; }
        public string AuthMethod { get; set; }
        public string AuthId { get; set; }
    }
    
    public class TicketAuthenticator : IWampClientAuthenticator
    {
        private const string AuthMethod = "ticket";
        private static readonly string[] AuthMethods = { AuthMethod };

        public TicketAuthenticator(string authId)
        {
            AuthenticationId = authId;
        }

        public AuthenticationResponse Authenticate(string authmethod, ChallengeDetails extra)
        {
            if (authmethod != AuthMethod)
            {
                throw new WampAuthenticationException("don't know how to authenticate using '" + authmethod + "'");
            }
            return new AuthenticationResponse { Signature = AuthenticationId };
        }

        public string[] AuthenticationMethods => AuthMethods;

        public string AuthenticationId { get; }
    }
}