# BackupUtil

BackupUtil allows to back up SQL Server database to Azure blob storage test

## Sample docker-compose file
```yaml
services:
  backup:
    image: bacr/backuputil
    environment:
      SqlBackup__ConnectionString: <ConnectionString>
      SqlBackup__Database: DatabaseName
      SqlBackup__BackupPath: /backup/path/in/sqlserver
      DbBackupJob__BackupPath: /backup/path/in/backuputil
      DbBackupJob__StoragePath: path/in/azure
      AzureStorage__ConnectionString: <ConnectionString>
      AzureStorage__Container: container-name
      BackupSchedulerJob__Db__0__Type: Full
      BackupSchedulerJob__Db__0__Cron: '0 0 0 * * ?'
      BackupSchedulerJob__Db__1__Type: Differential
      BackupSchedulerJob__Db__1__Cron: '0 0 1-23 * * ?'
      Logging__LogLevel__Default: 'Trace'
```
This configuration will do Full back up every day at 0:00 and Differential backup every hour except 0:00
