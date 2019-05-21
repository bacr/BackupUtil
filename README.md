# BackupUtil

BackupUtil allows to back up SQL Server database to Azure blob storage

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
      Logging__LogLevel__Default: 'Trace'
    volumes:
      - ./jobs.xml:/app/jobs.xml
```

## Default Quarts jobs.xml

Default trigger is set up to do Full back up every day at 0:00

```xml
    <trigger>
      <cron>
        <name>DbBackupTrigger</name>
        <group>BackupUtil</group>
        <job-name>DbBackupJob</job-name>
        <job-group>BackupUtil</job-group>
        <job-data-map>
          <entry>
            <key>backupType</key>
            <value>Full</value>
          </entry>
        </job-data-map>
        <cron-expression>0 0 0 * * ?</cron-expression>
      </cron>
    </trigger>
```