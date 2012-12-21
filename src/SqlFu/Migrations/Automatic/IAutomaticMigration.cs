namespace SqlFu.Migrations.Automatic
{
    public interface IAutomaticMigration
    {
        /// <summary>
        /// Tries to install/update all the specified schemas.
        /// If no schema is specified it tries to process all schemas found
        /// </summary>
        /// <param name="schemas"></param>
        void Execute(params string[] schemas);

        /// <summary>
        /// Removes the specified schemas names form the tracking table.
        /// It doesn't remove actual tables. 
        /// Next time the automatic migrations are run, these schemas will be installed again.
        /// Don't use it unless you have a good reason
        /// </summary>
        /// <param name="schemas"></param>
        void Untrack(params string[] schemas);
    }
}