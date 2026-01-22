namespace VODPipeline.UI.Data
{
    public interface IPipelineConfig
    {
        public string InputDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public string ArchiveDirectory { get; set; }
        public bool EnableHighlights { get; set; }
        public bool EnableScenes { get; set; }
        public int SilenceThreshold { get; set; }
    }
}
