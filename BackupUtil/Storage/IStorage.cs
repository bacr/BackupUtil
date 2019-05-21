using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BackupUtil.Storage
{
    public interface IStorage
    {
        Task BackupAsync(string filePath, string destinationPath);
    }
}
