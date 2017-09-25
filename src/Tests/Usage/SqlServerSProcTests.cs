using SqlFu;

namespace Tests.Usage
{
    public class SqlServerSProcTests:AStoredProcsTests
    {
        public SqlServerSProcTests() : base()
        {
        }

        protected override void CreateSproc()
        {
            _db.AddDbObjectOrIgnore(@"CREATE PROCEDURE spTest
	@id int,
	@pout varchar(50) out
AS
BEGIN
		SET NOCOUNT ON;
   set @pout='bla';
	SELECT 1 as id,'bla' as name, @id as [input];
	return 100;
END");
            
        }
    }
}