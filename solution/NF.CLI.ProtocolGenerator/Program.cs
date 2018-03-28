using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CommandLine;
using DotLiquid;

namespace NF.CLI.ProtocolGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ParserResult<Options> result = Parser.Default.ParseArguments<Options>(args);
            result.WithParsed(Run).WithNotParsed(Fail);
        }

        private static void Fail(IEnumerable<Error> errs)
        {
        }

        private static void Run(Options opt)
        {
            string messageDll = opt.MessageDll;
            string templateInterface = opt.InterfaceTemplate;
            string outputInterface = opt.InterfaceOutput;
            string templateTransfer = opt.TransferTemplate;
            string outputTransfer = opt.TransferOutput;

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

            List<MessageInfo> messageInfos = types.Select(Util.ExtractMessageInfo).ToList();
            MessageDic mdic = new MessageDic(messageInfos);

            // render interface
            Template interfaceTemplate = Template.Parse(File.ReadAllText(templateInterface));
            Directory.CreateDirectory(outputInterface);
            foreach (KeyValuePair<string, MessageQR> m in mdic._dic)
            {
                string name = m.Key;
                string mmm = interfaceTemplate.Render(Hash.FromAnonymousObject(new
                {
                    name,
                    m.Value.q,
                    m.Value.r
                }));
                string outputFpath = $"{outputInterface}/{name}.autogen.cs";
                File.WriteAllText(outputFpath, mmm);
            }

            // render transfer
            Template transferTemplate = Template.Parse(File.ReadAllText(templateTransfer));
            Directory.CreateDirectory(outputTransfer);
            foreach (KeyValuePair<string, MessageQR> m in mdic._dic)
            {
                string name = m.Key;
                string mmm = transferTemplate.Render(Hash.FromAnonymousObject(new
                {
                    name,
                    m.Value.q,
                    m.Value.r
                }));
                string outputFpath = $"{outputTransfer}/{name}.autogen.cs";
                File.WriteAllText(outputFpath, mmm);
            }
        }

        private class Options
        {
            [Option("dll", Required = true, HelpText = "intput dll")]
            public string MessageDll { get; set; }

            [Option("interface_template", Required = true, HelpText = "input interface template")]
            public string InterfaceTemplate { get; set; }

            [Option("interface_output", Required = true, HelpText = "output auto.interface dir")]
            public string InterfaceOutput { get; set; }

            [Option("transfer_template", Required = true, HelpText = "input transfer template")]
            public string TransferTemplate { get; set; }

            [Option("transfer_output", Required = true, HelpText = "output auto.transfer dir")]
            public string TransferOutput { get; set; }
        }
    }

    public enum MessageType
    {
        None,
        Q,
        R
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

    public class MessageQR : Drop
    {
        public MessageInfo q { get; set; }
        public MessageInfo r { get; set; }
    }

    public class MessageDic
    {
        public Dictionary<string, MessageQR> _dic = new Dictionary<string, MessageQR>();

        public MessageDic(IEnumerable<MessageInfo> infos)
        {
            foreach (MessageInfo mi in infos)
            {
                this.Add(mi);
            }
        }

        private void Add(MessageInfo mi)
        {
            MessageQR qr = null;
            string key = mi.MessageName;
            if (!this._dic.TryGetValue(key, out qr))
            {
                qr = new MessageQR();
                this._dic.Add(key, qr);
            }

            switch (mi.MessageType)
            {
                case MessageType.Q:
                    qr.q = mi;
                    break;
                case MessageType.R:
                    qr.r = mi;
                    break;
                default:
                    break;
            }
        }
    }

    public static class Util
    {
        public static MessageInfo ExtractMessageInfo(Type type)
        {
            Regex r1 = new Regex("(Q|R)(.*)");
            Match match = r1.Match(type.Name);
            if (!match.Success)
            {
                return null;
            }

            MessageInfo ret = new MessageInfo(type);
            string mType = match.Groups[1].Value;
            string mName = match.Groups[2].Value;
            ret.MessageType = ParseEnum<MessageType>(mType);
            ret.MessageName = mName;
            return ret;
        }

        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }
    }
}