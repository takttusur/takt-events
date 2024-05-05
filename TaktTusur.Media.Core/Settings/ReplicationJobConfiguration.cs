namespace TaktTusur.Media.Core.Settings;

public class ReplicationJobConfiguration : JobConfigurationBase
{
	public int MaxReplicatedItems { get; set; }
	
	public int CommitBuffer { get; set; }
}