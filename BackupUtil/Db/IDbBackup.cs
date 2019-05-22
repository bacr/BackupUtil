using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BackupUtil.Db
{
    public interface IDbBackup
    {
        Task BackupAsync(BackupType type);
    }

    public enum BackupType
    {
        Full,
        Differential,
        TransactionLog
    }
}
