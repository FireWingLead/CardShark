using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CardShark.PCShark.DAL.XmlLocal
{

	/// <summary>
	/// <para>Inheritable class for serializing model classes as XML and/or saving them to files.</para>
	/// <para>This class uses inverse algorithms to the <code>ModelReader</code> class.</para>
	/// <para>This class can be inherited to create writers for specific classes and customize which properties are written, and how.</para>
	/// </summary>
	public class ModelWriter<ModelType>
	{
		private static void WriteObj(object item, TextWriter writeTo) { if (item == null || writeTo == null) return; writeTo.Write(item.ToString()); }
		private static readonly KeyValuePair<Type, MemberWriter>[] baseDefTypeWriters = {
			
		};

		protected readonly Type ModelTypeType = typeof(ModelType);

		public ModelWriter() {
			MemberInfo[] members = ModelTypeType.GetMembers(BindingFlags.Public);
			foreach (MemberInfo member in members) {
				switch (member.MemberType) {
					case MemberTypes.Property:
						PropertyInfo prop = (PropertyInfo)member;
						memberSerSpecs.Add(member, new MemberSerializionSpecifications() { Member = member, WriteMember = true, ItemWriter = GetDefaultWriterFor(prop.PropertyType) });
						break;
				}
			}
		}

		private MemberWriter GetBaseDefaultWriterFor(Type type) {
			foreach (KeyValuePair<Type, MemberWriter> writer in baseDefTypeWriters)
				if (type == writer.Key)
					return writer.Value;
			return WriteObj;
		}
		protected virtual MemberWriter GetDefaultWriterFor(Type type) {
			if (defaultTypeWriters.ContainsKey(type))
				return defaultTypeWriters[type];
			return GetBaseDefaultWriterFor(type);
		}

		public void WriteItemTo(ModelType item, Stream toStream) {
			throw new NotImplementedException();
		}
		public void SaveItemTo(ModelType item, string fileName) {
			throw new NotImplementedException();
		}
		public string SeriailizeItem(ModelType item) {
			throw new NotImplementedException();
		}

		private Dictionary<MemberInfo, MemberSerializionSpecifications> memberSerSpecs = new Dictionary<MemberInfo, MemberSerializionSpecifications>();
		private Dictionary<Type, MemberWriter> defaultTypeWriters = new Dictionary<Type, MemberWriter>();


	}
}
