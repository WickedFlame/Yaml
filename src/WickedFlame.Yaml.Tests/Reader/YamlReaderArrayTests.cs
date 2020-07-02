﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace WickedFlame.Yaml.Tests.Reader
{
    [TestFixture]
    //[Ignore("Array is not yet implemented")]
    public class YamlReaderArrayTests
    {
        [Test]
        public void WickedFlame_Yaml_YamlReader_Array()
        {
            var lines = new[]
            {
                "Array:",
                "  - one",
                "  - 1"
            };

            var reader = new YamlReader();
            var data = reader.Read<StringNode>(lines);

            Assert.That(data.Array.Count() == 2);
            Assert.That(data.Array[0] == "one");
            Assert.That(data.Array[1] == "1");
        }

        [Test]
        public void WickedFlame_Yaml_YamlReader_Array_Object()
        {
            var lines = new[]
            {
                "ObjectArray:",
                "  - Name: one",
                "    Id: 1",
                "  - Name: two",
                "    Id: 2"
            };

            var reader = new YamlReader();
            var data = reader.Read<StringNode>(lines);

            Assert.That(data.ObjectArray.Count() == 2);
            Assert.That(data.ObjectArray[0].Name == "one" && data.ObjectArray[0].Id == 1);
            Assert.That(data.ObjectArray[1].Name == "two" && data.ObjectArray[1].Id == 2);
        }

        [Test]
        public void WickedFlame_Yaml_YamlReader_Array_Nested()
        {
	        var lines = new[]
	        {
				"- ObjectArray:",
		        "    - Name: one",
		        "      Id: 1",
		        "    - Name: two",
		        "      Id: 2",
		        "- ObjectArray:",
		        "    - Name: one-2",
		        "      Id: 1-2",
		        "    - Name: two-2",
		        "      Id: 2-2"
			};

	        var reader = new YamlReader();
	        var data = reader.Read<List<StringNode>>(lines);

	        Assert.That(data.Count() == 2);
	        //Assert.That(data.ObjectArray[0].Name == "one" && data.ObjectArray[0].Id == 1);
	        //Assert.That(data.ObjectArray[1].Name == "two" && data.ObjectArray[1].Id == 2);
        }

		public class StringNode
        {
            public string[] Array { get; set; }

            public Node[] ObjectArray { get; set; }
        }

        public class Node
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
