using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Cauldron.Anathema;
using Handelabra;
using System.Collections.Generic;

namespace CauldronTests
{
    [TestFixture()]
    public class AssemblyTests
    {
        [Test()]
        public void DeckListResourceNaming()
        {
            var asb = typeof(Cauldron.Necro.NecroCharacterCardController).Assembly;

            Console.WriteLine("Checking Embedded Resource Names...");
            foreach (var res in asb.GetManifestResourceNames())
            {
                var split = res.Split('.');

                Assert.AreEqual("Cauldron", split[0], "{0} has {1} and should have {2}", res, split[0], "Cauldron");
                Assert.AreEqual("DeckLists", split[1], "{0} has {1} and should have {2}", res, split[1], "DeckLists");
                StringAssert.EndsWith("DeckList", split[2], "{0} ends with {1} and end with {2}", res, split[2], "DeckList");
                Assert.AreEqual("json", split[3], "{0} has {1} and should have {2}", res, split[3], "json");
            }
            Console.WriteLine("Done");
        }


        [Test()]
        public void DeckListJsonValidation()
        {
            var asb = typeof(Cauldron.Necro.NecroCharacterCardController).Assembly;

            Console.WriteLine("Checking Embedded Resource Names...");
            foreach (var res in asb.GetManifestResourceNames())
            {
                Console.Write(res);
                Console.WriteLine("...");

                var stream = asb.GetManifestResourceStream(res);
                Assert.IsNotNull(stream, "'{0}' resource stream is null!", res);
                Assert.IsTrue(stream.Length != 0, "'{0}' resource stream is empty!", res);

                Boomlagoon.JSON.JSONObject jsonObject;
                using (var sr = new System.IO.StreamReader(stream))
                {
                    string text = sr.ReadToEnd();
                    Assert.IsFalse(string.IsNullOrEmpty(text), "'{0}' resource text is empty!", res);
                    jsonObject = Boomlagoon.JSON.JSONObject.Parse(text);
                }

                Assert.IsNotNull(jsonObject, "'{0}' jsonObject failed to parse.", res);

                var name = jsonObject.GetString("name");

                Assert.IsFalse(string.IsNullOrEmpty(name), "'{0}' failed to read name from jsonObject.", res);
            }
            Console.WriteLine("Done");
        }

    }
}
