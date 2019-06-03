using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SimpleStepWriter.Content.Internal
{    
    /// <summary>
    /// Mandatory STEP AP214 information.
    /// </summary>
    internal static class Default
    {
        public static void GetHeader(string fileName, string fileDescription, in StringBuilder sb, in List<string> stepEntries)
        {
            string date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

            sb.Append(@"ISO-10303-21;");
            sb.AppendLine().Append(@"HEADER;");
            sb.AppendLine().Append(@"FILE_DESCRIPTION(('" + fileDescription + "'),'2;1');");
            sb.AppendLine().Append(@"FILE_NAME('" + fileName + "', '" + date + "', ('author'),('organisation'), 'preprocessor_version','originating_system','authorization');");
            sb.AppendLine().Append(@"FILE_SCHEMA(('AUTOMOTIVE_DESIGN { 1 0 10303 214 1 1 1 1 }'));");
            sb.AppendLine().Append(@"ENDSEC;");
            sb.AppendLine().Append(@"DATA;");
            sb.AppendLine().Append(@"#1 = APPLICATION_PROTOCOL_DEFINITION('international standard', 'automotive_design',2000,#2);");
            sb.AppendLine().Append(@"#2 = APPLICATION_CONTEXT('core data for automotive mechanical design processes');");

            stepEntries.Add(sb.ToString());
            sb.Clear();
        }        
        
        public static void GetFooter(in StringBuilder sb, in List<string> stepEntries)
        {
            sb.Append(@"ENDSEC;");
            sb.AppendLine().Append(@"END-ISO-10303-21;");

            stepEntries.Add(sb.ToString());
            sb.Clear();
        }

    }
}
