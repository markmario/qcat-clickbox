using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Odes.Licence.Model;
using QCat;

namespace Odes.License.TestHarness
{
    [TestFixture]
    public class TestHarness
    {
        [Test]
        public void Can_call_document_koded()
        {
            var api = new QcatOdesApi();
            api.DocumentCoded(new DocumentCoded()
            {
                DateCreated = DateTimeOffset.Now,
                DocumentId = Guid.NewGuid(),
                MachineName = System.Environment.MachineName,
                ProjectId = Guid.NewGuid(),
                RequestId = new Guid("55cb57f2-3eac-4dc9-8ab6-7020fe5e72f4"),
                SId = "",
                UserName = "asdf@asdf.com"
            });
        }
        [Test]
        public void Can_validate_a_license()
        {
            LicenseValidation.CheckClient(null, null, () => Assert.Fail("This should have a valid license"), "Odes.License.TestHarness.pubkey.xml");
        }
    }
}
