using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.Models;
using TenmoServer.DAO;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private readonly ITransferDAO transferDAO;

        public TransferController(ITransferDAO _transferDAO)
        {
            transferDAO = _transferDAO;
        }

        [HttpGet("all/{userId}")]
        public ActionResult<List<Transfer>> GetAllTransfers(int userId)
        {
            List<Transfer> transfers = transferDAO.GetTransfers(userId);

            if (transfers != null)
            {
                return Ok(transfers);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("pending/{userId}")]
        public ActionResult<List<Transfer>> GetAllPendingTransfers(int userId)
        {
            List<Transfer> transfers = transferDAO.GetPendingTransfers(userId);

            if (transfers != null)
            {
                return Ok(transfers);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Transfer> GetTransfer(int id)
        {
            Transfer transfer = transferDAO.GetTransfer(id);
            
            if(transfer != null)
            {
                return Ok(transfer);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public ActionResult<Transfer> SendTransfer(Transfer transferToSend)
        {
            Transfer transfer = transferDAO.SendTransfer(transferToSend);

            if (transfer != null)
            {
                return Ok(transfer);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Transfer> UpdateTransfer(Transfer transferToUpdate)
        {
            Transfer transfer = transferDAO.UpdateTransfer(transferToUpdate);

            if (transfer != null)
            {
                return Ok(transfer);
            }
            else
            {
                return NotFound();
            }
        }
    }
}