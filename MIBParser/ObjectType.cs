using System.Text;

namespace MIBParser
{
    public class ObjectType : MibNode
    {
        private readonly Limiter limiter;

        public ObjectType(int nodeId, string nodeName, MibNode parent, string typeOfNode, AccessTypes access,
            string status, string description, Limiter limiter = null) : base(nodeId, nodeName, parent)
        {
            TypeOfNode = typeOfNode;
            Access = access;
            Status = status;
            Description = description;
            this.limiter = limiter;
        }

        public string TypeOfNode { get; }
        public AccessTypes Access { get; }
        public string Status { get; }
        public string Description { get; }
        public int IntValue { get; set; }
        public string OctetStringValue { get; set; }
        public NodeTypes NodeType { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(NodeId).Append(".").AppendLine(NodeName);
            builder.Append("Type of Node: ").AppendLine(TypeOfNode);
            builder.Append("Access: ").AppendLine(Access.ToString());
            builder.Append("Status: ").AppendLine(Status);
            builder.Append("Description: ").AppendLine(Description);

            return builder.ToString();
        }

        public bool SetValue(string value)
        {
            if (limiter != null)
                if (limiter.Check(value))
                {
                    OctetStringValue = value;
                    return true;
                }
                else
                {
                    return false;
                }
            OctetStringValue = value;
            return true;
        }

        public bool SetValue(int value)
        {
            if (limiter != null)
                if (limiter.Check(value))
                {
                    IntValue = value;
                    return true;
                }
                else
                {
                    return false;
                }
            IntValue = value;
            return true;
        }

        public bool IsReadable()
        {
            if (Access != AccessTypes.NoAccess) return true;
            return false;
        }

        public bool IsWritable()
        {
            if (Access == AccessTypes.ReadWrite) return true;
            return false;
        }
    }
}