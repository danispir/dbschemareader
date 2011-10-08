﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSchemaReader.DataSchema
{
    /// <summary>
    /// Extensions to enable schema to be created with a simple fluent interface
    /// </summary>
    public static class DatabaseSchemaConstraintExtensions
    {

        /// <summary>
        /// Makes this column the primary key.
        /// </summary>
        /// <param name="databaseColumn">The database column.</param>
        /// <returns></returns>
        public static DatabaseColumn AddPrimaryKey(this DatabaseColumn databaseColumn)
        {
            return databaseColumn.AddPrimaryKey(null);
        }

        /// <summary>
        /// Adds the primary key.
        /// </summary>
        /// <param name="databaseColumn">The database column.</param>
        /// <param name="primaryKeyName">Name of the primary key.</param>
        /// <returns></returns>
        public static DatabaseColumn AddPrimaryKey(this DatabaseColumn databaseColumn, string primaryKeyName)
        {
            if (databaseColumn == null) throw new ArgumentNullException("databaseColumn", "databaseColumn must not be null");
            var table = databaseColumn.Table;
            table.PrimaryKey = new DatabaseConstraint
            {
                ConstraintType = ConstraintType.PrimaryKey,
                TableName = table.Name,
                Name = primaryKeyName
            };
            table.PrimaryKey.Columns.Add(databaseColumn.Name);
            databaseColumn.IsPrimaryKey = true;
            databaseColumn.Nullable = false; //you can't have a nullable pk
            return databaseColumn;
        }

        /// <summary>
        /// Adds the identity.
        /// </summary>
        /// <param name="databaseColumn">The database column.</param>
        /// <returns></returns>
        public static DatabaseColumn AddIdentity(this DatabaseColumn databaseColumn)
        {
            if (databaseColumn == null) throw new ArgumentNullException("databaseColumn", "databaseColumn must not be null");
            var table = databaseColumn.Table;
            if (table.HasIdentityColumn && !databaseColumn.IsIdentity)
            {
                var existingIdentity = table.Columns.First(x => x.IsIdentity);
                existingIdentity.IsIdentity = false;
            }
            databaseColumn.IsIdentity = true;
            return databaseColumn;
        }

        /// <summary>
        /// Adds a foreign key with a single column
        /// </summary>
        /// <param name="databaseColumn">The database column.</param>
        /// <param name="foreignKeyName">Name of the foreign key.</param>
        /// <param name="foreignTableName">Name of the foreign table.</param>
        /// <returns></returns>
        public static DatabaseColumn AddForeignKey(this DatabaseColumn databaseColumn, string foreignKeyName, string foreignTableName)
        {
            if (databaseColumn == null) throw new ArgumentNullException("databaseColumn", "databaseColumn must not be null");
            if (string.IsNullOrEmpty(foreignTableName)) throw new ArgumentNullException("foreignTableName", "foreignTableName must not be null");
            var table = databaseColumn.Table;
            var foreignKey = new DatabaseConstraint
            {
                ConstraintType = ConstraintType.ForeignKey,
                Name = foreignKeyName,
                TableName = table.Name,
                RefersToTable = foreignTableName
            };
            table.ForeignKeys.Add(foreignKey);
            databaseColumn.IsForeignKey = true;

            //add the inverse relationship
            var fkTable = table.DatabaseSchema.FindTableByName(foreignTableName);
            if (fkTable != null) fkTable.ForeignKeyChildren.Add(table);

            return databaseColumn;
        }

        /// <summary>
        /// Adds a foreign key with a single column
        /// </summary>
        /// <param name="databaseColumn">The database column.</param>
        /// <param name="foreignKeyName">Name of the foreign key.</param>
        /// <param name="foreignTable">The foreign table.</param>
        /// <returns></returns>
        public static DatabaseColumn AddForeignKey(this DatabaseColumn databaseColumn, string foreignKeyName, Func<IEnumerable<DatabaseTable>, DatabaseTable> foreignTable)
        {
            if (databaseColumn == null) throw new ArgumentNullException("databaseColumn", "databaseColumn must not be null");
            if (foreignTable == null) throw new ArgumentNullException("foreignTable", "foreignTable must not be null");
            var table = databaseColumn.Table;
            var fkTable = foreignTable(table.DatabaseSchema.Tables);
            return databaseColumn.AddForeignKey(foreignKeyName, fkTable.Name);
        }

        /// <summary>
        /// Adds a foreign key with a single column (without a name)
        /// </summary>
        /// <param name="databaseColumn">The database column.</param>
        /// <param name="foreignTableName">Name of the foreign table.</param>
        /// <returns></returns>
        public static DatabaseColumn AddForeignKey(this DatabaseColumn databaseColumn, string foreignTableName)
        {
            return databaseColumn.AddForeignKey(null, foreignTableName);
        }

        /// <summary>
        /// Makes this column a unique key.
        /// </summary>
        /// <param name="databaseColumn">The database column.</param>
        /// <returns></returns>
        public static DatabaseColumn AddUniqueKey(this DatabaseColumn databaseColumn)
        {
            return databaseColumn.AddUniqueKey(null);
        }

        /// <summary>
        /// Adds a unique key.
        /// </summary>
        /// <param name="databaseColumn">The database column.</param>
        /// <param name="uniqueKeyName">Name of the unique key.</param>
        /// <returns></returns>
        public static DatabaseColumn AddUniqueKey(this DatabaseColumn databaseColumn, string uniqueKeyName)
        {
            if (databaseColumn == null) throw new ArgumentNullException("databaseColumn", "databaseColumn must not be null");
            var table = databaseColumn.Table;
            var uk = new DatabaseConstraint
             {
                 ConstraintType = ConstraintType.UniqueKey,
                 TableName = table.Name,
                 Name = uniqueKeyName
             };
            uk.Columns.Add(databaseColumn.Name);
            table.UniqueKeys.Add(uk);
            databaseColumn.IsUniqueKey = true;
            return databaseColumn;
        }
    }
}
