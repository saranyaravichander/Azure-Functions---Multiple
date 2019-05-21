﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hl7BlobToCosmos
{
    public static class StringExtentions
    {
      
            public static string UnEscapeHL7(this string str)
            {
                return str.Replace("\\T\\", "&").Replace("\\S\\", "^").Replace("\\E\\", "\\").Replace("\\R\\", "~").Replace("\\.br\\","\n");
            }
        
    }
}
