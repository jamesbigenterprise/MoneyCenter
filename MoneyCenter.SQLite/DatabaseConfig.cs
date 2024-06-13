

namespace MoneyCenter.SQLiteWrapper
{
    public static class DatabaseConfig
    {
        public const string DatabaseFilename = "MoneyCenterSQLite.db3";

        public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

        //Note for the eventual serverless cloud
        //checked ways to encrypt this file
        //find the best way to transfer it to other devices
        //      whole file
        //      or just re execute the commands in the sister database in the other device
        public static string DatabasePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseFilename);


    }
}
