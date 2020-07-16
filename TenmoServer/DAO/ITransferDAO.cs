using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        Transfer SendTransfer(Transfer transfer);
        Transfer GetTransfer(int transferId);
        List<Transfer> GetTransfers(int userId);
        List<Transfer> GetPendingTransfers(int userId);
        Transfer UpdateTransfer(Transfer transferToUpdate);
    }
}
