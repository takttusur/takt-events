namespace TaktTusur.Media.Infrastructure.Exceptions;

public class ApplicationConfigurationException(string configurationParameter)
	: Exception($"Configuration for {configurationParameter} is not valid");