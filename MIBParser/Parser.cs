using System.Linq;
using System.Text.RegularExpressions;

namespace MIBParser
{
    public class Parser
    {
        private readonly IFileReader fileReader;

        private readonly IImportsLoader importsLoader;
        //Regex SplitSpace = new Regex(@"[a-z|A-Z|0-9]*(?= )");

        private readonly Regex complexTypeOfNode =
            new Regex(@"(?=(.*)SYNTAX(.*)\n)(?s).*?(?>})", RegexOptions.Multiline);

        private readonly Regex description =
            new Regex(@"(DESCRIPTION\s*)(?<description>[\s\S]*"")", RegexOptions.Multiline);

        private readonly Regex getNumbers = new Regex(@"[0-9]*\)");

        private readonly Regex getSequenceValuesRegex =
                new Regex(@"\s+(?<name>\S*)\n\s*(?<value>\S*),", RegexOptions.Multiline)
            ; //TODO this regex doesn't work in VS (but works in regex online WTF?)

        private readonly Regex nameOfNode = new Regex(@"((?<name>.*)OBJECT-TYPE)");
        private readonly Regex objectIdentifierRegex = new Regex(@"(?<name>.*)OBJECT IDENTIFIER ::= {(?<parent>.*)}");

        private readonly Regex objectTypeRegex =
            new Regex(@"(?=[a-z|A-Z](.*)OBJECT-TYPE)(?s).*?(?>::= { [a-z|A-Z|0-9]* [0-9]* })");

        private readonly Regex parentAndId = new Regex(@"(?<=::= { )(?<parent>.*) (?<parentId>\d+)");

        private readonly Regex sequenceRegex =
            new Regex(@"(?<name>.*) \s*::=\s* SEQUENCE((.*)\n)((?s)(?<values>.*?)(?>)})", RegexOptions.Multiline);

        private readonly Regex splitDotNumbers = new Regex(@"\((?<min>[0-9]*)..(?<max>[0-9]*)\)");
        private readonly Regex status = new Regex(@"(?<=STATUS  )(?<status>.*)");
        private readonly Regex typeOfAccess = new Regex(@"(?<=ACCESS  )(?<access>.*)");
        private readonly Regex typeOfNode = new Regex(@"(?<=SYNTAX  )(?<syntax>.*)");

        public Parser(IFileReader fileReader, IImportsLoader importsLoader)
        {
            this.fileReader = fileReader;
            this.importsLoader = importsLoader;
        }

        public MibNode GenerateTree()
        {
            //Hardcoded roots of the tree
            var masterNode = new MibNode(1, "ISO", null);
            masterNode.AddChild(new MibNode(3, "org", masterNode));

            var org = masterNode.GetMibNodeStack().FirstOrDefault(node => node.NodeName == "org");
            org?.AddChild(new MibNode(6, "dod", org));

            var dod = masterNode.GetMibNodeStack().FirstOrDefault(node => node.NodeName == "dod");
            dod?.AddChild(new MibNode(1, "internet", dod));

            var internet = masterNode.GetMibNodeStack().FirstOrDefault(node => node.NodeName == "internet");
            internet?.AddChild(new MibNode(2, "mgmt", internet));


            var mibText = fileReader.GetFileEntireText(ParserConst.MIBPath);

            //Load imports
            var imports = importsLoader.ParseImports(mibText); //TODO load this files

            var objectIdentifierMatch = objectIdentifierRegex.Matches(mibText);

            foreach (Match match in objectIdentifierMatch)
            {
                var groups = match.Groups;

                var name = groups["name"].ToString().Trim(' ');
                var parent = groups["parent"].ToString().Split(' ')[1].Trim(' ');
                var valueString = groups["parent"].ToString().Split(' ')[2].Trim(' ');
                var value = int.Parse(valueString);

                var parentNode = masterNode.GetMibNodeStack().FirstOrDefault(node => node.NodeName == parent);
                parentNode?.AddChild(new MibNode(value, name, parentNode));
            }

            var sequenceMatch = sequenceRegex.Matches(mibText);
            foreach (Match match in sequenceMatch)
            {
                var sequenceValues = getSequenceValuesRegex.Matches(match.Groups["values"].Value);
                foreach (Match sequenceValue in sequenceValues)
                {
                    var name = sequenceValue.Groups["name"].Value;
                    var value = sequenceValue.Groups["value"].Value;

                    //TODO 
                }
            }

            var objectTypeMatch = objectTypeRegex.Matches(mibText);
            foreach (Match match in objectTypeMatch)
            {
                var objectTypeText = match.Value;
                var name = nameOfNode.Match(objectTypeText).Groups["name"].Value.Trim(' ');
                var typeOfNode = this.typeOfNode.Match(objectTypeText).Groups["syntax"];
                var access = typeOfAccess.Match(objectTypeText).Groups["access"];
                var status = this.status.Match(objectTypeText).Groups["status"];
                var description = this.description.Match(objectTypeText).Groups["description"];
                var parentName = parentAndId.Match(objectTypeText).Groups["parent"];
                var id = parentAndId.Match(objectTypeText).Groups["parentId"];

                if (IsObjectComplete(name, typeOfNode, access, status, parentName, id))
                {
                    var idParsed = int.Parse(id.ToString());

                    var parentNode = masterNode.GetMibNodeStack()
                        .FirstOrDefault(node => node.NodeName == parentName.ToString());

                    AccessTypes accessType;
                    if (access.Value.Contains("read-write"))
                        accessType = AccessTypes.ReadWrite;
                    else if (access.Value.Contains("read-only"))
                        accessType = AccessTypes.ReadOnly;
                    else
                        accessType = AccessTypes.NoAccess;

                    if (typeOfNode.ToString().Contains("{"))
                    {
                        var valueOfType = complexTypeOfNode.Match(match.Value).Value;
                        var values = getNumbers.Matches(valueOfType);
                        var min = values[0].Value;
                        var max = values[values.Count - 1].Value;
                        min = min.Substring(0, min.Length - 1);
                        max = max.Substring(0, max.Length - 1);

                        var limiter = new Limiter(int.Parse(min), int.Parse(max));

                        parentNode?.AddChild(new ObjectType(idParsed, name, parentNode, typeOfNode.Value, accessType,
                            status.Value, description.Value, limiter));
                    }
                    else if (typeOfNode.ToString().Contains(".."))
                    {
                        var numbers = splitDotNumbers.Match(typeOfNode.Value);
                        var min = numbers.Groups["min"].Value;
                        var max = numbers.Groups["max"].Value;
                        var limiter = new Limiter(int.Parse(min), int.Parse(max));

                        parentNode?.AddChild(new ObjectType(idParsed, name, parentNode, typeOfNode.Value, accessType,
                            status.Value, description.Value, limiter));
                    }
                    else
                    {
                        parentNode?.AddChild(new ObjectType(idParsed, name, parentNode, typeOfNode.Value, accessType,
                            status.Value, description.Value));
                    }
                }
            }
            return masterNode;
        }

        private bool IsObjectComplete(string name, Group typeOfNode, Group access, Group status, Group parent, Group id)
        {
            return !string.IsNullOrEmpty(name) && typeOfNode.Success && access.Success && status.Success &&
                   parent.Success && id.Success;
        }
    }
}
