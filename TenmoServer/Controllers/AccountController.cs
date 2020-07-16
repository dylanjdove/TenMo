using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountsDAO accountsDAO;

        public AccountController(IAccountsDAO _accountsDAO)
        {
            accountsDAO = _accountsDAO;
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccount(int id)
        {
            Account account = accountsDAO.GetAccount(id);
            
            if(account != null)
            {
                return Ok(account);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public ActionResult<List<Account>> GetAccounts()
        {
            List<Account> accounts = accountsDAO.GetAccounts();

            if (accounts != null)
            {
                return Ok(accounts);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Account> UpdateAccount(Account account)
        {
            Account existingAccount = accountsDAO.GetAccount(account.UserId);

            if (existingAccount == null)
            {
                return NotFound("Account not found.");
            }
            Account result = accountsDAO.UpdateBalance(account);
            return Ok(result);
        }
    }
}