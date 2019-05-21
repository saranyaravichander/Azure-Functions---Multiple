using System;
using System.Collections.Generic;
using System.Text;

namespace hl7ingestionallinone
{
    public static class StringExtentions
    {
      
            public static string UnEscapeHL7(this string str)
            {
                return str.Replace("\\T\\", "&").Replace("\\S\\", "^").Replace("\\E\\", "\\").Replace("\\R\\", "~").Replace("\\.br\\","\n");
            }
        
    }
}
