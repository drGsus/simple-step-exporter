using System;
using System.Globalization;

namespace SimpleStepWriter.Content.Internal
{    

    /// <summary>
    /// Mandatory STEP AP214 information.
    /// </summary>
    internal static class Default
    {
        public static string[] FileStart { get; private set; } = new[]{
            @"ISO-10303-21;"
        };

        public static string[] Header(string fileName, string fileDescription)
        {
            string date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
       
            return new[] {
                @"HEADER;",
                @"FILE_DESCRIPTION(('" + fileDescription + "'),'2;1');",
                @"FILE_NAME('" + fileName + "', '" + date + "', ('author'),('organisation'), 'preprocessor_version','originating_system','authorization');",
                @"FILE_SCHEMA(('AUTOMOTIVE_DESIGN { 1 0 10303 214 1 1 1 1 }'));",
                @"ENDSEC;"
            };
        }

        public static string[] DataStart { get; set; } = new[]
        {
            @"DATA;",
            @"#1 = APPLICATION_PROTOCOL_DEFINITION('international standard', 'automotive_design',2000,#2);",
            @"#2 = APPLICATION_CONTEXT('core data for automotive mechanical design processes');"
        };

        public static string[] DataEnd { get; set; } = new[]
        {
            @"ENDSEC;"
        };

        public static string[] FileEnd { get; set; } = new[]
        {
            @"END-ISO-10303-21;"
        };

    }
}
