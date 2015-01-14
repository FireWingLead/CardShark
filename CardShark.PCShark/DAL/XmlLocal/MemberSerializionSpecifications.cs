using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CardShark.PCShark.DAL.XmlLocal
{
    public delegate void MemberWriter(object member, TextWriter writeTo);

    public class MemberSerializionSpecifications
    {
        public MemberInfo Member;
        public bool WriteMember = true;
        public MemberWriter ItemWriter;
    }
}
