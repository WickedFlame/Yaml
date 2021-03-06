﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace WickedFlame.Yaml.Tests
{
	[TestFixture]
	public class Integration
	{
		[Test]
		public void LoadDataFromFile()
		{
			var path = Uri.UnescapeDataString(new UriBuilder(typeof(Integration).Assembly.CodeBase).Path);
			path = Path.Combine(Path.GetDirectoryName(path), "TestData", "users.yml");

			var reader = new YamlReader();
			var users = reader.Read<List<User>>(path);

			Assert.IsTrue(users.Count == 3);

		}
		public class User
		{
			public enum UserType
			{
				User,
				Client
			}

			public User()
			{
				Type = UserType.User;
			}

			public User(string id, string name)
				: this(id, name, UserType.User)
			{
			}

			public User(string id, string name, UserType type)
			{
				Id = id;
				Name = name;
				Type = type;
			}

			public string Id { get; set; }

			public UserType Type { get; set; }

			public string Name { get; set; }

			public string Password { get; set; }

			public Scope[] Scopes { get; set; }
		}

		public class Scope
		{
			public string Name { get; set; }
		}
	}
}
