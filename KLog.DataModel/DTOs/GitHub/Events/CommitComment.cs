namespace KLog.DataModel.DTOs.GitHub.Events
{
    /// <summary>
    /// Triggers when a commit comment is created
    /// </summary>
    public class CommitComment : GitHubEvent
    {
        public Comment Comment { get; set; }
    }
}
