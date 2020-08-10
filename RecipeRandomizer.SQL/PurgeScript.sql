-- disable all constraints
EXEC dbo.sp_MSForEachTable "ALTER TABLE ? NOCHECK CONSTRAINT all"

-- delete data in all tables
EXEC dbo.sp_MSForEachTable "DELETE FROM ?"

-- reset all identities
EXEC dbo.sp_MSforeachtable '
    IF OBJECTPROPERTY(object_id(''?''), ''TableHasIdentity'') = 1
    DBCC CHECKIDENT (''?'', RESEED, 1)'

-- enable all constraints
EXEC dbo.sp_MSForEachTable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"
