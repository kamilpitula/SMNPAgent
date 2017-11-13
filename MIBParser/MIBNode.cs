using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MIBParser
{
    public class MIBNode
    {
        public List<MIBNode> Children { get; private set; }
        public MIBNode Parent { get; private set; }
        public int NodeId { get; private set; }
        public string NodeName { get; private set; }

        public MIBNode(int nodeId, string nodeName,MIBNode parent)
        {
            NodeId = nodeId;
            NodeName = nodeName;
            Children = new List<MIBNode>();
            Parent = parent;
        }

        public void AddChild(MIBNode child)
        {
            Children.Add(child);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("OID ID: ").AppendLine(NodeId.ToString());
            builder.Append("OID Name: ").AppendLine(NodeName);

            return builder.ToString();
        }

        public string GetOID()
        {
            string result;
            if (Parent != null)
                result = Parent.GetOID() + "." + NodeId.ToString();
            else
                result = NodeId.ToString();
            
            return result;
        }
    }
}
