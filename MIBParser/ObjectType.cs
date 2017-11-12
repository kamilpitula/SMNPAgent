using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBParser
{
    public class ObjectType : MIBNode
    {
        public string TypeOfNode { get; private set; }
        public string Access { get; private set; }
        public string Status { get; private set; }
        public string Description { get; private set; }
        private readonly Limiter limiter;

        public ObjectType(int nodeId, string nodeName, MIBNode parent, string typeOfNode, string access, string status, string description, Limiter limiter = null) : base(nodeId, nodeName, parent)
        {
            TypeOfNode = typeOfNode;
            Access = access;
            Status = status;
            Description = description;
            this.limiter = limiter;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(NodeId).Append(".").AppendLine(NodeName);
            builder.Append("Type of Node: ").AppendLine(TypeOfNode);
            builder.Append("Access: ").AppendLine(Access);
            builder.Append("Status: ").AppendLine(Status);
            builder.Append("Description: ").AppendLine(Description);

            return builder.ToString();
        }
    }
}
