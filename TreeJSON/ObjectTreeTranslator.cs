using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TreeJSON
{
	public class ObjectTreeTranslator<ClassType>
	{
		private static readonly Type[] primitiveTypes = new Type[] {
			typeof(short), typeof(int), typeof(long)
		};
		Type classType = typeof(ClassType);
		PropertyInfo[] classProps = typeof(ClassType).GetProperties(BindingFlags.Public | BindingFlags.Instance);
		
		public ObjectTreeTranslator() {
		
		}

		public ClassType TreeToObject(JSONNode tree) {
			throw new NotImplementedException();
			//foreach (PropertyInfo property in classProps) {
			//	if (property.PropertyType == ) {
					 
			//	}
			//}
		}
	}
}
