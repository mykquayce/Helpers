using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Helpers.RabbitMQ.Tests.Fixtures
{
	public class UserSecretsFixture
	{
		private const string _sectionName = nameof(Models.RabbitMQSettings);

		public UserSecretsFixture()
		{
			var @base = new Helpers.XUnitClassFixtures.UserSecretsFixture();

			Settings = @base
				.Configuration.GetSection(_sectionName)
				.Get<Models.RabbitMQSettings>()
				?? throw new KeyNotFoundException($"{_sectionName} not found in user secrets");
		}

		public Models.RabbitMQSettings Settings { get; }
	}
}
