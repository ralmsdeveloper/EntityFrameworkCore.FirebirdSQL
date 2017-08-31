using Xunit;

namespace SouchProd.EntityFrameworkCore.Firebird.FunctionalTests.Tests
{
	public class SkipAppVeyorFact : FactAttribute {

		public SkipAppVeyorFact() {
			if(AppConfig.AppVeyor) {
				Skip = "Test does not work with AppVeyor's Firebird version";
			}
		}

	}
}
