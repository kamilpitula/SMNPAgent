using System.Collections.Generic;
using System.Text;

namespace MIBParser
{
    public class MibNode
    {
        //private Limiter = new Limiter();

        public MibNode(int nodeId, string nodeName, MibNode parent)
        {
            NodeId = nodeId;
            NodeName = nodeName;
            Children = new List<MibNode>();
            Parent = parent;
        }

        public List<MibNode> Children { get; }
        public MibNode Parent { get; }
        public int NodeId { get; }
        public string NodeName { get; }

        public void AddChild(MibNode child)
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
                result = Parent.GetOID() + "." + NodeId;
            else
                result = NodeId.ToString();

            return result;
        }
    }
}