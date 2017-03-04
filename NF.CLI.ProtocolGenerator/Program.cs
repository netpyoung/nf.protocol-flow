namespace NF.CLI.ProtocolGenerator
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;
    using DotLiquid;
    using CommandLine;

    class Program
    {
        class Options
        {
            [Option("dll", Required = true, HelpText = "intput dll")]
            public string MessageDLL { get; set; }

            [Option("interface_template", Required = true, HelpText = "input interface template")]
            public string InterfaceTemplate { get; set; }

            [Option("interface_output", Required = true, HelpText = "output auto.interface dir")]
            public string InterfaceOutput { get; set; }

            [Option("transfer_template", Required = true, HelpText = "input transfer template")]
            public string TransferTemplate { get; set; }

            [Option("transfer_output", Required = true, HelpText = "output auto.transfer dir")]
            public string TransferOutput { get; set; }
        }

        static void Main(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<Options>(args);
            result.WithParsed(Run);
        }

        private static void Run(Options opt)
        {
            string message_dll = opt.MessageDLL;
            string template_interface = opt.InterfaceTemplate;
            string output_interface = opt.InterfaceOutput;
            string template_transfer = opt.TransferTemplate;
            string output_transfer = opt.TransferOutput;

            // get mdic
            List<Type> types = new List<Type>();
            foreach (var type in Assembly.LoadFrom(message_dll).GetTypes())
            {
                if (!type.IsClass)
                    continue;

                var imessage = type.GetInterface("IMessage");
                if (imessage == null)
                    continue;

                if (imessage.FullName != "Google.Protobuf.IMessage")
                    continue;
                types.Add(type);
            }

            var message_infos = types.Select(x => Util.ExtractMessageInfo(x)).ToList();
            var mdic = new MessageDic(message_infos);

            // render interface
            Template interface_template = Template.Parse(File.ReadAllText(template_interface));
            System.IO.Directory.CreateDirectory(output_interface);
            foreach (var m in mdic._dic)
            {
                string name = m.Key;
                string mmm = interface_template.Render(Hash.FromAnonymousObject(new {
                            name = name, q = m.Value.q, r = m.Value.r,
                        }));
                string output_fpath = string.Format("{0}/{1}.cs", output_interface, name);
                File.WriteAllText(output_fpath, mmm);
            }

            // render transfer
            Template transfer_template = Template.Parse(File.ReadAllText(template_transfer));
            System.IO.Directory.CreateDirectory(output_transfer);
            foreach (var m in mdic._dic)
            {
                string name = m.Key;
                string mmm = transfer_template.Render(Hash.FromAnonymousObject(new {
                            name = name, q = m.Value.q, r = m.Value.r,
                        }));
                string output_fpath = string.Format("{0}/{1}.cs", output_transfer, name);
                File.WriteAllText(output_fpath, mmm);
            }
        }
    }

    public enum MessageType
    {
        None,
        Q,
        R,
    }

    public class Argument : Drop
    {
        public Type Type { get; set; }
        public string Name { get; set; }

        public override string ToString() {
            return string.Format("{0} {1}", Type, Name);
        }

        public string DuplicateAssignName() {
            return string.Format("{0} = {0}", Name);
        }
    }

    public class MessageInfo : Drop
    {
        public MessageType MessageType { get; set; }
        public string MessageName { get; set; }
        public string Name { get { return Type.Name; } }
        public Type Type { get; private set; }
        public List<Argument> Arguments { get; private set; }
        public MessageInfo(Type type) {
            this.Type = type;
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | System.Reflection.BindingFlags.SetProperty);
            this.Arguments = new List<Argument>();
            this.Arguments.AddRange(properties.ToList().Select(p => new Argument { Type = p.PropertyType, Name = p.Name }));
        }
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
            foreach (var mi in infos) {
                Add(mi);
            }
        }

        void Add(MessageInfo mi)
        {
            MessageQR qr = null;
            string key = mi.MessageName;
            if (!_dic.TryGetValue(key, out qr)) {
                qr = new MessageQR();
                _dic.Add(key, qr);
            }

            switch (mi.MessageType) {
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
                return null;
            MessageInfo ret = new MessageInfo(type);
            string m_type = match.Groups[1].Value;
            string m_name = match.Groups[2].Value;
            ret.MessageType = ParseEnum<MessageType>(m_type);
            ret.MessageName = m_name;
            return ret;
        }

        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }
    }
}
