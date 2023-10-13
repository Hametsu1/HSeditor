namespace HSeditor.Classes.Other
{
    public class CrashFile
    {
        public string FileName { get; private set; }
        public string FullPath { get; private set; }
        public string Content { get; private set; }

        public CrashFile(string filename, string content, string fullPath)
        {
            this.FileName = filename;
            this.Content = content;
            this.FullPath = fullPath;
        }
    }
}
