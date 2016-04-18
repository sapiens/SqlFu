namespace SqlFu.Migrations.Automatic
{
    public interface IAutomaticMigration
    {
        /// <summary>
        /// Tries to install/update all the specified schemas.
        /// If no schema is specified it tries to process all schemas found
        /// </summary>
        /// <param name="schemas"></param>
        void Install(params string[] schemas);

        /// <summary>
        /// Removes the specified schemas names form the tracking table.
        /// Runs the unistall function if exists on a latest version migration task
        /// Next time the automatic migrations are run, these schemas will be installed again.
        /// Don't use it unless you have a good reason
        /// </summary>
        /// <param name="schemas"></param>
        void Uninstall(params string[] schemas);

        void SelfDestroy();
    }
}