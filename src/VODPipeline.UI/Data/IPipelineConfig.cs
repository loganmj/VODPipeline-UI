namespace VODPipeline.UI.Data
{
    public interface IPipelineConfig
    {
        public string InputDirectory { get; }
        public string OutputDirectory { get; }
        public string ArchiveDirectory { get; }
        public bool EnableHighlights { get; }
        public bool EnableScenes { get; }
        public int SilenceThreshold { get; }
    }
}
