using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIBParser
{
    public static class MIBTreeExtensions
    {
        public static IEnumerable<MIBNode> GetMibNodeStack(this MIBNode root)
        {
            var nodes = new Stack<MIBNode>(new[] {root});
            while (nodes.Any())
            {
                var node = nodes.Pop();
                yield return node;
                foreach (var n in node.Children) nodes.Push(n);
            }
        }

        public static string GetLastChildrenString(this MIBNode root, string nodeName = null)
        {
            var builder = new StringBuilder();


            foreach (var child in root.Children)
            {
                if (string.IsNullOrEmpty(nodeName) && child.Children.Count == 0)
                    BuildString(builder, child);

                if (child.NodeName == nodeName)
                    BuildString(builder, child);
                builder.Append(child.GetLastChildrenString(nodeName));
            }

            return builder.ToString();
        }

        public static string GetTreeString(this MIBNode root, string indent, bool last)
        {
            var builder = new StringBuilder();
            builder.Append(indent);
            if (last)
            {
                builder.Append("\\-");
                indent += "  ";
            }
            else
            {
                builder.Append("|-");
                indent += "| ";
            }
            builder.Append(root.NodeId).Append(". ").AppendLine(root.NodeName);

            for (var i = 0; i < root.Children.Count; i++)
                builder.Append(root.Children[i].GetTreeString(indent, i == root.Children.Count - 1));

            return builder.ToString();
        }

        private static void BuildString(StringBuilder builder, MIBNode child)
        {
            var temp = child;
            var idList = new List<int>();
            while (temp.Parent != null)
            {
                temp = temp.Parent;
                idList.Add(temp.NodeId);
            }
            for (var j = idList.Count - 1; j >= 0; j--)
            {
                builder.Append(idList[j].ToString());

                builder.Append('.');
            }
            builder.Append(child.NodeId);
            builder.Append(' ');
            builder.AppendLine(child.NodeName);
        }
    }
}