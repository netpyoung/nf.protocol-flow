using CommandLine;

namespace NF.Tools.ProtocolFlow
{
    public partial class Program
    {
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
}