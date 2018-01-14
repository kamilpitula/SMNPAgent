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
        public AccessTypes Access { get; private set; }
        public string Status { get; private set; }
        public string Description { get; private set; }
        private readonly Limiter limiter;
        public int IntValue { get; set; }
        public string OctetStringValue { get; set; }
        public NodeTypes nodeType { get; set; }

        public ObjectType(int nodeId, string nodeName, MIBNode parent, string typeOfNode, AccessTypes access, string status, string description, Limiter limiter = null) : base(nodeId, nodeName, parent)
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
            builder.Append("Access: ").AppendLine(Access.ToString());
            builder.Append("Status: ").AppendLine(Status);
            builder.Append("Description: ").AppendLine(Description);

            return builder.ToString();
        }
        public bool SetValue(string value)
        {
            if (limiter != null)
            {
                if (limiter.Check(value))
                {
                    OctetStringValue = value;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                OctetStringValue = value;
                return true;
            }

        }
        public bool SetValue(int value)
        {
            if (limiter != null)
            {
                if (limiter.Check(value))
                {
                    IntValue = value;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                IntValue = value;
                return true;
            }
        }

        public bool IsReadable()
        {
            if (Access != AccessTypes.No_access) return true;
            else return false;
        }

        public bool IsWritable()
        {
            if (Access == AccessTypes.Read_write) return true;
            else return false;
        }
    }
}
