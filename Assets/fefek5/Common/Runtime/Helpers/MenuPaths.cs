namespace fefek5.Common.Runtime.Helpers
{
    public static class MenuPaths
    {
        public static class fefek5
        {
            public const string PATH = "fefek5";

            public static class Systems
            {
                public const string PATH = fefek5.PATH + "/Systems";

                public static class SingletonSystem
                {
                    public const string PATH = Systems.PATH + "/Singleton System";
                }
                
                public static class ThemeSystem
                {
                    public const string PATH = Systems.PATH + "/Theme System";
                }
            }
        }
        
        public static class CONTEXT
        {
            public const string PATH = "CONTEXT";
            
            public static class Image
            {
                public const string PATH = CONTEXT.PATH + "/Image";
            }
            
            public static class TMP_Text
            {
                public const string PATH = CONTEXT.PATH + "/TMP_Text";
            }
        }
    }
}