using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CommandLine;
using DotLiquid;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NF.CLI.ProtocolGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            ParserResult<Options> result = Parser.Default.ParseArguments<Options>(args);
            result.WithParsed(Run).WithNotParsed(Fail);
        }

        private void Fail(IEnumerable<Error> errs)
        {
            foreach (Error err in errs)
            {
                Console.Error.WriteLine(err);
            }
        }

        private string ReadFromResource(string resourceName)
        {
            var assembly = typeof(Program).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private void Run(Options opt)
        {
            string messageDll = opt.MessageDll;
            string outputInterface = opt.InterfaceOutput;
            string outputProtocol = opt.ProtocolOutput;
            string outDir = opt.Output;

            if (string.IsNullOrEmpty(outputInterface)
                && string.IsNullOrEmpty(outputProtocol)
                && string.IsNullOrEmpty(outDir))
            {
                Console.Error.WriteLine("output directory required.");
                return;
            }

            Config config;
            if (string.IsNullOrEmpty(opt.Config))
            {
                config = new Config();
            }
            else
            {
                Deserializer deserializer = 
                    new DeserializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention())
                    .Build();
                config = deserializer.Deserialize<Config>(File.ReadAllText(opt.Config));
            }

            if (!string.IsNullOrWhiteSpace(opt.MessageDll))
            {
                config.MessageDll = messageDll;
            }
            if (!string.IsNullOrWhiteSpace(opt.InterfaceOutput))
            {
                config.OutputInterface = opt.InterfaceOutput;
            }
            if (!string.IsNullOrWhiteSpace(opt.ProtocolOutput))
            {
                config.OutputProtocol = opt.ProtocolOutput;
            }
            if (!string.IsNullOrWhiteSpace(opt.Output))
            {
                config.OutputDir = opt.Output;
            }
            // get mdic
            List<Type> types = new List<Type>();
            foreach (Type type in Assembly.LoadFrom(messageDll).GetTypes())
            {
                if (!type.IsClass)
                {
                    continue;
                }

                Type imessage = type.GetInterface("IMessage");
                if (imessage == null)
                {
                    continue;
                }

                if (imessage.FullName != "Google.Protobuf.IMessage")
                {
                    continue;
                }

                types.Add(type);
            }

            if (string.IsNullOrEmpty(config.NamespaceMessage))
            {
                config.NamespaceMessage = types.First().Namespace;
            }

            List<MessageInfo> messageInfos =
                types
                .Select(x => ExtractRequestMessageInfo(x, config.RegRequest))
                .Where(x => x != null)
                .Concat(
                    types
                    .Select(x => ExtractResponseMessageInfo(x, config.RegResponse))
                    .Where(x => x != null)
                )
                .ToList();


            MessageDic mdic = new MessageDic(messageInfos);

            // render interface
            RenderInterface(mdic, config);
            RenderProtocol(mdic, config);
            
        }

        private void RenderInterface(MessageDic mdic, Config config)
        {
            string interfaceLiquid = string.Empty;

            if (string.IsNullOrEmpty(config.LiquidInterface))
            {
                interfaceLiquid = ReadFromResource("interface.liquid");
            }
            else
            {
                interfaceLiquid = File.ReadAllText(config.LiquidInterface);
            }

            Template interfaceTemplate = Template.Parse(interfaceLiquid);
            Template libraryTemplate = Template.Parse(ReadFromResource("InterfaceLibrary.cs"));

            foreach (string dir in new string[] { config.OutputInterface, config.OutputDir})
            {
                if (string.IsNullOrWhiteSpace(dir))
                {
                    continue;
                }

                Directory.CreateDirectory(dir);
                foreach (KeyValuePair<string, MessagePack> m in mdic._dic)
                {
                    string name = m.Key;
                    string mmm = interfaceTemplate.Render(Hash.FromAnonymousObject(new
                    {
                        name,
                        message_namespace = config.NamespaceMessage,
                        interface_namespace = config.NamespaceInterface,
                        q = m.Value.Request,
                        r = m.Value.Response
                    }));
                    string outputFpath = $"{dir}/{name}.interface.autogen.cs";
                    File.WriteAllText(outputFpath, mmm);
                }


                string lll = libraryTemplate.Render(Hash.FromAnonymousObject(new
                {
                    interface_namespace = config.NamespaceInterface,
                }));
                File.WriteAllText($"{dir}/InterfaceLibrary.cs", lll);
            }
        }

        private void RenderProtocol(MessageDic mdic, Config config)
        {
            string protocolLiquid = string.Empty;

            if (string.IsNullOrEmpty(config.LiquidProtocol))
            {
                protocolLiquid = ReadFromResource("protocol.liquid");
            }
            else
            {
                protocolLiquid = File.ReadAllText(config.LiquidInterface);
            }

            // render protocol
            Template transferTemplate = Template.Parse(protocolLiquid);
            Template libraryTemplate = Template.Parse(ReadFromResource("ProtocolLibrary.cs"));

            foreach (string dir in new string[] { config.OutputProtocol, config.OutputDir })
            {
                if (string.IsNullOrWhiteSpace(dir))
                {
                    continue;
                }

                Directory.CreateDirectory(dir);
                foreach (KeyValuePair<string, MessagePack> m in mdic._dic)
                {
                    string name = m.Key;
                    string mmm = transferTemplate.Render(Hash.FromAnonymousObject(new
                    {
                        name,
                        message_namespace = config.NamespaceMessage,
                        interface_namespace = config.NamespaceInterface,
                        protocol_namespace = config.NamespaceProtocol,
                        q = m.Value.Request,
                        r = m.Value.Response
                    }));
                    string outputFpath = $"{dir}/{name}.protocol.autogen.cs";
                    File.WriteAllText(outputFpath, mmm);
                }


                string lll = libraryTemplate.Render(Hash.FromAnonymousObject(new
                {
                    interface_namespace = config.NamespaceInterface,
                    protocol_namespace = config.NamespaceProtocol,
                }));
                File.WriteAllText($"{dir}/ProtocolLibrary.cs", lll);
            }
        }
    
        private MessageInfo ExtractRequestMessageInfo(Type type, string regString)
        {
            Regex reg = new Regex(regString);
            Match match = reg.Match(type.Name);
            if (!match.Success)
            {
                return null;
            }

            MessageInfo ret = new MessageInfo(type);
            ret.MessageType = MessageType.Request;
            ret.MessageName = match.Groups["name"].Value;
            return ret;
        }
        private static MessageInfo ExtractResponseMessageInfo(Type type, string regString)
        {
            Regex reg = new Regex(regString);
            Match match = reg.Match(type.Name);
            if (!match.Success)
            {
                return null;
            }

            MessageInfo ret = new MessageInfo(type);
            ret.MessageType = MessageType.Response;
            ret.MessageName = match.Groups["name"].Value;
            return ret;
        }

        public class Options
        {
            [Option("dll", Required = false, HelpText = "intput dll")]
            public string MessageDll { get; set; }

            [Option('f', longName: "config", Required = false, HelpText = "intput dll")]
            public string Config { get; set; }

            [Option("out", Required = false, HelpText = "output auto.* dir")]
            public string Output { get; set; }

            [Option("interface_output", Required = false, HelpText = "output auto.interface dir")]
            public string InterfaceOutput { get; set; }

            [Option("transfer_output", Required = false, HelpText = "output auto.transfer dir")]
            public string ProtocolOutput { get; set; }
        }
    }

    public enum MessageType
    {
        None,
        Request,
        Response
    }

    public class Config
    {
        public string MessageDll { get; set; }
        public string OutputInterface { get; set; }
        public string OutputProtocol { get; set; }
        public string OutputDir { get; set; }

        public string NamespaceInterface { get; set; } = "AutoGenerated.Interface";
        public string NamespaceProtocol { get; set; } = "AutoGenerated.Protocol";
        public string NamespaceMessage { get; set; } = string.Empty;

        public string LiquidInterface { get; set; }// = "interface.liquid";
        public string LiquidProtocol { get; set; }// = "protocol.liquid";

        public string RegRequest { get; set; } = "(?<protocol>Q(?<name>.*))";
        public string RegResponse { get; set; } = "(?<protocol>R(?<name>.*))";
    }

    public class Argument : Drop
    {
        public Type Type { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{this.Type} {this.Name}";
        }

        public string DuplicateAssignName()
        {
            return string.Format("{0} = {0}", this.Name);
        }
    }

    public class MessageInfo : Drop
    {
        public MessageInfo(Type type)
        {
            this.Type = type;
            PropertyInfo[] properties =
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
            this.Arguments = new List<Argument>();
            this.Arguments.AddRange(
                properties.ToList().Select(p => new Argument {Type = p.PropertyType, Name = p.Name}));
        }

        public MessageType MessageType { get; set; }
        public string MessageName { get; set; }
        public string Name => this.Type.Name;
        public Type Type { get; }
        public List<Argument> Arguments { get; }
    }

    public class MessagePack : Drop
    {
        public MessageInfo Request { get; set; }
        public MessageInfo Response { get; set; }
    }

    public class MessageDic
    {
        public Dictionary<string, MessagePack> _dic = new Dictionary<string, MessagePack>();

        public MessageDic(IEnumerable<MessageInfo> infos)
        {
            foreach (MessageInfo mi in infos)
            {
                this.Add(mi);
            }
        }

        private void Add(MessageInfo mi)
        {
            MessagePack mp = null;
            string key = mi.MessageName;
            if (!this._dic.TryGetValue(key, out mp))
            {
                mp = new MessagePack();
                this._dic.Add(key, mp);
            }

            switch (mi.MessageType)
            {
                case MessageType.Request:
                    mp.Request = mi;
                    break;
                case MessageType.Response:
                    mp.Response = mi;
                    break;
                default:
                    break;
            }
        }
    }
}