using TaktTusur.Media.Infrastructure.Exceptions;

namespace TaktTusur.Media.Worker.Configuration;

public static class ConfigurationExtensions
{
	public static JobsTimetableConfiguration GetTimetable(this IConfiguration configuration)
	{
		var timetable = configuration.GetSection(nameof(JobsTimetableConfiguration)).Get<JobsTimetableConfiguration>();
		if (timetable == null)
		{
			throw new ApplicationConfigurationException(nameof(JobsTimetableConfiguration));
		}
		return timetable;
	}
}